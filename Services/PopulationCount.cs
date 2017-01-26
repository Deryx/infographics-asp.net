﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NewClassrooms.DAL;
using NewClassrooms.Models;

namespace NewClassrooms.Services
{
    public static class PopulationCount
    {
        public static int getCount(IEnumerable<JToken> pop)
        {
            var count = 0;
            foreach(var item in pop)
            {
                count++;
            }

            return count;
        }
    }
}