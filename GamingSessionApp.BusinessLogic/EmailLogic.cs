using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using GamingSessionApp.Models;
using SendGrid;

namespace GamingSessionApp.BusinessLogic
{
    public static class EmailLogic
    {
        private static readonly string FromEmail = "noreply@axelsmash.com";

        public static async Task NewPrivateMessageEmail(List<UserProfile> recipients, string senderName, UserMessage message)
        {
            var request = HttpContext.Current.Request;
            string baseUrl = string.Format($"{request.Url.Scheme}://{request.Url.Authority}");

            foreach (var r in recipients)
            {
                var email = new SendGridMessage
                {
                    From = new MailAddress(FromEmail, "Axel Smash"),
                    Subject = "New Private Message"
                };

                StringBuilder body = new StringBuilder();

                body.Append($"Hi {r.User.UserName}\n\n");
                body.Append($"You have recieved a new private message from {senderName}.\n\n");
                body.Append("Head over to your inbox to see it now.\n\n");
                body.Append($"{baseUrl}/messages/inbox \n\n");
                body.Append("=========================================== \n\n");
                body.Append("Axel Smash \n");

                email.Text = body.ToString();
                email.AddTo(r.User.Email);

                await SendEmail(email, r.Preferences);
            }

        }

        private static async Task SendEmail(SendGridMessage email, UserPreferences pref)
        {
            //Don't send email if the user doesn't want emails
            if (pref.ReceiveEmail == false) return;

            if (email.To.Length == 0) return;
            if (email.From == null) return;
            if (email.Text == null && email.Html == null) return;

            // Create a Web transport, using API Key
            var transportWeb = new Web("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");

            // Send the email.
            await transportWeb.DeliverAsync(email);
        }
    }
}
