using System;
using System.Drawing;

namespace EM.Logging
{
    public interface IMessageWriter
    {
        Color color { get; set; }
        void restorePreviousColor();

        void Write(string txt, ILogLevel level, params object[] args);
        void Write(string txt, ILogLevel level);
        
        void WriteException(Exception er);
        
        void WriteLine(string txt, ILogLevel level, params object[] args);
        void WriteLine(string txt, ILogLevel level);

        void Write(Color color, string txt, ILogLevel level);
        void Write(Color color, string txt, ILogLevel level, params object[] args);

        void WriteLine(Color color, string txt, ILogLevel level);
        void WriteLine(Color color, string txt, ILogLevel level, params object[] args);
    }
}
