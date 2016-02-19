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
    public class EmailLogic : BaseLogic
    {
        private static readonly string FromEmail = "noreply@axelsmash.com";
        private readonly string BaseUrl;

        public EmailLogic()
        {
            var request = HttpContext.Current.Request;
            BaseUrl = string.Format($"{request.Url.Scheme}://{request.Url.Authority}");
        }

        public async Task NewPrivateMessageEmail(List<UserProfile> recipients, string senderName, UserMessage message)
        {
            try
            {
                foreach (var r in recipients)
                {
                    var email = new SendGridMessage
                    {
                        From = new MailAddress(FromEmail, "Axel Smash"),
                        Subject = "New Private Message"
                    };

                    StringBuilder body = new StringBuilder();

                    body.Append($"Hi {r.DisplayName},\n\n");
                    body.Append($"You have recieved a new private message from {senderName}.\n\n");
                    body.Append("Head over to your inbox to see it now.\n\n");
                    body.Append($"{BaseUrl}/messages/inbox \n\n");
                    body.Append("=========================================== \n\n");
                    body.Append("Axel Smash \n");

                    email.Text = body.ToString();
                    email.AddTo(r.User.Email);

                    await SendEmail(email, r.Preferences);

                    return;
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        private async Task SendEmail(SendGridMessage email, UserPreferences pref)
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

        internal async Task SessionInviteEmail(Session session, string creatorName, List<UserProfile> recipients)
        {
            try
            {
                foreach (var r in recipients)
                {
                    var email = new SendGridMessage
                    {
                        From = new MailAddress(FromEmail, "Axel Smash"),
                        Subject = "Session Invitation"
                    };

                    StringBuilder body = new StringBuilder();

                    body.Append($"Hi {r.DisplayName},\n\n");
                    body.Append($"{creatorName} has invited you to join their new session.\n\n");
                    body.Append("Click the link below to see the full details of the session.\n\n");
                    body.Append($"{BaseUrl}/Sessions/Details/{session.Id} \n\n");
                    body.Append("Grab your place before someone else does!\n\n");

                    body.Append("=========================================== \n\n");
                    body.Append("Axel Smash \n");

                    email.Text = body.ToString();
                    email.AddTo(r.User.Email);

                    await SendEmail(email, r.Preferences);
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "Unable to send session invite email for session : " + session.Id);
            }
        }
    }
}
