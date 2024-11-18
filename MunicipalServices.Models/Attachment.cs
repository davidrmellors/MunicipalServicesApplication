using System;

namespace MunicipalServices.Models
{
//-------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Represents an attachment with base64-encoded content
    /// </summary>
    public class Attachment
    {
//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the name of the attachment
        /// </summary>
        public string Name { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the base64-encoded content of the attachment
        /// </summary>
        public string ContentBase64 { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the ID of the request this attachment belongs to
        /// </summary>
        public string RequestId { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Converts a byte array to a base64-encoded string
        /// </summary>
        /// <param name="content">The byte array to convert</param>
        /// <returns>The base64-encoded string representation</returns>
        public static string ConvertToBase64(byte[] content)
        {
            return Convert.ToBase64String(content);
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Converts a base64-encoded string back to a byte array
        /// </summary>
        /// <param name="base64">The base64-encoded string to convert</param>
        /// <returns>The decoded byte array</returns>
        public static byte[] ConvertFromBase64(string base64)
        {
            return Convert.FromBase64String(base64);
        }

//-------------------------------------------------------------------------------------------------------------
    }
}
//-----------------------------------------------------END-OF-FILE-----------------------------------------------------//