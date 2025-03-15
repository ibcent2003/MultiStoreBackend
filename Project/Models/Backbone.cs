using EASendMail;
using Project.Areas.Admin.Models;
using Project.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            List<Store> store = db.Store.OrderBy(x => x.Name).ToList();
            return store;
        }

        public static Store GetStore(PROEntities db, Guid Id)
        {
            var store = db.Store.Where(x => x.ProcessInstaceId == Id).FirstOrDefault();
            return store;

        }

        //public static List<ProductOrder> GetStoreRegisteredCustomers(PROEntities db, int Id)
        //{
        //    var store = db.ProductOrder.Where(x => x.StoreId == Id).Select(x=>x.UserId).Distinct().ToList();
        //    return store;

        //}

        public static StoreProduct GetProduct(PROEntities db, int Id, int PId)
        {
            var store = db.Store.Where(x => x.Id == Id).FirstOrDefault();
            var product = store.StoreProduct.Where(x => x.Id == PId).FirstOrDefault();
            return product;

        }

        public static List<StoreProduct> GetReoderProductLevel(PROEntities db, int Id)
        {
            var store = db.Store.Where(x => x.Id == Id).FirstOrDefault();
            var product = store.StoreProduct.Where(x => x.ReorderLevel >= x.Quantity).ToList();
            return product;

        }

        public static List<ProductOrder> GetStoreRecentOrder(PROEntities db, int Id)
        {
            var store = db.Store.Where(x => x.Id == Id).FirstOrDefault();
            var GetOrder = store.ProductOrder.Where(x => x.ConfirmBy == null && x.CancelledOrder != true && x.OrderStatus != "Payment Cancelled").OrderByDescending(x => x.OrderDate).Take(10).ToList();
            return GetOrder;

        }
        public static List<ProductOrder> GetStoreNewOrder(PROEntities db, int Id)
        {
            var store = db.Store.Where(x => x.Id == Id).FirstOrDefault();
            var GetOrder = store.ProductOrder.Where(x => x.OrderStatus == "Order Placed").ToList();
            return GetOrder;

        }
        public static List<ProductOrder> GetStoreOverallOrder(PROEntities db, int Id)
        {
            var store = db.Store.Where(x => x.Id == Id).FirstOrDefault();
            var GetOrder = store.ProductOrder.ToList();
            return GetOrder;

        }
        public static List<ProductOrder> GetStoreConfirmOrder(PROEntities db, int Id)
        {
            var store = db.Store.Where(x => x.Id == Id).FirstOrDefault();
            var GetOrder = store.ProductOrder.Where(x => x.OrderStatus == "Confirmed").ToList();
            return GetOrder;
        }
        public static List<ProductOrder> GetStoreConfirmPayment(PROEntities db, int Id)
        {
            var store = db.Store.Where(x => x.Id == Id).FirstOrDefault();
            var GetOrder = store.ProductOrder.Where(x => x.OrderStatus == "Paid").ToList();
            return GetOrder;
        }

        public static List<ProductOrder> GetDeliveredProduct(PROEntities db, int Id)
        {
            var store = db.Store.Where(x => x.Id == Id).FirstOrDefault();
            var GetOrder = store.ProductOrder.Where(x => x.OrderStatus == "Delivered").ToList();
            return GetOrder;
        }

        public static List<ProductOrder> GetStoreCancelledOrder(PROEntities db, int Id)
        {
            var store = db.Store.Where(x => x.Id == Id).FirstOrDefault();
            var GetOrder = store.ProductOrder.Where(x => x.OrderStatus == "Cancelled").ToList();
            return GetOrder;

        }

        public static List<ProductOrder> GetStoreCancelledPayment(PROEntities db, int Id)
        {
            var store = db.Store.Where(x => x.Id == Id).FirstOrDefault();
            var GetOrder = store.ProductOrder.Where(x => x.OrderStatus == "Payment Cancelled").ToList();
            return GetOrder;

        }

        public static List<ProductOrder> GetStoreNewOrderList(PROEntities db, string OrderNo)
        {
            // var store = db.Store.Where(x => x.Id == Id).FirstOrDefault();
            var GetOrder = db.ProductOrder.Where(x => x.OrderNo == OrderNo).ToList();
            return GetOrder;

        }

        public static List<StoreSlider> GetStoreSlider(PROEntities db, Guid Id)
        {
            var store = db.Store.Where(x => x.ProcessInstaceId == Id).FirstOrDefault();
            var GetSlider = store.StoreSlider.ToList();
            return GetSlider;

        }

        public static ProductOrder GetOrder(PROEntities db, string OrderNo, int cId)
        {
            var order = db.ProductOrder.Where(x => x.OrderNo == OrderNo && x.CartId == cId).FirstOrDefault();
            return order;

        }

        public static List<CartItem> GetCartItems(PROEntities db, Guid Id, int CartId)
        {
            var store = db.Store.Where(x => x.ProcessInstaceId == Id).FirstOrDefault();
            var Getitems = db.CartItem.Where(x => x.CartId == CartId).ToList();
            return Getitems;

        }

        public static List<StoreImageCollection> GetImageCollection(PROEntities db, Guid Id)
        {
            var store = db.Store.Where(x => x.ProcessInstaceId == Id).FirstOrDefault();
            var GetCollection = store.StoreImageCollection.ToList();
            return GetCollection;

        }

        public static Users StoreUser(PROEntities db, Guid UserId)
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
                    var GetOrg = db.Store.Where(x => x.ProcessInstaceId == store.ProcessInstaceId).FirstOrDefault();

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
                email = email.Replace("%Company_Name%", store.Name).Replace("%Reason%", form.Reason);
                sms = sms.Replace("%Company_Name%", store.Name);
                if (nextStep.Id.Equals(GetFirstWorkflowStepStoreRegistration(db, store.WorkFlowId).Id))
                {
                    var getContact = GetLicenseContactInfoLicenseApplication(db, form.Id);
                    SendEmailNotificationToUser(db, alert.SubjectEmail, email.Replace("%First_Name%", getContact.FirstName).Replace("Company_Name", store.Name), getContact.EmailAddress, Properties.Settings.Default.FromEmail, 2);
                    SendSMSNotificationToUser(db, alert.SubjectSms, sms.Replace("%First_Name%", getContact.FirstName), getContact.MobileNo, "Rockteamall", 1);
                    
                }
                else if (nextStep.Id.Equals(GetLastWorkflowStepLicenseApplication(db, store.WorkFlowId).Id))
                {
                    //send alert that license generated


                    var getContact = GetLicenseContactInfoLicenseApplication(db, form.Id);
                    SendEmailNotificationToUser(db, alert.SubjectEmail, email, getContact.EmailAddress, Properties.Settings.Default.FromEmail, 2);
                    SendSMSNotificationToUser(db, alert.SubjectSms, sms, getContact.MobileNo, "Rockteamall", 1);
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
            var GetOrganisation = db.Store.Where(x => x.ProcessInstaceId == Id).FirstOrDefault();
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

        public static void SendEmailNotificationToUser(PROEntities db, string subject, String message, string EmailAddress, string sender, int Id)
        {
            try
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

                //using (MailMessage mm = new MailMessage(sender, EmailAddress))
                //{
                //    mm.Subject = subject;
                //    mm.Body = message;
                //    mm.IsBodyHtml = true;
                //    SmtpClient smtp = new SmtpClient();
                //    smtp.Host = Properties.Settings.Default.Host;
                //    smtp.EnableSsl = false;
                //    NetworkCredential NetworkCred = new NetworkCredential(Properties.Settings.Default.Username, Properties.Settings.Default.Password);
                //    smtp.UseDefaultCredentials = true;
                //    smtp.Credentials = NetworkCred;
                //    smtp.Port = Properties.Settings.Default.Port;
                //    smtp.Send(mm);
                //    System.Threading.Thread.Sleep(3000);
                //    Environment.Exit(0);
                //}
            }
            catch (Exception exception)
            {
                string innerexception = (exception.InnerException != null) ? exception.InnerException.Message : "";

            }




        }
        public static void SendSMSNotificationToUser(PROEntities db, string subject, String message, string PhoneNumber, string sender, int Id)
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
            // db.SaveChanges();
        }

        #endregion

        #region Send Notification to Admin Role

        public void SendEmailNotificationToAdmin(PROEntities db, string subject, String message, string EmailAddress, string sender, int Id)
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

            //using (MailMessage mm = new MailMessage(sender, EmailAddress))
            //{
            //    mm.Subject = subject;
            //    mm.Body = message;
            //    mm.IsBodyHtml = true;
            //    SmtpClient smtp = new SmtpClient();
            //    smtp.Host = Properties.Settings.Default.Host;
            //    smtp.EnableSsl = false;
            //    NetworkCredential NetworkCred = new NetworkCredential(Properties.Settings.Default.Username, Properties.Settings.Default.Password);
            //    smtp.UseDefaultCredentials = true;
            //    smtp.Credentials = NetworkCred;
            //    smtp.Port = Properties.Settings.Default.Port;
            //    smtp.Send(mm);
            //  // System.Threading.Thread.Sleep(3000);
            //    //Environment.Exit(0);
            //}
        }
        public void SendSMSNotificationToAdmin(PROEntities db, string subject, String message, string PhoneNumber, string sender, int Id)
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
            // db.SaveChanges();
        }

        #endregion


        #region Smtp method
        public void SendEmail(PROEntities db, string email)
        {

            var getAlert = db.AlertNotification.Where(x => x.AlertTypeId == 2 && x.Status == 0 && x.Receiver==email).ToList();
            if (getAlert.Count == 0)
            {
                
            }
            foreach (var alert in getAlert)
            {
                SmtpMail oMail = new SmtpMail("TryIt");
                oMail.From = Properties.Settings.Default.FromEmail;
                oMail.To = alert.Receiver;
                // Set email subject
                oMail.Subject = alert.Subject;
                //  Attachment oAttachment = oMail.AddAttachment(@"h:\root\home\rocktea-001\www\documents\rockteadocuments\logo.png");
                Attachment oAttachment = oMail.AddAttachment(@"C:\Users\USER\source\repos\MultiStoreBackend\Project\Content\Frontend\light\img\logo.png");

                string contentID = "test001@host";
                oAttachment.ContentID = contentID;
                oMail.HtmlBody = alert.Message.Replace("%Image%", "<html><body><img src=\"cid:" + contentID + "\"> </body></html>");
                SmtpServer oServer = new SmtpServer(Properties.Settings.Default.Host);
                oServer.User = Properties.Settings.Default.Username;
                oServer.Password = Properties.Settings.Default.Password;
                oServer.ConnectType = SmtpConnectType.ConnectTryTLS;
                oServer.Port = Properties.Settings.Default.Port;
                SmtpClient oSmtp = new SmtpClient();
                oSmtp.SendMail(oServer, oMail);

                var up = db.AlertNotification.Where(x => x.Id == alert.Id).FirstOrDefault();
                up.Status = 1;
                db.SaveChanges();
                

            }
        }
        #endregion

       

    }
}