using Project.Areas.Admin.Models;
using Project.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Project.Models
{
    public class Backbone
    {
        private PROEntities db = new PROEntities();


        public static List<Store> GetStoreList(PROEntities db)
        {            
            List<Store> store = db.Store.OrderBy(x=>x.Name).ToList();
            return store;
        }

        public static Store GetStore(PROEntities db, Guid Id)
        {
            var store = db.Store.Where(x => x.ProcessInstaceId == Id).FirstOrDefault();
            return store;

        }

        public static Users StoreUser (PROEntities db, Guid UserId)
        {
            var getUser = db.Users.Where(x => x.UserId == UserId).FirstOrDefault();
            return getUser;
        }

        public static List<DAL.Roles> GetAllRoles(PROEntities db)
        {
            List<DAL.Roles> role = db.Roles.OrderBy(x => x.RoleName).ToList();
            return role;
        }

        public static List<ContactInfo> GetStoreContactInfo(PROEntities db, Guid Id)
        {
            var getstore = db.Store.Where(x => x.ProcessInstaceId == Id).FirstOrDefault();
            var getContact = getstore.ContactInfo.ToList();
            return getContact;
        }

        public static List<AddressBook> GetStorAddress(PROEntities db, Guid Id)
        {
            var getstore = db.Store.Where(x => x.ProcessInstaceId == Id).FirstOrDefault();
            var getaddress = getstore.AddressBook.ToList();
            return getaddress;
        }

        public static List<Store> GetNewStoreRegistration(PROEntities db, string[] roles, int workflowId)
        {
            string username = Membership.GetUser().UserName;
            List<Store> applications = db.Store.Where(x => x.WorkFlowId == Properties.Settings.Default.StoreRegistrationWorkFlowId && x.Status == "Registration Verification" && (roles.Contains("Administrator") || x.OwnedBy == "Administrator")).ToList();
            return applications;
        }

        public static List<Store> GetApprovedRegistration(PROEntities db, string[] roles, int workflowId)
        {
            string username = Membership.GetUser().UserName;
            List<Store> applications = db.Store.Where(x => x.WorkFlowId == Properties.Settings.Default.StoreRegistrationWorkFlowId && x.Status == "Approved").ToList();
            return applications;
        }

        public static List<Store> GetRejectedRegistration(PROEntities db, string[] roles, int workflowId)
        {
            string username = Membership.GetUser().UserName;
            List<Store> applications = db.Store.Where(x => x.WorkFlowId == Properties.Settings.Default.StoreRegistrationWorkFlowId && x.Status == "Registration Rejected").ToList();
            return applications;
        }


        public static List<Store> GetPendingRegistration(PROEntities db, string[] roles, int workflowId)
        {
            string username = Membership.GetUser().UserName;
            List<Store> applications = db.Store.Where(x => x.WorkFlowId == Properties.Settings.Default.StoreRegistrationWorkFlowId && x.Status == "draft").ToList();
            return applications;
        }


        public static List<Store> GetOwnStoreRegistration(PROEntities db, string[] roles, int workflowId)
        {
            string username = Membership.GetUser().UserName;
            List<Store> applications = db.Store.Where(x => x.WorkFlowId == Properties.Settings.Default.StoreRegistrationWorkFlowId && (roles.Contains(x.OwnedBy) || x.OwnedBy == username)).ToList();
            return applications;
        }

        public static List<SelectListItem> GetWorkflowStepActionForStoreRegistration(PROEntities db, Guid Id)
        {
          
            List<SelectListItem> list = new List<SelectListItem>();
            Store StoreApplication = db.Store.Where(x => x.ProcessInstaceId == Id).FirstOrDefault();
            var GetworkflowStepId = StoreApplication.WorkflowSteps.FirstOrDefault();          
            WorkflowSteps step = StoreApplication.WorkflowSteps.Where(x => x.Id == GetworkflowStepId.Id).FirstOrDefault();           
            if (null == step)
                return list;
            SelectListItem s = null;
            foreach (var a in step.WorkflowStepActions)
            {
                s = new SelectListItem();
                s.Text = a.DisplayName;
                s.Value = a.ActionId.ToString();
                list.Add(s);
            }
            return list;
        }

        public static List<SelectListItem> GetOtherStepsForWorkflowRegistration(PROEntities db, Guid Id)
        {           
            List<SelectListItem> list = new List<SelectListItem>();
            Store storeApplication = db.Store.Where(x => x.ProcessInstaceId == Id).FirstOrDefault();
            WorkflowSteps step = storeApplication.WorkflowSteps.FirstOrDefault();

            Workflow workflow = step.Workflow;
            List<WorkflowSteps> steps = workflow.WorkflowSteps.OrderBy(x => x.Priority).ToList();

            foreach (var s in steps)
            {
                if (!s.Equals(step))
                {
                    SelectListItem i = new SelectListItem
                    {
                        Text = s.Name,
                        Value = s.Id.ToString()
                    };
                    list.Add(i);
                }
            }

            return list;
        }

        public static KeyValuePair<bool, string> ApplyActionStoreApplication(PROEntities db, ApprovalForm form)
        {

            
            var user = Membership.GetUser().UserName;
            var GetUser = db.Users.Where(x => x.UserName == user).FirstOrDefault();
            var GetUserDetails = GetUser.UserDetail.FirstOrDefault();
            string fname = GetUserDetails.FirstName;
            string lname = GetUserDetails.LastName;
            string uname = Membership.GetUser().UserName;
            string space = " ";
            string line = " | ";
            string fullName = fname + space + lname + space + line + uname;

          
            KeyValuePair<bool, string> status = new KeyValuePair<bool, string>(false, "");

            Store store = db.Store.Where(x => x.ProcessInstaceId == form.Id).FirstOrDefault();
            if (null == store)
            {
                status = new KeyValuePair<bool, string>(false, "Invalid store");
                return status;
            }           
            var step = store.WorkflowSteps.Where(x => x.WorkflowId == store.WorkFlowId).FirstOrDefault();
            if (null == step)
            {
                status = new KeyValuePair<bool, string>(false, "store is not associated with a workflow step");
                return status;
            }

            // Ensure that the requested action is configured properly with step and Alert
            WorkflowStepActions stepAction = step.WorkflowStepActions.Where(x => x.ActionId == form.ActionId).FirstOrDefault();
            if (null == stepAction || null == stepAction.Alert)
            {
                status = new KeyValuePair<bool, string>(false, "store current workflow step not properly configured!");
                return status;
            }

            Alert alert = stepAction.Alert;

            // Get the applied action
            WorkFlowActions appliedAction = db.WorkFlowActions.Where(x => x.Id == form.ActionId).FirstOrDefault();
            if (null == appliedAction)
            {
                status = new KeyValuePair<bool, string>(false, "Applied Action does not exist");
                return status;
            }
            WorkflowSteps nextStep = null;
            var timeInstance = DateTime.Now;

            if (appliedAction.IsMovable)
            {
                if (appliedAction.Direction.Equals("Forward"))
                {
                    // For the forward movement, get the next step


                    WorkflowSteps lastStep = GetLastWorkflowStepstoreRegistration(db, store.WorkFlowId);
                    if (step.Id.Equals(lastStep.Id))
                    {
                        status = new KeyValuePair<bool, string>(false, "Action cannot be applied! Application is currently at the last step");
                        return status;
                    }
                    nextStep = GetNextWorkflowStepStoreRegistration(db, step.Id);

                }
                else if (appliedAction.Direction.Equals("Backward"))
                {
                    // For the Backward movement, get the previous step
                    //nextStep = GetPreviousWorkflowStep(db, step.Id);
                    nextStep = GetFirstWorkflowStepStoreRegistration(db, step.WorkflowId);                   
                }
                else if (appliedAction.Direction.Equals("Flexible"))
                {
                    // For the forward movement, get the requested Step
                    //nextStep = db.WorkflowSteps.Where(x => x.Id == form.NextStep).FirstOrDefault();
                    nextStep = GetPreviousWorkflowStepStoreRegistration(db, step.Id);
                }
                else
                {
                    status = new KeyValuePair<bool, string>(false, "Error acquiring the next workflow Step");
                    return status;
                }

                if (null == nextStep)
                {
                    status = new KeyValuePair<bool, string>(false, "Error acquiring the next workflow Step 2");
                    return status;
                }
                //string rolename = "";
                if (appliedAction.Direction.Equals("Backward"))
                {
                    store.OwnedBy = store.Name;
                    store.Status = nextStep.Name;
                }
                else
                {
                    store.OwnedBy = nextStep.RoleName;
                }
                store.ModifiedBy = Membership.GetUser().UserName;
                store.ModifiedDate = timeInstance;
                store.Status = nextStep.Name;

                // Remove the current step
                store.WorkflowSteps.Remove(step);
                // Replace with the next step
                store.WorkflowSteps.Add(nextStep);


                if (nextStep.Id.Equals(GetLastWorkflowStepStoreRegistration(db, store.WorkFlowId).Id)) 
                {
                    var GetOrg = db.Store.Where(x => x.ProcessInstaceId ==store.ProcessInstaceId).FirstOrDefault();
                      
                    if (GetOrg != null)
                    {
                        GetOrg.OwnedBy = store.Name;
                       
                        db.Store.Context.SaveChanges();
                        db.SaveChanges();
                    }
                }
                // Log action here                
                var getstatus = store.WorkflowSteps.FirstOrDefault();
                var GetApplicationStatus = db.WorkflowSteps.Where(x => x.Id == getstatus.Id).FirstOrDefault();
                StoreAction log = new StoreAction
                {
                    StoreId = store.Id,
                    Name = nextStep.Status,
                    Reason = form.Reason,
                    ModifiedBy = fullName,
                    ModifiedDate = timeInstance,                  
                };
                db.StoreAction.AddObject(log);


                // Do messaging here
                String sms = alert.Sms;
                String email = alert.Email;
                email = email.Replace("%Store_Name%", store.Name).Replace("%Reason%", form.Reason);
                sms = sms.Replace("%Store_Name%", store.Name);
                if (nextStep.Id.Equals(GetFirstWorkflowStepStoreRegistration(db, store.WorkFlowId).Id))
                {
                   var getContact = GetLicenseContactInfoLicenseApplication(db, form.Id);
                   SendEmailNotificationToUser(db,alert.SubjectEmail, email.Replace("%First_Name%", getContact.FirstName).Replace("Store_Name", store.Name), getContact.EmailAddress, "no-reply@fortressmall.com.ng",2);
                   SendSMSNotificationToUser(db,alert.SubjectSms, sms.Replace("%First_Name%", getContact.FirstName), getContact.MobileNo, "Fortressmall",1);
                    
                }
                else if (nextStep.Id.Equals(GetLastWorkflowStepLicenseApplication(db, store.WorkFlowId).Id))
                {
                    //send alert that license generated


                    var getContact = GetLicenseContactInfoLicenseApplication(db, form.Id);
                   SendEmailNotificationToUser(db,alert.SubjectEmail, email, getContact.EmailAddress, "no-reply@fortressmall.com.ng", 2);
                   SendSMSNotificationToUser(db, alert.SubjectSms, sms, getContact.MobileNo, "Fortressmall",1);
                }
                //else
                //{

                //   SendEmailNotificationToAdmin(db, alert.SubjectEmail, email.Replace("%Store_Name%", store.Name), nextStep.RoleName);
                //}
            }
            else
            {
                //  var getstatus = License.WorkflowSteps.FirstOrDefault();
                // var GetApplicationStatus = entity.WorkflowSteps.Where(x => x.Id == getstatus.Id).FirstOrDefault();

                // if Action is Not flexible
                // Log action here
                StoreAction log = new StoreAction
                {
                    StoreId = store.Id,
                    Name = nextStep.Status,
                    Reason = form.Reason,
                    ModifiedBy = fullName,
                    ModifiedDate = timeInstance,                   
                };
                db.StoreAction.AddObject(log);

                // Do messaging here
                String sms = alert.Sms;
                String email = alert.Email;
                email = email.Replace("%Store_Name%", store.Name);
                sms = sms.Replace("%Store_Name%", store.Name);
                nextStep = GetPreviousWorkflowStepStoreRegistration(db, step.Id);
 
            }
            db.SaveChanges();
            status = new KeyValuePair<bool, string>(true, "Action applied successfully");
            return status;
        }

        public static WorkflowSteps GetLastWorkflowStepstoreRegistration(PROEntities db, int workflowId)
        {
            WorkflowSteps step = db.WorkflowSteps.Where(x => x.WorkflowId == workflowId).OrderByDescending(x => x.Priority).FirstOrDefault();
            return step;
        }

        
        public static WorkflowSteps GetNextWorkflowStepStoreRegistration(PROEntities db, int stepId)
        {
            WorkflowSteps step = db.WorkflowSteps.Where(x => x.Id == stepId).FirstOrDefault();
            WorkflowSteps nextStep = null;

            Workflow workflow = step.Workflow;
            List<WorkflowSteps> steps = workflow.WorkflowSteps.OrderBy(x => x.Priority).ToList();
            // Loop thru all the workflow and take the one after current step
            bool shouldStop = false;

            foreach (var s in steps)
            {
                if (shouldStop)
                {
                    nextStep = s;
                    break;
                }
                // If condition is true, then break the loop
                if (s.Id == step.Id)
                    shouldStop = true;
            }
            return nextStep;
        }

        public static WorkflowSteps GetFirstWorkflowStepStoreRegistration(PROEntities db, int workflowId)
        {
            WorkflowSteps step = db.WorkflowSteps.Where(x => x.WorkflowId == workflowId).OrderBy(x => x.Priority).FirstOrDefault();
            return step;
        }

        public static WorkflowSteps GetPreviousWorkflowStepStoreRegistration(PROEntities db, int stepId)
        {
            WorkflowSteps step = db.WorkflowSteps.Where(x => x.Id == stepId).FirstOrDefault();
            WorkflowSteps nextStep = null;

            Workflow workflow = step.Workflow;
            List<WorkflowSteps> steps = workflow.WorkflowSteps.OrderBy(x => x.Priority).ToList();
            // Loop thru all the workflow and take the one after current step
            bool shouldStop = false;
            // Reverse the order of the steps
            steps.Reverse();
            foreach (var s in steps)
            {
                if (shouldStop)
                {
                    nextStep = s;
                    break;
                }
                // If condition is true, then break the loop
                if (s.Id == step.Id)
                    shouldStop = true;
            }
            return nextStep;
        }

        public static WorkflowSteps GetLastWorkflowStepStoreRegistration(PROEntities db, int workflowId)
        {
            WorkflowSteps step = db.WorkflowSteps.Where(x => x.WorkflowId == workflowId).OrderByDescending(x => x.Priority).FirstOrDefault();
            return step;
        }

        public static ContactInfo GetLicenseContactInfoLicenseApplication(PROEntities db, Guid Id)
        {
            //Guid processId = new Guid(Id);
            ContactInfo contact = null;                      
            var GetOrganisation = db.Store.Where(x => x.ProcessInstaceId ==Id).FirstOrDefault();
            var contact2 = GetOrganisation.ContactInfo.FirstOrDefault();
            contact = contact2;
            //check later
            return contact;
        }

        public static WorkflowSteps GetLastWorkflowStepLicenseApplication(PROEntities db, int workflowId)
        {
            WorkflowSteps step = db.WorkflowSteps.Where(x => x.WorkflowId == workflowId).OrderByDescending(x => x.Priority).FirstOrDefault();
            return step;
        }

        #region Send Notification to User

        public static void SendEmailNotificationToUser(PROEntities db,string subject, String message, string EmailAddress, string sender, int Id)
        {
            AlertNotification notification = new AlertNotification
            {
                AlertTypeId = 2,
                Subject = subject,
                Message = message,
                Sender = sender,
                Receiver = EmailAddress,
                ModifedBy = "System",
                ModifedDate = DateTime.Now,
                SentDate = DateTime.Now,
                Status = 0
            };
            db.AlertNotification.AddObject(notification);
            db.SaveChanges();
        }
        public static void SendSMSNotificationToUser(PROEntities db,string subject, String message, string PhoneNumber, string sender, int Id)
        {
            AlertNotification notification = new AlertNotification
            {
                AlertTypeId = 1,
                Subject = subject,
                Message = message,
                Sender = sender,
                Receiver = PhoneNumber,
                ModifedBy = "System",
                ModifedDate = DateTime.Now,
                SentDate = DateTime.Now,
                Status = 0
            };

            db.AlertNotification.AddObject(notification);
            db.SaveChanges();
        }

        #endregion

        #region Send Notification to Admin Role

        public void SendEmailNotificationToAdmin(PROEntities db,string subject, String message, string EmailAddress, string sender, int Id)
        {
            AlertNotification notification = new AlertNotification
            {
                AlertTypeId = 2,
                Subject = subject,
                Message = message,
                Sender = sender,
                Receiver = EmailAddress,
                ModifedBy = "System",
                ModifedDate = DateTime.Now,
                SentDate = DateTime.Now,
                Status = 0
            };

             db.AlertNotification.AddObject(notification);
             db.SaveChanges();
        }
        public void SendSMSNotificationToAdmin(PROEntities db,string subject, String message, string PhoneNumber, string sender, int Id)
        {
            AlertNotification notification = new AlertNotification
            {
                AlertTypeId = 1,
                Subject = subject,
                Message = message,
                Sender = sender,
                Receiver = PhoneNumber,
                ModifedBy = "System",
                ModifedDate = DateTime.Now,
                SentDate = DateTime.Now,
                Status = 0
            };

             db.AlertNotification.AddObject(notification);
             db.SaveChanges();
        }

        #endregion





    }
}