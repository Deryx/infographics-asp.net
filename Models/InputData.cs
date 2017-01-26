using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace NewClassrooms.Models
{
    public class InputData
    {
        public string JsonInput { get; set; }
        public string UriInput { get; set; }
        public string FileFormat { get; set; }
        public IEnumerable<SelectListItem> FileFormats
        {
            get
            {
                return new List<SelectListItem>()
                {
                    new SelectListItem { Text = "JSON", Value = "JSON" },
                    new SelectListItem { Text = "XML", Value = "XML" },
                    new SelectListItem { Text = "Plain Text", Value = "Plain Text" }
                };
            }
        }

    }
}