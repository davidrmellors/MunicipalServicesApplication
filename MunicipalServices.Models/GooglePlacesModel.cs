using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalServices.Models
{
//-------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Represents the response from Google Places Autocomplete API
    /// </summary>
    public class AutocompleteResponse
    {
//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the list of place predictions
        /// </summary>
        public List<PredictionResult> Predictions { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the status of the API response
        /// </summary>
        public string Status { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets any error message returned by the API
        /// </summary>
        public string Error_Message { get; set; }
    }

//-------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Represents a single place prediction result from the Autocomplete API
    /// </summary>
    public class PredictionResult
    {
//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the human-readable description of the place
        /// </summary>
        public string Description { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the unique identifier for the place
        /// </summary>
        public string PlaceId { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the list of matched substrings in the description
        /// </summary>
        public List<MatchedSubstring> MatchedSubstrings { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the list of terms in the description
        /// </summary>
        public List<Term> Terms { get; set; }
    }

//-------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Represents a matched substring in the place description
    /// </summary>
    public class MatchedSubstring
    {
//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the length of the matched substring
        /// </summary>
        public int Length { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the starting position of the matched substring
        /// </summary>
        public int Offset { get; set; }
    }

//-------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Represents a term in the place description
    /// </summary>
    public class Term
    {
//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the starting position of the term
        /// </summary>
        public int Offset { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the text value of the term
        /// </summary>
        public string Value { get; set; }
    }

//-------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Represents the response from Google Places Details API
    /// </summary>
    public class PlaceDetailsResponse
    {
//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the detailed place information
        /// </summary>
        public PlaceResult Result { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the status of the API response
        /// </summary>
        public string Status { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets any error message returned by the API
        /// </summary>
        public string Error_Message { get; set; }
    }

//-------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Represents detailed information about a place
    /// </summary>
    public class PlaceResult
    {
//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the formatted address of the place
        /// </summary>
        public string FormattedAddress { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the geometric information of the place
        /// </summary>
        public Geometry Geometry { get; set; }
    }

//-------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Represents the geometric information of a place
    /// </summary>
    public class Geometry
    {
//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the location coordinates
        /// </summary>
        public Location Location { get; set; }
    }

//-------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Represents geographic coordinates
    /// </summary>
    public class Location
    {
//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the latitude coordinate
        /// </summary>
        public double Lat { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the longitude coordinate
        /// </summary>
        public double Lng { get; set; }
    }

//-------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Represents a simplified place prediction result
    /// </summary>
    public class PlacePrediction
    {
//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the unique identifier for the place
        /// </summary>
        public string PlaceId { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the human-readable description of the place
        /// </summary>
        public string Description { get; set; }
    }

//-------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Represents simplified place details
    /// </summary>
    public class PlaceDetails
    {
//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the formatted address of the place
        /// </summary>
        public string FormattedAddress { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the latitude coordinate
        /// </summary>
        public double Latitude { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the longitude coordinate
        /// </summary>
        public double Longitude { get; set; }
    }
//-------------------------------------------------------------------------------------------------------------
}
//-----------------------------------------------------END-OF-FILE-----------------------------------------------------//
