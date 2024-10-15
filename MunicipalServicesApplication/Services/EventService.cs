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
                                var dateNode = eventNode.SelectSingleNode(".//div[contains(@class, 'mec-event-date')]");
                                var eventUrl = titleNode?.GetAttributeValue("href", "");
                                var categoryNode = eventNode.SelectSingleNode(".//span[@class='mec-category']");
                                var imageNode = eventNode.SelectSingleNode(".//img[contains(@class, 'attachment-full') and contains(@class, 'size-full')]");

                                

                                if (!string.IsNullOrEmpty(eventUrl))
                                {
                                    var title = titleNode?.InnerText.Trim() ?? "No Title";
                                    title = title.Replace("&#038;", "&")
                                        .Replace("&#8217;", "'");
                                    var (parsedDate, originalDateString) = await GetEventDateAsync(eventUrl);
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
                                            Date = parsedDate,
                                            DateString = originalDateString, // Store the original date string
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
                            .Replace("&amp;", "&")
                            .Replace("&#8216;", "'")
                            .Replace("&#8211;", "-");

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

        private async Task<(DateTime, string)> GetEventDateAsync(string eventUrl)
        {
            try
            {
                var html = await _httpClient.GetStringAsync(eventUrl);
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);

                var dateNode = htmlDocument.DocumentNode.SelectSingleNode("//div[contains(@class, 'mec-single-event-date')]") ??
                               htmlDocument.DocumentNode.SelectSingleNode("//div[contains(@class, 'mec-event-date')]") ??
                               htmlDocument.DocumentNode.SelectSingleNode("//div[contains(@class, 'mec-start-date-label')]");
                if (dateNode != null)
                {
                    var startDateNode = dateNode.SelectSingleNode(".//span[contains(@class, 'mec-start-date-label')]") ??
                                        dateNode.SelectSingleNode(".//span[contains(@class, 'mec-start-date')]") ??
                                        dateNode;

                    var endDateNode = dateNode.SelectSingleNode(".//span[contains(@class, 'mec-end-date-label')]") ??
                                      dateNode.SelectSingleNode(".//span[contains(@class, 'mec-end-date')]");

                    string dateString = startDateNode?.InnerText.Trim();
                    if (endDateNode != null && !string.IsNullOrWhiteSpace(endDateNode.InnerText))
                    {
                        dateString += " " + endDateNode.InnerText.Trim();
                    }

                    if (!string.IsNullOrEmpty(dateString))
                    {
                        return ParseDate(dateString);
                    }
                }

                var metaDate = htmlDocument.DocumentNode.SelectSingleNode("//meta[@property='event:start_date']/@content");
                if (metaDate != null)
                {
                    var dateString = metaDate.GetAttributeValue("content", "");
                    if (!string.IsNullOrEmpty(dateString))
                    {
                        return ParseDate(dateString);
                    }
                }

                throw new Exception("Date information not found on the event page.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching event date for {eventUrl}: {ex.Message}");
                return (DateTime.Now, "Date not available");
            }
        }

        private (DateTime, string) ParseDate(string dateString)
        {
            dateString = dateString.Trim();
            string originalDateString = dateString;

            string[] formats = {
                "MMM dd yyyy",
                "MMMM dd yyyy",
                "MMM d yyyy",
                "MMMM d yyyy",
                "yyyy-MM-dd",  // ISO 8601 format
                "dd MMMM yyyy",
                "d MMMM yyyy",
                "MMM dd",
                "MMMM dd",
                "MMM d",
                "MMMM d"
            };

            // Handle date ranges with double dash
            dateString = dateString.Replace(" - ", " – ");

            // Handle date ranges
            if (dateString.Contains("–"))
            {
                var parts = dateString.Split('–');
                string startDateString = parts[0].Trim();
                string endDateString = parts[1].Trim();

                // If the year is only present in the end date, add it to the start date
                if (endDateString.Length > 4 && char.IsDigit(endDateString[endDateString.Length - 1]))
                {
                    string year = endDateString.Substring(endDateString.Length - 4);
                    if (!startDateString.EndsWith(year))
                    {
                        startDateString += " " + year;
                    }
                }

                // Try parsing with various formats
                foreach (var format in formats)
                {
                    if (DateTime.TryParseExact(startDateString, format, null, System.Globalization.DateTimeStyles.None, out DateTime startDate))
                    {
                        return (startDate, originalDateString);
                    }
                }
            }

            // Try parsing the full date string with various formats
            foreach (var format in formats)
            {
                if (DateTime.TryParseExact(dateString, format, null, System.Globalization.DateTimeStyles.None, out DateTime result))
                {
                    return (result, originalDateString);
                }
            }

            // If all else fails, try a more lenient parsing
            if (DateTime.TryParse(dateString, out DateTime lenientResult))
            {
                return (lenientResult, originalDateString);
            }

            Console.WriteLine($"Failed to parse date: {dateString}");
            return (DateTime.Now, originalDateString);
        }
    }
}