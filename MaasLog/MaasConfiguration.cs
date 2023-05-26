using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace MaasLog;

public class MaasConfiguration
{
    public LogLevel LogLevel { get; set; }
    public string BootstrapServers;
    public string TopicName;
}