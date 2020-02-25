using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using Project.Models;
using Project.Properties;

namespace Project.HtmlHelpers
{
    public static class DateFormatHelpers
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static MvcHtmlString FormatDate(this HtmlHelper html, DateTime datetime)
        {
            string format = string.Empty;
            format = Settings.Default.DateTimeFormat;
            string result = string.Empty;
            result = datetime.ToString(format);
            return MvcHtmlString.Create(result);
        }
        /// <summary>
        /// Applacation Date Format
        /// </summary>
        /// <param name="html"></param>
        /// <param name="datetime"></param>
        /// <param name="rtnoptions"> Date string format 1: DateOnly, 2: TimeOnly </param>
        /// <returns>date format in sting</returns>
        public static MvcHtmlString FormatDate(this HtmlHelper html, DateTime datetime, int? rtnoptions)
        {
            string format = string.Empty;
            format = Settings.Default.DateTimeFormat;
            if (rtnoptions.HasValue)
            {
                if (rtnoptions == 1)
                    format = Settings.Default.DateFormat;
                else if (rtnoptions == 2)
                    format = Settings.Default.TimeFormat;
            }
            string result = string.Empty;
            result = datetime.ToString(format);
            return MvcHtmlString.Create(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="datetime"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static MvcHtmlString FormatDate(this HtmlHelper html, DateTime datetime, string format)
        {
            string result = string.Empty;
            result = datetime.ToString(format);
            return MvcHtmlString.Create(result);
        }

    }
}