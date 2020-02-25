using System.Web.Security;
using SecurityGuard.Core.Pagination;
using Project.Models;
using Project.DAL;
using System.ComponentModel.DataAnnotations;
using System;

namespace SecurityGuard.ViewModels
{
    public class ManageUsersViewModel : PageInfoModel
    {
        public MembershipUserCollection Users { get; set; }
        public PaginatedList<MembershipUser> PaginatedUserList { get; set; }
        public string FilterBy { get; set; }
        public string SearchTerm { get; set; }
        //public int PageSize { get; set; }       
        public System.Collections.Generic.List<Users> Rows { get; set; }

      

    }

}
