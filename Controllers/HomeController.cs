using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Hosting;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NewClassrooms.DAL;
using NewClassrooms.Models;
using NewClassrooms.Services;

namespace NewClassrooms.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View(new InputData());
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult Index(InputData input)
        {
            // Intake form data
            input.JsonInput = Request.Form["JsonInput"];
            input.UriInput = Request.Form["UriInput"];
            if (!String.IsNullOrEmpty(Request.Form["FileFormat"]))
            {
                input.FileFormat = Request.Form["FileFormat"];
            }

            string populationData = "";
            if (!String.IsNullOrEmpty(input.JsonInput))
            {
                populationData = input.JsonInput;
            }
            else if (!String.IsNullOrEmpty(input.UriInput))
            {
                populationData = RetrieveData.getJson(input.UriInput);
            }

            var json = JsonConvert.DeserializeObject(populationData);

            JObject repository = JObject.Parse(json.ToString());

            IEnumerable<JToken> population = repository.SelectTokens("$..results[*]");
            IEnumerable<JToken> females = repository.SelectTokens("$..results[?(@.gender=='female')]");
            IEnumerable<JToken> males = repository.SelectTokens("$..results[?(@.gender=='male')]");

            // Calculate the total population downloaded
            var totalPopulation = (float)PopulationCount.getCount(population);

            
            // Create female to male percentages section
            var numFemales = PopulationCount.getCount(females);
            var numMales = PopulationCount.getCount(males);
            float femalePct = (numFemales / totalPopulation) * 100;
            var malePct = (numMales / totalPopulation) * 100;
            var pctFemaleMaleString = String.Format("{0:0.0}", femalePct) + "% / " + String.Format("{0:0.0}", malePct) + "%";
            

            // Create first name percentages section
            IEnumerable<JToken> firstNames = repository.SelectTokens("$..first");
            var numFirstNamesTop = NamePopulation.getCount(firstNames, "top");
            var numFirstNamesBottom = NamePopulation.getCount(firstNames, "bottom");
            var firstNameTopPct = (numFirstNamesTop / totalPopulation) * 100;
            var firstNamesBottomPct = (numFirstNamesBottom / totalPopulation) * 100;
            var pctFirstNamesString = String.Format("{0:0.0}", firstNameTopPct) + "% / " + String.Format("{0:0.0}", firstNamesBottomPct) + "%";
               
            
            // Create last name percentages section
            IEnumerable<JToken> lastNames = repository.SelectTokens("$..last");
            var numLastNamesTop = NamePopulation.getCount(lastNames, "top");
            var numLastNamesBottom = NamePopulation.getCount(lastNames, "bottom");
            var lastNameTopPct = (numLastNamesTop / totalPopulation) * 100;
            var lastNamesBottomPct = (numLastNamesBottom / totalPopulation) * 100;
            var pctLastNamesString = String.Format("{0:0.0}", lastNameTopPct) + "% / " + String.Format("{0:0.0}", lastNamesBottomPct) + "%";


            // Create percentage of people in each state, up to the top 10 section
            var stateTable = new Dictionary<string, float>();
            IEnumerable<JToken> states = repository.SelectTokens("$..state");
            var stateGroups = states.GroupBy(state => state);
            foreach(var state in stateGroups)
            {
                stateTable.Add((string)state.Key, state.Count());
            }
            var sortedStateTable = stateTable.OrderByDescending(entry => entry.Value).Take(10);
            List<KeyValuePair<string, float>> stateList = sortedStateTable.ToList();
            var slist = "";
            foreach(KeyValuePair<string, float> item in stateList)
            {
                var statePopPct = (item.Value / totalPopulation) * 100;
                slist += item.Key + ":" + String.Format("{0:0.0}", statePopPct) + "%  ";
            }

            
            // Create percentage of females in each state, up to the top 10 section
            var femaleStateTable = new Dictionary<string, float>();
            IEnumerable<JToken> statesFemale = repository.SelectTokens("$..results[?(@.gender=='female')].location['state']");
            var femaleGroups = statesFemale.GroupBy(femState => femState);
            foreach(var femState in femaleGroups)
            {
                femaleStateTable.Add((string)femState.Key, femState.Count());
            }
            var sortedFSTable = femaleStateTable.OrderByDescending(entry1 => entry1.Value).Take(10);
            List<KeyValuePair<string, float>> femStateList = sortedFSTable.ToList();
            var fStateList = "";
            foreach(KeyValuePair<string, float> fitem in femStateList)
            {
                var femStatePopPct = (fitem.Value / totalPopulation) * 100;
                fStateList += fitem.Key + ":" + String.Format("{0:0.0}", femStatePopPct) + "%  ";
            }

            
            // Create percentage of males in each state, up to the top 10 section
            var maleStateTable = new Dictionary<string, float>();
            IEnumerable<JToken> statesMale = repository.SelectTokens("$..results[?(@.gender=='male')].location['state']");
            var maleGroups = statesMale.GroupBy(maleState => maleState);
            foreach (var maleState in maleGroups)
            {
                maleStateTable.Add((string)maleState.Key, maleState.Count());
            }
            var sortedMSTable = maleStateTable.OrderByDescending(entry2 => entry2.Value).Take(10);
            List<KeyValuePair<string, float>> maleStateList = sortedMSTable.ToList();
            var mStateList = "";
            var mcount = 1;
            foreach (KeyValuePair<string, float> mitem in maleStateList)
            {
                var maleStatePopPct = (mitem.Value / totalPopulation) * 100;
                mStateList += mcount + ". " + mitem.Key + ":" + String.Format("{0:0.0}", maleStatePopPct) + "% ";
                mcount++;
            }


            // Create percentage of people in certain age ranges section
            IEnumerable<JToken> dobs = repository.SelectTokens("$..dob");
            var range1 = (AgePopulation.getCount(dobs, 0, 20) / totalPopulation) * 100;
            var range2 = (AgePopulation.getCount(dobs, 21, 40) / totalPopulation) * 100;
            var range3 = (AgePopulation.getCount(dobs, 41, 60) / totalPopulation) * 100;
            var range4 = (AgePopulation.getCount(dobs, 61, 80) / totalPopulation) * 100;
            var range5 = (AgePopulation.getCount(dobs, 81, 100) / totalPopulation) * 100;
            var range6 = (AgePopulation.getCount(dobs, -1, -1) / totalPopulation) * 100;

            // Create output files
            string path = Server.MapPath(@"~/App_Data/");

            // Create data labels
            string label1 = "Percentage female versus male: ";
            string label2 = "Percentage of first names that start with A-M versus N-Z: ";
            string label3 = "Percentage of last names that start with A-M versus N-Z: ";
            string label4 = "Percentage of people in each state, up to the top 10 most populous states: ";
            string label5 = "Percentage of females in each state, up to the top 10 most populous states: ";
            string label6 = "Percentage of males in each state, up to the top 10 most populous states: ";
            string label7 = "Percentage of people in the following age ranges - 0-20, 21-40,41-60, 61-80, 81-100, 100+: ";

            var rangeString = new StringBuilder();
            rangeString.Append(range1.ToString() + ", ");
            rangeString.Append(range2.ToString() + ", ");
            rangeString.Append(range3.ToString() + ", ");
            rangeString.Append(range4.ToString() + ", ");
            rangeString.Append(range5.ToString() + ", ");
            rangeString.Append(range6.ToString() + ", ");

            ViewBag.Response = "";

            // JSON data output file
            if (input.FileFormat.Equals("JSON"))
            {
                var dataList = new List<JsonData>
                {
                new JsonData {DataLabel = label1, Data = pctFemaleMaleString },
                new JsonData {DataLabel = label2, Data = pctFirstNamesString },
                new JsonData {DataLabel = label3, Data = pctLastNamesString },
                new JsonData {DataLabel = label4, Data = slist },
                new JsonData {DataLabel = label5, Data = fStateList },
                new JsonData {DataLabel = label6, Data = mStateList },
                new JsonData {DataLabel = label7, Data = rangeString.ToString() }
                };

                string jsonData = JsonConvert.SerializeObject(dataList);

                // Create file for data storage
                string jsonFile = path + "output.json";

                //  Place data into jsonFile
                using (StreamWriter jsonStream = new StreamWriter(jsonFile, true))
                {
                    jsonStream.WriteLine(jsonData);

                    jsonStream.Close();
                }
                ViewBag.Response += "<p><strong>Here's your file: </strong></p>";
                ViewBag.Response += "<p><a href='~/App_Data/output.json'>JSON File</a></p>";
            }
            else if (input.FileFormat.Equals("XML"))
            { 
                // XML data output file
                string xmlFile = path + "output.xml";

                XmlTextWriter writer = new XmlTextWriter(xmlFile, Encoding.UTF8);
                writer.WriteStartDocument(true);
                writer.Formatting = System.Xml.Formatting.Indented;
                writer.Indentation = 2;
                writer.WriteStartElement("PopulationData");

                writer.WriteStartElement("Data");
                writer.WriteElementString(label1, pctFemaleMaleString);

                writer.WriteStartElement("Data");
                writer.WriteElementString(label2, pctFirstNamesString);

                writer.WriteStartElement("Data");
                writer.WriteElementString(label3, pctLastNamesString);

                writer.WriteStartElement("Data");
                writer.WriteElementString(label4, slist);

                writer.WriteStartElement("Data");
                writer.WriteElementString(label5, fStateList);

                writer.WriteStartElement("Data");
                writer.WriteElementString(label6, mStateList);

                writer.WriteStartElement("Data");
                writer.WriteElementString(label7, rangeString.ToString());

                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();

                ViewBag.Response += "<p><strong>Here's your file: </strong></p>";
                ViewBag.Response += "<p><a href='~/App_Data/output.xml'>XML File</a></p>";
            }
            else if (input.FileFormat.Equals("Plain Text"))
            {
                // Plain text output file
                string textFile = path + "output.txt";

                var textData = new StringBuilder();
                textData.Append(label1 + pctFemaleMaleString + "/n");
                textData.Append(label2 + pctFirstNamesString + "/n");
                textData.Append(label3 + pctLastNamesString + "/n");
                textData.Append(label4 + slist + "/n");
                textData.Append(label5 + fStateList + "/n");
                textData.Append(label6 + mStateList + "/n");
                textData.Append(label7 + rangeString.ToString());

                textData.ToString();

                using (StreamWriter textStream = new StreamWriter(textFile, true))
                {
                    textStream.WriteLine(textData);

                    textStream.Close();
                }

                ViewBag.Response += "<p><strong>Here's your file: </strong></p>";
                ViewBag.Response += "<p><a href='~/App_Data/output.txt'>Text File</a></p>";
            }


            return View(input);
        }
    }
}
