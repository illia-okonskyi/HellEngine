using HellEngine.Core.Models;
using HellEngine.Core.Services.Assets;
using HellEngine.Core.Services.Locale;
using HellEngine.Utils.Configuration.ServiceRegistrator;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace HellEngine.Core.Services.Sessions
{
    public interface ISessionManager
    {
        Session GetSession(Guid sessionId);
        void PingSession(Guid sessionId);
    }

    [ApplicationService(
        Service = typeof(ISessionManager),
        Lifetime = ApplicationServiceLifetime.Singletone)]
    public class SessionManager : ISessionManager
    {
        private class SessionDataDto
        {
            public IServiceScope ServiceScope { get; set; }
            public DateTime LastAccessTime { get; set; }
        }

        private static int SessionLifetimeCheckPeriodTimeout = 60000; // 1 min
        private static TimeSpan SessionExpiredTimeout = new TimeSpan(0, 10, 0); // 10 min

        private readonly IServiceProvider serviceProvider;
        private readonly ISessionLifetimeCheckTimer sessionLifetimeCheckTimer;

        private readonly Dictionary<Guid, SessionDataDto> sessions =
            new Dictionary<Guid, SessionDataDto>();
        private readonly object sessionsLockObject = new object();

        public SessionManager(
            IServiceProvider serviceProvider,
            ISessionLifetimeCheckTimer sessionLifetimeCheckTimer)
        {
            this.serviceProvider = serviceProvider;
            this.sessionLifetimeCheckTimer = sessionLifetimeCheckTimer;
            this.sessionLifetimeCheckTimer.Setup(
                SessionLifetimeCheckPeriodTimeout,
                () => CheckSessionsLifetime());
        }

        public Session GetSession(Guid sessionId)
        {
            var serviceScope = GetServiceScope(sessionId);
            var services = serviceScope.ServiceProvider;

            return new Session
            {
                Id = sessionId,
                LocaleManager = services.GetRequiredService<ILocaleManager>(),
                AssetsManager = services.GetRequiredService<IAssetsManager>()
            };
        }

        public void PingSession(Guid sessionId)
        {
            GetServiceScope(sessionId);
        }

        private IServiceScope GetServiceScope(Guid sessionId)
        {
            var now = DateTime.Now;

            lock (sessionsLockObject)
            {
                var data = sessions.ContainsKey(sessionId)
                    ? sessions[sessionId]
                    : CreateSession(sessionId);
                data.LastAccessTime = now;
                return data.ServiceScope;
            }
        }

        private void CheckSessionsLifetime()
        {
            var now = DateTime.Now;

            var sessionsToRemove = new List<Guid>();
            lock (sessionsLockObject)
            {
                foreach (var kvp in sessions)
                {
                    var data = kvp.Value;
                    if (now - data.LastAccessTime >= SessionExpiredTimeout)
                    {
                        sessionsToRemove.Add(kvp.Key);
                    }
                }

                sessionsToRemove.ForEach(sessionId => RemoveSession(sessionId));
            }
        }

        private SessionDataDto CreateSession(Guid sessionId)
        {
            var serviceScope = serviceProvider.CreateScope();
            var data = new SessionDataDto
            {
                ServiceScope = serviceScope
            };
            sessions.Add(sessionId, data);
            var sessionIdProvider = serviceScope.ServiceProvider
                .GetRequiredService<ISessionIdProvider>();
            sessionIdProvider.SetSessionId(sessionId);
            return data;
        }

        private void RemoveSession(Guid sessionId)
        {
            sessions.Remove(sessionId, out var data);
            data.ServiceScope.Dispose();
        }
    }
}
