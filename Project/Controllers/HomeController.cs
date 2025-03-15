using EASendMail;
using Project.DAL;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Controllers
{
   
    public class HomeController : Controller
    {
      //  Backbone services = new Backbone();
        public PROEntities db = new PROEntities();
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Error404()
        {
            return View();
        }

        public ActionResult SendEmail()
        {
            HomeViewModel model = new HomeViewModel();
            var getAlert = db.AlertNotification.Where(x => x.AlertTypeId == 2 && x.Status == 0).ToList();
            if (getAlert.Count == 0)
            {
                TempData["Message"] = "Starting Rockteamall Alert's Services at " + DateTime.Now.ToString();
                TempData["Message"] = "Sending Pending Alerts Please wait...";
                TempData["Message"] = "No messages was sent. You have " + getAlert.Count() + " pending";
                return View(model);
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

            TempData["Message"] = "Starting Rockteamall Alert's Services at " + DateTime.Now.ToString();
            TempData["Message"] = "Sending Pending Alerts Please wait...";
            TempData["Message"] = "We have sent " + getAlert.Count() + " pending messages";            
            return View(model);
        }

    }
}
