using Nirast.Pcms.Api.Sdk.Logger;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Logger
{
    public class PCMSLogger : IPCMSLogger
    {
        #region Private Members
        Serilog.ILogger _logger = null;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="IRLogger"/> class.
        /// </summary>
        public PCMSLogger()
        {
            _logger = new LoggerConfiguration()
            .ReadFrom.AppSettings()
            .CreateLogger();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Informations the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Info(string message)
        {
            if(ConfigurationManager.AppSettings["WriteLogs"].ToString() == "True")
                _logger.Information(message);
        }

        /// <summary>
        /// Informations the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="propertyValues">The property values.</param>
        public void Info(string message, params object[] propertyValues)
        {
            if(ConfigurationManager.AppSettings["WriteLogs"].ToString() == "True")
                _logger.Information(message, propertyValues);
        }

        /// <summary>
        /// Informations the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        public void Info(Exception exception, string message)
        {
            if(ConfigurationManager.AppSettings["WriteLogs"].ToString() == "True")
                _logger.Information(exception, message);
        }

        /// <summary>
        /// Informations the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        /// <param name="propertyValues">The property values.</param>
        public void Info(Exception exception, string message, params object[] propertyValues)
        {
            if(ConfigurationManager.AppSettings["WriteLogs"].ToString() == "True")
                _logger.Information(exception, message, propertyValues);
        }

        /// <summary>
        /// Debugs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Debug(string message)
        {
            if(ConfigurationManager.AppSettings["WriteLogs"].ToString() == "True")
                _logger.Information(message);
        }

        /// <summary>
        /// Debugs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="propertyValues">The property values.</param>
        public void Debug(string message, params object[] propertyValues)
        {
            if(ConfigurationManager.AppSettings["WriteLogs"].ToString() == "True")
                _logger.Information(message, propertyValues);
        }

        /// <summary>
        /// Debugs the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        public void Debug(Exception exception, string message)
        {
            if(ConfigurationManager.AppSettings["WriteLogs"].ToString() == "True")
                _logger.Information(exception, message);
        }

        /// <summary>
        /// Debugs the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        /// <param name="propertyValues">The property values.</param>
        public void Debug(Exception exception, string message, params object[] propertyValues)
        {
            if(ConfigurationManager.AppSettings["WriteLogs"].ToString() == "True")
                _logger.Information(exception, message, propertyValues);
        }

        /// <summary>
        /// Errors the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Error(string message)
        {
            if(ConfigurationManager.AppSettings["WriteLogs"].ToString() == "True")
                _logger.Error(message);
        }

        /// <summary>
        /// Errors the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="propertyValues">The property values.</param>
        public void Error(string message, params object[] propertyValues)
        {
            if(ConfigurationManager.AppSettings["WriteLogs"].ToString() == "True")
                _logger.Error(message, propertyValues);
        }

        /// <summary>
        /// Errors the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        public void Error(Exception exception, string message)
        {
            if(ConfigurationManager.AppSettings["WriteLogs"].ToString() == "True")
                _logger.Error(exception, message);
        }

        /// <summary>
        /// Errors the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        /// <param name="propertyValues">The property values.</param>
        public void Error(Exception exception, string message, params object[] propertyValues)
        {
            if (ConfigurationManager.AppSettings["WriteLogs"].ToString() == "True")
                _logger.Error(exception, message, propertyValues);
        }

        /// <summary>
        /// Fatals the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Fatal(string message)
        {
            if (ConfigurationManager.AppSettings["WriteLogs"].ToString() == "True")
                _logger.Fatal(message);
        }

        /// <summary>
        /// Fatals the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="propertyValues">The property values.</param>
        public void Fatal(string message, params object[] propertyValues)
        {
            if(ConfigurationManager.AppSettings["WriteLogs"].ToString() == "True")
                _logger.Fatal(message, propertyValues);
        }

        /// <summary>
        /// Fatals the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        public void Fatal(Exception exception, string message)
        {
            if(ConfigurationManager.AppSettings["WriteLogs"].ToString() == "True")
                _logger.Fatal(exception, message);
        }

        /// <summary>
        /// Fatals the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        /// <param name="propertyValues">The property values.</param>
        public void Fatal(Exception exception, string message, params object[] propertyValues)
        {
            if(ConfigurationManager.AppSettings["WriteLogs"].ToString() == "True")
                _logger.Fatal(exception, message, propertyValues);
        }

        /// <summary>
        /// Warnings the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Warning(string message)
        {
            if(ConfigurationManager.AppSettings["WriteLogs"].ToString() == "True")
                _logger.Warning(message);
        }

        /// <summary>
        /// Warnings the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="propertyValues">The property values.</param>
        public void Warning(string message, params object[] propertyValues)
        {
            if(ConfigurationManager.AppSettings["WriteLogs"].ToString() == "True")
                _logger.Warning(message, propertyValues);
        }

        /// <summary>
        /// Warnings the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        public void Warning(Exception exception, string message)
        {
            if(ConfigurationManager.AppSettings["WriteLogs"].ToString() == "True")
                _logger.Warning(exception, message);
        }

        /// <summary>
        /// Warnings the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        /// <param name="propertyValues">The property values.</param>
        public void Warning(Exception exception, string message, params object[] propertyValues)
        {
            if(ConfigurationManager.AppSettings["WriteLogs"].ToString() == "True")
                _logger.Warning(exception, message, propertyValues);
        }
        #endregion
    }
}
