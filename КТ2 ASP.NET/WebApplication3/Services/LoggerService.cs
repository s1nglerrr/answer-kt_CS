using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace WebApplication3.Services
{
    public interface ILoggerService
    {
        void Write(string message);
    }

    public class LoggerService : ILoggerService
    {
        private string _logFilePath;
        private StreamWriter _writer;

        public LoggerService(IConfiguration configuration)
        {
            _logFilePath = configuration["Logging:FilePath"] ?? "log.txt";

            string? directory = Path.GetDirectoryName(_logFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            _writer = new StreamWriter(_logFilePath, append: true)
            {
                AutoFlush = true
            };
        }

        public void Write(string message)
        {
            string logStr = $"{message} - {DateTime.Now:yyyy.MM.dd HH:mm:ss}";
            _writer.WriteLine(logStr);
        }
    }
}