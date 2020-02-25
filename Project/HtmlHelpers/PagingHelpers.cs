using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using Project.Models;

namespace Project.HtmlHelpers
{
    public static class PagingHelpers
    {
        public static MvcHtmlString PageLinks(this HtmlHelper html, PagingInfo pagingInfo, Func<int, string> pageUrl)
        {
            if (pagingInfo.TotalPages == 1) return null;
            
            StringBuilder result = new StringBuilder();
           
            TagBuilder tagLi = new TagBuilder("li"); // Construct an <a> tag

            TagBuilder tagPrev = new TagBuilder("a"); // Construct an <a> tag
            if (pagingInfo.CurrentPage > 1)
                tagPrev.MergeAttribute("href", pageUrl(pagingInfo.CurrentPage - 1));
            else
            { tagPrev.MergeAttribute("href", "#"); tagPrev.AddCssClass("disable"); }
            tagPrev.InnerHtml = "Prev";
            tagLi.InnerHtml = tagPrev.ToString();

            int startPg = 1;
            if (pagingInfo.CurrentPage >= 5)
                startPg = pagingInfo.CurrentPage;
           
            int abbrTpage = pagingInfo.CurrentPage + 5;
            if (abbrTpage >= pagingInfo.TotalPages)
            {
                abbrTpage = pagingInfo.TotalPages;
                if (pagingInfo.TotalPages>5)
                startPg = pagingInfo.TotalPages - 5;
                
            }
            result.AppendLine(tagLi.ToString());

            TagBuilder tagFirst = new TagBuilder("a");
            tagFirst.MergeAttribute("href", pageUrl(1));
            tagFirst.InnerHtml = "First";
            tagLi.InnerHtml = tagFirst.ToString();
            result.AppendLine(tagLi.ToString());

            for (int i = startPg; i <= abbrTpage; i++)
            {
                TagBuilder tag = new TagBuilder("a"); // Construct an <a> tag
                tag.MergeAttribute("href", pageUrl(i));
                tag.InnerHtml = i.ToString();
                if (i == pagingInfo.CurrentPage)
                    tag.AddCssClass("active");
                tagLi.InnerHtml=tag.ToString();
                result.AppendLine(tagLi.ToString());
                
            }

            TagBuilder tagLast = new TagBuilder("a");
            tagLast.MergeAttribute("href", pageUrl(pagingInfo.TotalPages));
            tagLast.InnerHtml = "Last";
            tagLi.InnerHtml = tagLast.ToString();
            result.AppendLine(tagLi.ToString());


            TagBuilder tagNext = new TagBuilder("a");
            if (pagingInfo.TotalPages != pagingInfo.CurrentPage)
                tagNext.MergeAttribute("href", pageUrl(pagingInfo.CurrentPage + 1));
            else
            { tagNext.MergeAttribute("href", "#"); tagNext.AddCssClass("disable"); }
            tagNext.InnerHtml = "Next";
            tagLi.InnerHtml = tagNext.ToString();
            result.AppendLine(tagLi.ToString());
            
            return MvcHtmlString.Create(result.ToString());
        }
    }
}