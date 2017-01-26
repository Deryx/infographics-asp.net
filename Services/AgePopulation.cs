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
    public static class AgePopulation
    {
        public static int getCount(IEnumerable<JToken> dobs, int lower, int upper)
        {
            Func<string, int> currentAge = (string dob) =>
            {
                var today = DateTime.Now;
                var currentYear = today.Year;

                var bdate = DateTime.Parse(dob);
                var birthYear = bdate.Year;

                return currentYear - birthYear;
            };

            var count = 0;
            foreach(var item in dobs)
            {
                int age = currentAge(item.ToString());
                if(lower != -1 && upper != -1)
                {
                    if (age >= lower && age <= upper)
                    {
                        count++;
                    }
                }
                else
                {
                    if (age > 100)
                    {
                        count++;
                    }
                }
            }

            return count;
        }
    }
}