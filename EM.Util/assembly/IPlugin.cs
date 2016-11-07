using System;
using System.Collections.Generic;
using System.Text;

namespace EM.Util
{
    public interface IPlugin
    {
        void installPlugin(IPluginHost host);
    }
}
