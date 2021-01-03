using HellEngine.Utils.Configuration.ServiceRegistrator;
using System;
using System.Collections.Generic;
using System.Text;

namespace HellEngine.Core.Services.Sessions
{
    public interface ISessionIdProvider
    {
        void SetSessionId(Guid sessionId);
        Guid GetSessionId();
    }

    [ApplicationService(
        Service = typeof(ISessionIdProvider),
        Lifetime = ApplicationServiceLifetime.Scoped)]
    public class SessionIdProvider : ISessionIdProvider
    {
        private Guid sessionId = Guid.Empty;

        public void SetSessionId(Guid sessionId)
        {
            this.sessionId = sessionId;
        }

        public Guid GetSessionId()
        {
            return sessionId;
        }
    }
}
