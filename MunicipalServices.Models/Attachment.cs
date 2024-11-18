using System;

namespace MunicipalServices.Models
{
    public class Attachment
    {
        public string Name { get; set; }
        public string ContentBase64 { get; set; }
        public string RequestId { get; set; }

        public static string ConvertToBase64(byte[] content)
        {
            return Convert.ToBase64String(content);
        }

        public static byte[] ConvertFromBase64(string base64)
        {
            return Convert.FromBase64String(base64);
        }
    }
}
