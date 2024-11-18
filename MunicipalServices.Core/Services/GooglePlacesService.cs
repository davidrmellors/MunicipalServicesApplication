using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MunicipalServices.Models;
using Newtonsoft.Json;

namespace MunicipalServices.Core.Services
{
    public class GooglePlacesService
    {
        private readonly string apiKey;
        private readonly HttpClient client;

        public GooglePlacesService(string apiKey)
        {
            this.apiKey = apiKey;
            this.client = new HttpClient();
        }

        public async Task<List<PlacePrediction>> GetPlacePredictions(string input)
        {
            try
            {
                var url = $"https://maps.googleapis.com/maps/api/place/autocomplete/json?input={Uri.EscapeDataString(input)}&key={apiKey}&components=country:za";

                var response = await client.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"API Response: {content}");

                var result = JsonConvert.DeserializeObject<AutocompleteResponse>(content);

                if (result.Status != "OK")
                {
                    var errorMessage = !string.IsNullOrEmpty(result.Error_Message) 
                        ? result.Error_Message 
                        : "Unknown Places API error";
                    Debug.WriteLine($"API Error Status: {result.Status} - {errorMessage}");
                    throw new Exception($"Places API error: {result.Status} - {errorMessage}");
                }

                return result.Predictions?.Select(p => new PlacePrediction
                {
                    PlaceId = p.PlaceId,
                    Description = p.Description
                }).ToList() ?? new List<PlacePrediction>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Places API Error: {ex.Message}");
                throw;
            }
        }

        public async Task<PlaceDetails> GetPlaceDetails(string placeId)
        {
            try
            {
                Debug.WriteLine($"GetPlaceDetails called with Place ID: {placeId}");
                if (string.IsNullOrEmpty(placeId))
                {
                    throw new ArgumentException("Place ID cannot be null or empty");
                }
                Debug.WriteLine($"Requesting details for Place ID: {placeId}");
                var url = $"https://maps.googleapis.com/maps/api/place/details/json?place_id={placeId}&key={apiKey}&fields=formatted_address,geometry";

                var response = await client.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"API Response: {content}");

                var result = JsonConvert.DeserializeObject<PlaceDetailsResponse>(content);

                if (result.Status != "OK")
                {
                    var errorMessage = !string.IsNullOrEmpty(result.Error_Message) 
                        ? result.Error_Message 
                        : "Unknown Places API error";
                    Debug.WriteLine($"API Error Status: {result.Status} - {errorMessage}");
                    throw new Exception($"Places API error: {result.Status} - {errorMessage}");
                }

                return new PlaceDetails
                {
                    FormattedAddress = result.Result.FormattedAddress,
                    Latitude = result.Result.Geometry.Location.Lat,
                    Longitude = result.Result.Geometry.Location.Lng
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Places API Error: {ex.Message}");
                throw;
            }
        }
    }

    public class AutocompleteResponse
    {
        public string Status { get; set; }
        public string Error_Message { get; set; }
        public List<PredictionResult> Predictions { get; set; }
    }

    public class PredictionResult
    {
        [JsonProperty("place_id")]
        public string PlaceId { get; set; }
        public string Description { get; set; }
    }
}
