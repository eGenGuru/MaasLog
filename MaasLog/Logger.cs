﻿using Microsoft.Extensions.Logging;
using System;
using Confluent.Kafka;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Net;
namespace MaasLog
{
    public class Logger : ILogger
    {
        private readonly string _name;
        private readonly Func<MaasConfiguration> _getCurrentConfig;
        private readonly IProducer<Null, string> _producer;
        private readonly ConcurrentQueue<LogMessage> _logQueue;
        private readonly Task _loggingTask;
        private readonly TaskCompletionSource<object> _stopSignal;
        public Logger(
            string name,
            Func<MaasConfiguration> getCurrentConfig)
        {
            (_name, _getCurrentConfig) = (name, getCurrentConfig);
            _producer = new ProducerBuilder<Null, string>(getCurrentConfig().config).Build();
            _logQueue = new ConcurrentQueue<LogMessage>();
            _stopSignal = new TaskCompletionSource<object>();
            _loggingTask = Task.Run(LogMessages);

        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (IsEnabled(logLevel))
            {
                var message = new LogMessage(logLevel, $"{formatter(state, exception)}", exception);
                _logQueue.Enqueue(message);
            }
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => default!;

        private async Task LogMessages()
        {
            while (true)
            {
                if (_logQueue.IsEmpty)
                {
                    await Task.Delay(100); // Delay for a short duration if the log queue is empty
                }
                else
                {
                    while (_logQueue.TryDequeue(out var message))
                    {

                        var dr =
                            await _producer.ProduceAsync(
                                _getCurrentConfig().TopicName,
                                new Message<Null, string> { Value = "test" });

                    }
                }

                if (_stopSignal.Task.IsCompleted)
                {
                    break; // Stop logging if the stop signal is set
                }
            }
        }

        public async Task StopLogging()
        {
            _stopSignal.TrySetResult(null);
            await _loggingTask;
        }


        private class LogMessage
        {

            [JsonPropertyName("agent_timestamp")]
            public string AgentTimestamp { get; set; } = DateTime.UtcNow.ToString("u").Replace(" ", "T");
            [JsonPropertyName("agent_hostname")]
            public string AgentHostname { get; set; } = Dns.GetHostName();
            [JsonPropertyName("agent_source")]
            public string AgentSource { get; set; } = "undefined";
            [JsonPropertyName("agent_offset")]
            public string AgentOffset { get; set; } = TimeZoneInfo.Utc.GetUtcOffset(DateTime.Now).ToString();
            [JsonPropertyName("metadata_account_id")]
            public string MetadataAccountId { get; set; } = "";

            public LogLevel LogLevel { get; }
            public string Message { get; }
            public Exception Exception { get; }

            public LogMessage(LogLevel logLevel, string message, Exception exception)
            {
                LogLevel = logLevel;
                Message = message;
                Exception = exception;
            }
        }
    }
}
