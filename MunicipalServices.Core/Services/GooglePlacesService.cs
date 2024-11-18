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
//-------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Service for interacting with the Google Places API to get location predictions and details
    /// </summary>
    public class GooglePlacesService
    {
//-------------------------------------------------------------------------------------------------------------
        private readonly string apiKey;
        private readonly HttpClient client;

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the GooglePlacesService class
        /// </summary>
        /// <param name="apiKey">The Google Places API key</param>
        /// <exception cref="ArgumentException">Thrown when API key is null or empty</exception>
        public GooglePlacesService(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentException("API key cannot be null or empty. Please check your .env file.");
            }
            this.apiKey = apiKey;
            this.client = new HttpClient();
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets place predictions based on user input text
        /// </summary>
        /// <param name="input">The search text to get predictions for</param>
        /// <returns>A list of place predictions matching the input</returns>
        /// <exception cref="Exception">Thrown when the Places API returns an error</exception>
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

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets detailed information about a place using its ID
        /// </summary>
        /// <param name="placeId">The Google Places ID of the location</param>
        /// <returns>Detailed information about the place</returns>
        /// <exception cref="ArgumentException">Thrown when place ID is null or empty</exception>
        /// <exception cref="Exception">Thrown when the Places API returns an error</exception>
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

//-------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Response model for the Google Places Autocomplete API
    /// </summary>
    public class AutocompleteResponse
    {
//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// The status of the API response
        /// </summary>
        public string Status { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Error message if the API request failed
        /// </summary>
        public string Error_Message { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// List of place predictions returned by the API
        /// </summary>
        public List<PredictionResult> Predictions { get; set; }
    }

//-------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Model representing a single place prediction result
    /// </summary>
    public class PredictionResult
    {
//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// The unique Google Places ID for the predicted place
        /// </summary>
        [JsonProperty("place_id")]
        public string PlaceId { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Human-readable description of the predicted place
        /// </summary>
        public string Description { get; set; }
    }
}
//-----------------------------------------------------END-OF-FILE-----------------------------------------------------//