using System;
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
using System.Text.RegularExpressions;

namespace NewClassrooms.Services
{
    public static class NamePopulation
    {
        public static int getCount(IEnumerable<JToken> names, string pos)
        {
            var count = 0;
            foreach(var item in names)
            {
                var firstLetter = item.ToString().Substring(0,1);
                if(pos == "top")
                {
                    if (Regex.IsMatch(firstLetter.ToUpper(), "^[A-M]"))
                    {
                        count++;
                    }
                }
                else
                {
                    if (Regex.IsMatch(firstLetter.ToUpper(), "^[N-Z]"))
                    {
                        count++;
                    }
                }
            }

            return count;
        }
    }
}