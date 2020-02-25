using Project.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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


        #region Send Notification to User

        public void SendEmailNotificationToUser(string subject, String message, string EmailAddress, string sender, int Id)
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
        public void SendSMSNotificationToUser(string subject, String message, string PhoneNumber, string sender, int Id)
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

        public void SendEmailNotificationToChamsAdmin(string subject, String message, string EmailAddress, string sender, int Id)
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
        public void SendSMSNotificationToAdmin(string subject, String message, string PhoneNumber, string sender, int Id)
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