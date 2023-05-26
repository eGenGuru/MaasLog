using System.Collections.Generic;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace MaasLog;

public class MaasConfiguration
{
    public LogLevel LogLevel { get; set; }
    public string TopicName;
    public ProducerConfig config;
}