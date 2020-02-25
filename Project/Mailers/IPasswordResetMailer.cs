using Mvc.Mailer;
using Project.Mailers.Models;

namespace Project.Mailers
{ 
    public interface IPasswordResetMailer
    {
			MvcMailMessage PasswordReset(MailerModel model);
	}
}