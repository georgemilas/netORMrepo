using System;
using System.Collections.Generic;
using System.Text;
using EM.Util;
using System.Windows.Forms;

namespace DeploymentTools
{
    public interface IDeployToolsPluginHost: IPluginHost, IContainerControl 
    {
        TabControl mainSelector { get; }
        PluginLoader pluginLoader { get; }
        ConfigManager cm { get; }
        void repaint();
    }
}
