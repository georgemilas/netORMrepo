using System;
namespace EM.Util.TickerService
{
    public interface IProcessingStrategy
    {
        bool CanProcess { get; }
        double TimerTickInterval { get; }
        void SetProcessingFlag();
    }
}
