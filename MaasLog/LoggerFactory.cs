using Microsoft.Extensions.Logging;

namespace MaasLog;

public class LoggerFactory: ILoggerFactory
{
    public void Dispose()
    {
        throw new System.NotImplementedException();
    }

    public ILogger CreateLogger(string categoryName)
    {
        throw new System.NotImplementedException();
    }

    public void AddProvider(ILoggerProvider provider)
    {
        throw new System.NotImplementedException();
    }
}