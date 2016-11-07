using System;
namespace EM.Logging
{
    public interface ILogLevel
    {
        Level level { get; set; }
        int priority { get; }
        string ToString();
    }
}
