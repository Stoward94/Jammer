using System.Collections.Generic;
using Newtonsoft.Json;

namespace GamingSessionApp.ViewModels.Shared
{
    public class CaptchaResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("error-codes")]
        public List<string> ErrorCodes { get; set; }
    }
}
