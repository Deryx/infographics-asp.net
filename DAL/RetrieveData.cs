using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace NewClassrooms.DAL
{
    public static class RetrieveData
    {
        public static string getJson(string uri)
        {
            string data = "";

            try
            {
                using (var client = new WebClient())
                {
                    var json = client.DownloadString(uri);

                    data = json;
                }
            }
            catch(HttpRequestException e)
            {
                data = "Error: " + e.ToString();
            }

            return data;
        }
    }
}