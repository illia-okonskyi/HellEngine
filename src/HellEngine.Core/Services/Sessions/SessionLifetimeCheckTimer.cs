using HellEngine.Utils.Configuration.ServiceRegistrator;
using System;
using System.Timers;

namespace HellEngine.Core.Services.Sessions
{
    public interface ISessionLifetimeCheckTimer
    {
        void Setup(int interval, Action onTick);
    }

    [ApplicationService(Service = typeof(ISessionLifetimeCheckTimer))]
    public class SessionLifetimeCheckTimer : ISessionLifetimeCheckTimer, IDisposable
    {
        private Action onTick;
        private Timer timer;

        public void Dispose()
        {
            DisposeTimer();
        }

        public void Setup(int interval, Action onTick)
        {
            DisposeTimer();
            
            if (onTick == null)
            {
                throw new ArgumentNullException(nameof(onTick));
            }

            timer = new Timer(interval);
            timer.Elapsed += OnTimerElapsed;
            this.onTick = onTick;
            timer.Start();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();
            onTick();
            timer.Start();
        }

        private void DisposeTimer()
        {
            if (timer == null)
            {
                return;
            }

            timer.Elapsed -= OnTimerElapsed;
            timer.Dispose();
            timer = null;
        }
    }
}
