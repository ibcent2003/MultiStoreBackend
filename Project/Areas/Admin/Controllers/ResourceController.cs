using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Areas.Admin.Models;
using Project.Properties;
using System.ComponentModel;
using Project.Models;
using System.Data;
using MvcInstaller;
using System.Data.Entity;
using Project.DAL;

namespace Project.Areas.Admin.Controllers
{
    [Authorize]
    public class ResourceController : Controller
    {
        //
        // GET: /Admin/Resource/
        private PROEntities db = new PROEntities();

        [ChildActionOnly]
        public PartialViewResult Nav()
        {
            try
            {
                List<string> roleN = System.Web.Security.Roles.GetRolesForUser(User.Identity.Name).ToList();
                List<Guid> role = db.Roles.Where(x => roleN.Contains(x.RoleName)).Select(x => x.RoleId).ToList();

                ResourceViewModel viewModel = new ResourceViewModel
                {
                    Rows = (from r in db.Resource
                            join a in db.ResourcesInRole on r.Id equals a.ResourceId
                            where role.Contains(a.RoleId) && (r.EndDate == null) || (DateTime.Now < r.EndDate)
                            select r).Distinct().OrderBy(x => x.Order).ToList()
                };


                return PartialView(viewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }
        }

        public ActionResult BulkAction(string approvalaction, string[] items)
        {
            return RedirectToAction("index");
        }

        [Authorize(Roles ="Administrator")]
        public ActionResult Index([DefaultValue(1)] int page, string keywords, [DefaultValue(12)] int pgsize)
        {

            List<Resource> rowsToShow = new List<Resource>();
            int totalRecords = 0;

            if (keywords != null)
            {
                rowsToShow = db.Resource.Where(s => s.Name.Contains(keywords)).OrderBy(x => x.Order).Skip((page - 1) * pgsize).Take(pgsize).ToList();
                totalRecords = db.Resource.Where(s => s.Name.Contains(keywords)).Count();
            }
            else
            {
                rowsToShow = db.Resource.OrderBy(x => x.Order).Skip((page - 1) * pgsize).Take(pgsize).ToList();
                totalRecords = db.Resource.Count();
            }
            var viewModel = new ResourceViewModel
            {
                Rows = rowsToShow,
                PagingInfo = new PagingInfo
                {
                    FirstItem = ((page - 1) * pgsize) + 1,
                    LastItem = page * pgsize,
                    CurrentPage = page,
                    ItemsPerPage = pgsize,
                    TotalItems = totalRecords
                },
                CurrentKeywords = keywords,
                PageSize = pgsize

            };

            return View(viewModel);
        }

        private List<SelectListItem> MakeList()
        {

            List<SelectListItem> _List = new List<SelectListItem>();
            SelectListItem _mList = new SelectListItem();
            _mList = new SelectListItem() { Text = "Top Resource", Value = "0" };
            _List.Add(_mList);
            foreach (var item in db.Resource.Where(x => x.ParentId == 0).ToList())
            {

                _mList = new SelectListItem() { Text = item.Name, Value = item.Id.ToString() };
                _List.Add(_mList);
            }
            return _List;
        }


        public ActionResult Create()
        {

            ResourceEditModel viewModel = new ResourceEditModel
            {
                Row = new Resource(),
                Resources = MakeList()
            };
            return View("Edit", viewModel);
        }

        public ActionResult Edit(int Id)
        {
            try
            {

                ResourceEditModel viewModel = new ResourceEditModel
                {
                    Row = db.Resource.First(x => x.Id == Id),
                    Resources = MakeList()
                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                    TempData["messageType"] = "danger";
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
          
        }

        [HttpPost]
        public ActionResult Edit(Resource row)
        {

            if (ModelState.IsValid)
            {
                if (Save(row))
                {
                    TempData["message"] = row.Name + " has been saved.";
                    TempData["messageType"] = "success";
                }
                return RedirectToAction("Index");
            }
            else // Validation error, so redisplay same view
                return View(row);
        }

        private bool Save(Resource row)
        {

            try
            {
                row.ModifiedDate = DateTime.Now;
                row.ModifiedBy = User.Identity.Name;

                //If it's a new record, just attach it to the DataContext
                if (row.Id == 0) db.Resource.AddObject(row);
                else
                {
                    
                    db.Resource.Attach(row);
                  // db.AddObject(row).State = EntityState.Modified;                    

                }
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                return false;
            }
        }


        private bool DoGrant(string roleId, string[] resourceId)
        {

            //var rows = db.ResourcesInRoles.Where(x => x.RoleId == Guid.Parse(roleId));
            //db.ResourcesInRoles.DeleteAllOnSubmit(rows);
            //db.SubmitChanges();

            foreach (var item in resourceId)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    var guidRoleId = Guid.Parse(roleId);
                    var intItem = int.Parse(item);
                    var row = (from x in db.ResourcesInRole where x.RoleId == guidRoleId && x.ResourceId == intItem select x);
                    if (row.Count() > 0)
                    {                     
                    }
                    else
                    {
                        ResourcesInRole newrow = new ResourcesInRole
                        {
                            RoleId = guidRoleId,
                            ResourceId = int.Parse(item)
                        };
                        db.ResourcesInRole.AddObject(newrow);
                    }

                }
            }
            db.SaveChanges();
            return true;
        }

        private bool DoRevoke(string roleId, string[] resourceId)
        {

            foreach (var item in resourceId)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    var guidRoleId = Guid.Parse(roleId);
                    var intItem = int.Parse(item);
                    ResourcesInRole rows = db.ResourcesInRole.Where(x => x.RoleId == guidRoleId && x.ResourceId == intItem).FirstOrDefault();
                    db.ResourcesInRole.DeleteObject(rows);
                }
            }
            db.SaveChanges();
            return true;
        }

        #region Grant Role Resource

        /// <summary>
        /// Return two lists:
        ///   1)  a list of Resource not granted to the role
        ///   2)  a list of Resource granted to the role
        /// </summary>
        /// <param name="roleid"></param>
        /// <returns></returns>
        [HttpGet]
        public virtual ActionResult ResourceToRole(string Id)
        {

            GrantResourceToRoleViewModel model = new GrantResourceToRoleViewModel();
            model.Id = Id;
            model.AllRoles = (from a in db.Roles orderby a.RoleName select a).AsEnumerable().Select(x => new SelectListItem() { Value = x.RoleId.ToString(), Text = x.RoleName }).ToList();
            if (Id == null || Id == "")
            {
                model.AvailableResources = (from r in db.Resource
                                            orderby r.Name
                                            select r).AsEnumerable().Select(r => new SelectListItem() { Value = r.Id.ToString(), Text = r.Name }).ToList();

                model.GrantedResources = new List<SelectListItem>();
            }
            else
            {
                var me = Guid.Parse(Id);
                var innerquery = (from rr in db.ResourcesInRole where rr.RoleId == me select rr.ResourceId).ToList();
                var resource = (from r in db.Resource select r.Id).ToList().Except(innerquery);
                model.AvailableResources = (from r in db.Resource
                                            orderby r.Name
                                            where resource.Contains(r.Id)
                                            select r).AsEnumerable().Select(r => new SelectListItem() { Value = r.Id.ToString(), Text = r.Name }).ToList();
                model.GrantedResources = (from r in db.Resource
                                          join a in db.ResourcesInRole on r.Id equals a.ResourceId
                                          orderby r.Name
                                          where a.RoleId == me
                                          select r).AsEnumerable().Select(r => new SelectListItem() { Value = r.Id.ToString(), Text = r.Name }).ToList();
            }
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="Resource"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual ActionResult GrantResourceToRole(string roleId, string Resource)
        {

            JsonResponse response = new JsonResponse();
            var guidRoleId = Guid.Parse(roleId);
            var roleName = (from r in db.Roles where r.RoleId == guidRoleId select r.RoleName).FirstOrDefault();

            if (string.IsNullOrEmpty(roleId))
            {
                response.Success = false;
                response.Message = "The role is missing.";
                return Json(response);
            }

            string[] resourceNames = Resource.Substring(0, Resource.Length - 1).Split(',');

            if (resourceNames.Length == 0)
            {
                response.Success = false;
                response.Message = "No resource have been granted to the role.";
                return Json(response);
            }

            try
            {

                response.Success = DoGrant(roleId, resourceNames);
                response.Message = "The Resource(s) has been GRANTED successfully to " + roleName;
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "There was a problem adding the role to the Resource.";
            }

            return Json(response);
        }

        /// <summary>
        /// Revoke the selected roles for the user.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="roleNames"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RevokeResourceForRole(string roleId, string Resource)
        {

            JsonResponse response = new JsonResponse();
            var guidRoleId = Guid.Parse(roleId);
            var roleName = (from r in db.Roles where r.RoleId == guidRoleId select r.RoleName).FirstOrDefault();
            if (string.IsNullOrEmpty(roleId))
            {
                response.Success = false;
                response.Message = "The role is missing.";
                return Json(response);
            }

            if (string.IsNullOrEmpty(Resource))
            {
                response.Success = false;
                response.Message = "Resource is missing";
                return Json(response);
            }

            string[] resourceId = Resource.Substring(0, Resource.Length - 1).Split(',');

            if (resourceId.Length == 0)
            {
                response.Success = false;
                response.Message = "No Resource are selected to be revoked.";
                return Json(response);
            }

            try
            {
                response.Success = DoRevoke(roleId, resourceId);
                response.Message = "The Resource(s) has been REVOKED successfully for " + roleName;
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "There was a problem revoking resource for the role.";
            }

            return Json(response);
        }

        #endregion

    }
}
