﻿using log4net;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace GodsUnchained_Companion_App.Utilities.Http
{
    public static class RestClient
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly string apiBasicUri = "https://api.godsunchained.com/v0/";
        private static readonly HttpClient client = new HttpClient();

        public static async Task<string> Get<T>(string url) {
            string callResult = "";
            try {
                if(client.BaseAddress == null) {
                    client.BaseAddress = new Uri(apiBasicUri);
                }
                var result = await client.GetAsync(url);
                result.EnsureSuccessStatusCode();
                callResult = await result.Content.ReadAsStringAsync();
            } catch (Exception e) {
                log.Error(e.Message);
                log.Error(e.StackTrace);
            }
            return callResult;
        }
    }
}
