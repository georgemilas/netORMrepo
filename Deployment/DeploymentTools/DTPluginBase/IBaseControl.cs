using System;
using EM.Logging;
using System.Threading;

namespace DeploymentTools.Controls
{
    public interface IBaseControl
    {
        ConfigManager configManager { get; set; }
        void configManager_OnInitControls(ConfigManager cfg);
        void configManager_OnSave(ConfigManager cfg);
        MessageWriter msgWriter { get; set; }
        void runInThread(ThreadStart func);
    }
}
