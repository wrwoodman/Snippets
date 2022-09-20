using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Xml;
using Argo.ECD.Common.Contracts.Logging;
using Argo.ECD.Common.Logging;
using Argo.ECD.Common.Util;
using NLog;
using PostSharp.Patterns.Diagnostics;
using LogLevel = NLog.LogLevel;

namespace Argo.ECD.Common.Inspectors
{
    [Log(AttributeExclude = true)]
    public class ServiceMessageInspector : IClientMessageInspector
    {
        private readonly IUniqueIdExtractor _uniqueIdExtractor;
        private readonly ILogger _logger = LogManager.GetLogger(Constants.RequestResponseName);
        private readonly string _messageFormat = "Single";
        private readonly List<string> _values = new List<string> { "Single", "Multiple" };

        public ServiceMessageInspector(IUniqueIdExtractor uniqueIdExtractor, string messageFormat)
        {
            _uniqueIdExtractor = uniqueIdExtractor;
            _messageFormat = messageFormat;
            // Make sure we have a valid format indicator.
            if (!_values.Contains(_messageFormat, StringComparer.OrdinalIgnoreCase))
                _messageFormat = "Single";
        }

        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            WriteMessageToLog(ref reply);
        }

        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, IClientChannel channel)
        {
            WriteMessageToLog(ref request);
            return request;
        }

        private void WriteMessageToLog(ref System.ServiceModel.Channels.Message message)
        {
            string messageString;
            if (_messageFormat == Constants.MessageFormatSingle)
                messageString = MessageString(ref message);
            else
                messageString = message.ToString();
            var uniqueId = _uniqueIdExtractor.GetUniqueId();
            LogHelper.Log(LogLevel.Info, _logger, uniqueId, messageString);
        }

        // Found at https://stackoverflow.com/a/10759660/2528666
        /// <summary>
        /// Get the XML of a Message even if it contains an unread Stream as its Body.
        /// <para>message.ToString() would contain "... stream ..." as
        ///       the Body contents.</para>
        /// </summary>
        /// <param name="message">A reference to the <c>Message</c>. </param>
        /// <returns>A string of the XML after the Message has been fully read and parsed.</returns>
        /// <remarks>The Message <paramref cref="message"/> is re-created in its original state.</remarks>
        string MessageString(ref Message message)
        {
            // copy the message into a working buffer.  TODO: can this size be the same as what is sent in?
            MessageBuffer messageBuffer = message.CreateBufferedCopy(int.MaxValue);

            // re-create the original message, because "copy" changes its state.
            message = messageBuffer.CreateMessage();

            Stream memoryStream = new MemoryStream();
            XmlWriter xmlWriter = XmlWriter.Create(memoryStream);
            messageBuffer.CreateMessage().WriteMessage(xmlWriter);
            xmlWriter.Flush();
            memoryStream.Position = 0;

            byte[] bXml = new byte[memoryStream.Length];
            memoryStream.Read(bXml, 0, (int)memoryStream.Length);

            if (bXml[0] != (byte)'<')
            {
                return Encoding.UTF8.GetString(bXml, 3, bXml.Length - 3);
            }
            else
            {
                return Encoding.UTF8.GetString(bXml, 0, bXml.Length);
            }
        }
    }
}
