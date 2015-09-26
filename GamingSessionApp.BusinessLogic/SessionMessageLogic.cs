using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamingSessionApp.DataAccess;
using GamingSessionApp.Models;
using static GamingSessionApp.BusinessLogic.SystemEnums;

namespace GamingSessionApp.BusinessLogic
{
    public class SessionMessageLogic : BaseLogic
    {
        //Session Repository
        private readonly GenericRepository<SessionMessage> _messageRepo;

        public SessionMessageLogic()
        {
            _messageRepo = UoW.Repository<SessionMessage>();
        }

        public async Task<List<SessionMessage>> GetSessionMessages(Guid sessionId)
        {
            var messages = await _messageRepo.Get(x => x.SessionId == sessionId)
                .OrderBy(x => x.MessageNo)
                .ToListAsync();

            return messages;
        }

        public void AddSessionCreatedMessage(Session session)
        {
            SessionMessage message = new SessionMessage
            {
                AuthorId = session.CreatorId,
                MessageNo = 1,
                MessageTypeId = (int)SessionMessageTypeEnum.System
            };

            message.Body = "New session created!";

            session.Messages.Add(message);
        }
    }
}
