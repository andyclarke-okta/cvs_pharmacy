using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

namespace cvs_SCIM20.Okta.SCIM.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SCIMException : Exception
    {
        [JsonConstructor]
        public SCIMException() : base() { }
        public SCIMException(string message) : base(message) { }
        public SCIMException(string message, Exception exception) : base(message, exception) { }
        public SCIMException(SCIMException scimException)
        {
            this.ErrorCode = scimException.ErrorCode;
            this.ErrorMessage = scimException.ErrorMessage;
            this.ErrorSummary = scimException.ErrorSummary;
            this.ErrorLink = scimException.ErrorLink;
            this.ErrorId = scimException.ErrorId;
            this.ErrorCauses = scimException.ErrorCauses;
            this.HttpStatusCode = scimException.HttpStatusCode;
        }

        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }

        [JsonProperty("errorSummary")]
        public string ErrorSummary { get; set; }

        [JsonProperty("errorLink")]
        public string ErrorLink { get; set; }

        [JsonProperty("errorId")]
        public string ErrorId { get; set; }

        [JsonProperty("errorCauses")]
        public ErrorCause[] ErrorCauses { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
    }


    /// <summary>
    /// Further explanation for why an <see cref="OktaException"/> occurred
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ErrorCause
    {
        [JsonProperty("errorSummary")]
        public string ErrorSummary { get; set; }
    }
}
