using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MunicipalServicesApplication.Models;
using Newtonsoft.Json;

namespace MunicipalServicesApplication.Services
{
    public class EventService
    {
        private const string BASE_URL = "https://eventsincapetown.com/all-events/";
        private readonly HttpClient _httpClient;
        private List<LocalEvent> _allEvents;
        private bool _isLoading;

        public EventService()
        {
            _httpClient = new HttpClient();
            _allEvents = new List<LocalEvent>();
            _isLoading = false;
        }

        private TaskCompletionSource<bool> _loadingCompletionSource = new TaskCompletionSource<bool>();

        public Task WaitForInitialLoadAsync()
        {
            return _loadingCompletionSource.Task;
        }

        public bool IsLoading => _isLoading;
        public IReadOnlyList<LocalEvent> AllEvents => _allEvents.AsReadOnly();

        public async Task GetEventsAsync(Action<LocalEvent> onEventProcessed, int batchSize = 12)
        {
            if (_isLoading) return;
            _isLoading = true;

            try
            {
                if (_allEvents.Count == 0)
                {
                    var html = await _httpClient.GetStringAsync(BASE_URL);
                    var htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(html);

                    var eventNodes = htmlDocument.DocumentNode.SelectNodes("//article[contains(@class, 'mec-event-article')]");

                    if (eventNodes != null)
                    {
                        int count = 0;
                        foreach (var eventNode in eventNodes)
                        {
                            try
                            {
                                var titleNode = eventNode.SelectSingleNode(".//h4[@class='mec-event-title']/a");
                                var dateNode = eventNode.SelectSingleNode(".//span[@class='mec-event-date']");
                                var eventUrl = titleNode?.GetAttributeValue("href", "");
                                var categoryNode = eventNode.SelectSingleNode(".//span[@class='mec-category']");
                                var imageNode = eventNode.SelectSingleNode(".//img[contains(@class, 'attachment-full') and contains(@class, 'size-full')]");

                                if (!string.IsNullOrEmpty(eventUrl))
                                {
                                    var title = titleNode?.InnerText.Trim() ?? "No Title";
                                    title = title.Replace("&#038;", "&")
                                        .Replace("&#8217;", "'");
                                    var eventDate = dateNode?.InnerText.Trim() ?? DateTime.Now.ToString("MMMM d, yyyy");
                                    var imageUrl = imageNode?.GetAttributeValue("data-lazy-src", "") ??
                                                   imageNode?.GetAttributeValue("src", "") ??
                                                   imageNode?.GetAttributeValue("data-src", "");
                                    var category = categoryNode?.InnerText.Trim() ?? "Uncategorized";

                                    if (string.IsNullOrEmpty(imageUrl) || imageUrl.StartsWith("data:image/svg+xml,"))
                                    {
                                        imageUrl = "pack://application:,,,/Resources/placeholderImage.png";
                                    }
                                    else if (!imageUrl.StartsWith("http") && !imageUrl.StartsWith("pack://"))
                                    {
                                        imageUrl = new Uri(new Uri(BASE_URL), imageUrl).ToString();
                                    }

                                    var description = await GetEventDescriptionAsync(eventUrl);
                                    if (description != null)
                                    {
                                        var localEvent = new LocalEvent
                                        {
                                            Title = title,
                                            Date = ParseDate(eventDate),
                                            Description = description,
                                            Category = category,
                                            ImageUrl = imageUrl,
                                            Url = eventUrl
                                        };
                                        _allEvents.Add(localEvent);
                                        onEventProcessed?.Invoke(localEvent);
                                        count++;

                                        if (count % batchSize == 0)
                                        {
                                            await Task.Delay(10); // Allow UI to update
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error processing event: {ex.Message}");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("No events found on the page.");
                    }
                }
                else if (onEventProcessed != null)
                {
                    foreach (var evt in _allEvents)
                    {
                        onEventProcessed(evt);
                    }
                }
                _loadingCompletionSource.TrySetResult(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching events: {ex.Message}");
            }

            finally
            {
                _isLoading = false;
            }
        }


        private async Task<String> GetEventDescriptionAsync(string eventUrl)
        {
            try
            {
                var html = await _httpClient.GetStringAsync(eventUrl);
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);

                var descriptionNode = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='mec-single-event-description mec-events-content']") ??
                                      htmlDocument.DocumentNode.SelectSingleNode("//div[contains(@class, 'mec-events-content')]");

                if (descriptionNode != null)
                {
                    var paragraphs = descriptionNode.SelectNodes(".//p");
                    if (paragraphs != null && paragraphs.Any())
                    {
                        var descriptionParts = paragraphs.Select(p => p.InnerText.Trim()).Where(text => !string.IsNullOrWhiteSpace(text));
                        var description = string.Join("\n\n", descriptionParts);

                        description = description.Replace("&#8217;", "'")
                            .Replace("&#8230;", "...")
                            .Replace("&nbsp;", " ")
                            .Replace("&amp;", "&");

                        return description.Trim();
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching event details for {eventUrl}: {ex.Message}");
                return null;
            }
        }

        private string DetermineCategory(string title, string description)
        {
            var combinedText = (title + " " + description).ToLower();
            if (combinedText.Contains("sport") || combinedText.Contains("cricket") || combinedText.Contains("rugby") || combinedText.Contains("football"))
            {
                return "Sport";
            }
            if (combinedText.Contains("music") || combinedText.Contains("concert") || combinedText.Contains("festival"))
            {
                return "Music";
            }
            if (combinedText.Contains("art") || combinedText.Contains("exhibition") || combinedText.Contains("gallery"))
            {
                return "Art";
            }
            if (combinedText.Contains("food") || combinedText.Contains("culinary") || combinedText.Contains("wine"))
            {
                return "Food & Drink";
            }
            if (combinedText.Contains("tech") || combinedText.Contains("technology") || combinedText.Contains("conference"))
            {
                return "Technology";
            }
            return "Uncategorized";
        }

        private DateTime ParseDate(string dateString)
        {
            if (DateTime.TryParse(dateString, out DateTime result))
            {
                return result;
            }

            // Try parsing custom formats
            string[] formats = { "MMMM d, yyyy", "dd/MM/yyyy", "yyyy-MM-dd" };
            if (DateTime.TryParseExact(dateString, formats, null, System.Globalization.DateTimeStyles.None, out result))
            {
                return result;
            }

            // If all parsing attempts fail, return the current date
            return DateTime.Now;
        }
    }
}