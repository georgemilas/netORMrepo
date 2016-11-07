using System;
using EM.Logging;

namespace EM.Util.TickerService
{
    public interface ITickService
    {
        string logID { get; }
        IProcessingStrategy ProcessingStrategy { get; }
        ILogger log { get; set; }

        void Pause();
        void Start();
        void Stop();
        void Continue();
        bool EnsureServiceIsRunning();

    }
}
