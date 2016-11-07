using System;
using EM.Logging;
using TreeSync;
namespace DeploymentTools
{
    public interface ISourceContainer
    {
        MessageWriter msgWriter { get; set; }
        bool preview { get; set; }
        FileSystemFolderTree source { get; }
    }
}
