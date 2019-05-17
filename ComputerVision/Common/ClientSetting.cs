using System;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;

namespace ComputerVision.Common
{
    public static class ClientSetting
    {
        /// <summary>
        /// Azure ApiKey
        /// </summary>
        /// <value>The credentials.</value>
        public static ApiKeyServiceClientCredentials Credentials
        {
            get
            {
                return new ApiKeyServiceClientCredentials("");
            }
        }

        /// <summary>
        /// Azure EndPoint
        /// </summary>
        /// <value>The end point.</value>
        public static string EndPoint
        {
            get
            {
                return "https://eastus.api.cognitive.microsoft.com";
            }
        }
    }
}
