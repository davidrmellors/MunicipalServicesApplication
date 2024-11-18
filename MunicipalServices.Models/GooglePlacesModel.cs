using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalServices.Models
{
    public class AutocompleteResponse
    {
        public List<PredictionResult> Predictions { get; set; }
        public string Status { get; set; }
        public string Error_Message { get; set; }
    }

    public class PredictionResult
    {
        public string Description { get; set; }
        public string PlaceId { get; set; }
        public List<MatchedSubstring> MatchedSubstrings { get; set; }
        public List<Term> Terms { get; set; }
    }

    public class MatchedSubstring
    {
        public int Length { get; set; }
        public int Offset { get; set; }
    }

    public class Term
    {
        public int Offset { get; set; }
        public string Value { get; set; }
    }

    public class PlaceDetailsResponse
    {
        public PlaceResult Result { get; set; }
        public string Status { get; set; }
        public string Error_Message { get; set; }
    }

    public class PlaceResult
    {
        public string FormattedAddress { get; set; }
        public Geometry Geometry { get; set; }
    }

    public class Geometry
    {
        public Location Location { get; set; }
    }

    public class Location
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

    public class PlacePrediction
    {
        public string PlaceId { get; set; }
        public string Description { get; set; }
    }

    public class PlaceDetails
    {
        public string FormattedAddress { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
