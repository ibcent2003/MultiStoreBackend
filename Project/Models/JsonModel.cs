using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class JsonModel
    {
        public Boolean Status { get; set; }
        public Boolean SubStatus { get; set; }
        public Object Payload { get; set; }
        public string ResponseMsg { get; set; }
    }
}