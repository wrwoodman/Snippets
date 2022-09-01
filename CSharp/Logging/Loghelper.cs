using System;
using System.Collections.Generic;
using NLog;

namespace Company.Solution.Project.Logging
{
    /// <summary>
    /// A helper class to write messages for NLog
    /// </summary>
    public static class LogHelper
    {
        public static void Log(LogLevel level, ILogger logger, string message, Exception exception = null, IDictionary<object, object> properties = null)
        {
            var eventInfo = new LogEventInfo(level, logger.Name, message);
            eventInfo.Exception = exception;
            if (properties != null)
            {
                foreach (var key in properties.Keys)
                {
                    eventInfo.Properties.Add(key, properties[key]);
                }
            }

            logger.Log(eventInfo);

        }
    }
}
