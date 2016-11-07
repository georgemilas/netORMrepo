using System;
using EM.Logging;
using System.ServiceModel.Configuration;
using System.Configuration;
using System.ServiceModel;

namespace EM.Util.TickerService
{
    public enum ApplicationStartType { TestConsoleApp, WindowsService }

    public class ServiceStartSettings
    {
        /// <summary>
        /// either "test" or "service"
        /// </summary>
        public ApplicationStartType runType { get; set; }

        /// <summary>
        /// name of windows service
        /// </summary>
        public string serviceName { get; set; }
        public ILogger log { get; set; }

        public Action onStart { get; set; }
        public Action onStop { get; set; }

    }
}