using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class PageInfoModel
    {
        public PagingInfo PagingInfo { get; set; }
        public int PageSize { get; set; }
        public string CurrentKeywords { get; set; }
    }
}