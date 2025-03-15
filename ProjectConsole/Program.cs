using EASendMail;
using System;
using System.Configuration;
 
using System.Linq;
namespace ProjectConsole
{
    public class Program
    {
       
        public static void Main(string[] args)
        {
            DataDBDataContext db = new DataDBDataContext();
           
            try
            {
                Console.WriteLine("Starting Rockteamall Alert's Services at " + DateTime.Now.ToString());
                ///Send Email//////////////
                Console.WriteLine("Sending Pending Alerts Please wait... ");
                var getAlert = db.AlertNotifications.Where(x => x.AlertTypeId == 2 && x.Status == 0).ToList();
                if(getAlert.Count ==0)
                {
                    Console.WriteLine("No Pending Message.");
                    Console.WriteLine("This window is closing!");
                    System.Threading.Thread.Sleep(3000);
                    Environment.Exit(0);
                }
                Console.WriteLine("We are sending " + getAlert.Count + " Message(s)");
                foreach (var alert in getAlert)
                {
                    SmtpMail oMail = new SmtpMail("TryIt");
                    oMail.From = ConfigurationManager.AppSettings["FromEmail"];
                    oMail.To = alert.Receiver;
                    // Set email subject
                    oMail.Subject = alert.Subject;
                  //  Attachment oAttachment = oMail.AddAttachment(@"h:\root\home\rocktea-001\www\documents\rockteadocuments\logo.png");
                   Attachment oAttachment = oMail.AddAttachment(@"C:\Users\USER\source\repos\MultiStoreBackend\Project\Content\Frontend\light\img\logo.png");
                
                    string contentID = "test001@host";
                    oAttachment.ContentID = contentID;
                    oMail.HtmlBody = alert.Message.Replace("%Image%", "<html><body><img src=\"cid:" + contentID + "\"> </body></html>");     
                    SmtpServer oServer = new SmtpServer(ConfigurationManager.AppSettings["Host"]);
                    oServer.User = ConfigurationManager.AppSettings["Username"];
                    oServer.Password = ConfigurationManager.AppSettings["Password"];
                    oServer.ConnectType = SmtpConnectType.ConnectTryTLS;
                    oServer.Port = int.Parse(ConfigurationManager.AppSettings["Port"]);                  
                    SmtpClient oSmtp = new SmtpClient();
                    oSmtp.SendMail(oServer, oMail);

                    var up = db.AlertNotifications.Where(x => x.Id == alert.Id).FirstOrDefault();
                    up.Status = 1;
                    db.AlertNotifications.Context.SubmitChanges();
                    db.SubmitChanges();
                                 
                }
               
                Console.WriteLine("Messages sent Successfully. This window is closing!");
                System.Threading.Thread.Sleep(3000);


            }
            catch (Exception ep)
            {
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ep.Message);
            }



        }






    }
}
