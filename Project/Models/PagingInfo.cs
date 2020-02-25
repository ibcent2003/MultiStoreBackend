using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class PagingInfo
    {
        public int FirstItem { get; set; }
        public int LastItem
        {
            get
            {
                int result = (this.CurrentPage * this.ItemsPerPage);
                if (this.TotalItems < result)
                {
                    result = this.TotalItems;
                }
                return result;
            }
            set { }
        }
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages
        {
            get { return (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage); }
        }
    }
}