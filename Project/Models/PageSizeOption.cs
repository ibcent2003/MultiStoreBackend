using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class PageSizeOption
    {
        public static IEnumerable<DropOptionItem> Limit = new List<DropOptionItem> { 
    new DropOptionItem {
        Value = 12,
       Label = "12"
    },
    new DropOptionItem {
        Value = 48,
        Label = "48"
    },
    new DropOptionItem {
        Value = 96,
        Label = "96"
    }
};
    }
}