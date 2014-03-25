namespace OTE.eAdapter
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.ServiceModel;
    using System.Text;
    using CargoWise.eHub.Common;
    using CargoWise.eHub.Common.Extensions;
    using OTE.eAdapter.DataTypes;
    using Interchange = OTE.eAdapter.DataTypes.Interchange;
    using Universal = OTE.eAdapter.DataTypes.Universal;
    using System.Diagnostics;

    public class eAdapterStreamedService : IeAdapterStreamedService
    {
        private const string PROCESSED_FOLDER = "Processed";
        private const string FAILED_FOLDER = "Failed";
        private const string SOURCE = "eAdapterOutbound";
        private const string SQL_MISC_ERROR = "A transport-level error has occurred when sending the request to the server";
        private const string SQL_DUPLICATE_ERROR = "Violation of PRIMARY KEY constraint";
        private const string SQL_TIMEOUT = "Timeout expired.";

        private enum ExceptionAction
        {
            Ignore,
            TryAgain,
            Throw
        }

        public bool Ping()
        {
            return true;
        }

        public void SendStream(SendStreamRequest request)
        {
            string senderId = OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name;
            var messageExceptionDictionary = new Dictionary<Guid, Exception>();

            foreach (var message in request.Messages)
            {
                try
                {
                    processMessage(message, senderId);
                }
                catch (Exception ex)
                {
                    // Throw Exception for each message Therefore Enterprise can show the message status correspondingly
                    if (!messageExceptionDictionary.ContainsKey(message.MessageTrackingID))
                    {
                        messageExceptionDictionary.Add(message.MessageTrackingID, ex);
                    }
                }
            }
            if (messageExceptionDictionary.Count > 0)
            {
                CombineExceptionsAndThrow(messageExceptionDictionary);
            }
        }

        private void processMessage(eHubGatewayMessage message, string senderId)
        {
            string fileName = string.Format("Message_{1}_{0}.xml", message.MessageTrackingID, senderId);

            // You can put your codes in here to process the message
            string destinationFolder = @"C:\CW EDI UNIVERSAL-test\"; //Please change;
            
            if (senderId.Equals("OTXOTEPHX", StringComparison.InvariantCultureIgnoreCase))
            {
                destinationFolder = @"C:\CW EDI UNIVERSAL\"; //Please change
            }
            else
            {
                // Save message
                using (var fileStream = new FileStream(Path.Combine(destinationFolder, fileName), FileMode.Create))
                {
                    message.MessageStream.DecodeAndDecompress().WriteTo(fileStream);
                }

                return;
            }

            string messages = string.Empty;

            string filePath = Path.Combine(destinationFolder, fileName);

            FileInfo messageFile = new FileInfo(filePath);

            try
            {
                // Get message
                messages = message.MessageStream.DecodeAndDecompress().ReadToEnd();

                // Save message
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    message.MessageStream.DecodeAndDecompress().WriteTo(fileStream);
                }

                // Get universal type
                eAdapterMessageHeader messageHeader = new eAdapterMessageHeader(messages);

                List<Universal.UniversalShipmentData> universalShipments = null;
                List<Universal.UniversalShipmentData> universalConsolidations = null;
                List<Universal.UniversalEventData> events = null;
                List<Interchange.Shipments> interchangeShipments = null;
                List<Universal.UniversalTransactionData> transactions = null;

                // Unwrap messages
                switch (messageHeader.MessageType)
                {
                    case MessageType.UniversalShipment:
                        universalShipments = eAdapterDataTypesFactory.UnwrapMessage<Universal.UniversalShipmentData>(messages);
                        break;
                    case MessageType.UniversalConsolidation:
                        universalConsolidations = eAdapterDataTypesFactory.UnwrapMessage<Universal.UniversalShipmentData>(messages);
                        break;
                    case MessageType.UniversalEvent:
                        events = eAdapterDataTypesFactory.UnwrapMessage<Universal.UniversalEventData>(messages);
                        break;
                    case MessageType.UniversalTransactionInfo:
                        transactions = eAdapterDataTypesFactory.UnwrapMessage<Universal.UniversalTransactionData>(messages);
                        break;
                    case MessageType.InterchangeShipment:
                        interchangeShipments = eAdapterDataTypesFactory.UnwrapMessage<Interchange.Shipments>(messages);
                        break;
                }

                // List of processors
                List<IeAdapterMessageProcessor> messageProcessors = new List<IeAdapterMessageProcessor>();

                foreach (string code in messageHeader.RecipientID.ToCharArray().Select(c => c.ToString()))
                {
                    switch (code)
                    {
                        //case "P":
                        //    messageProcessors.Add(new SPR.eAdapterAdapter());
                        //    break;
                        //case "A":
                        //    messageProcessors.Add(new AER.eAdapterAdapter());
                        //    break;
                        //case "I":
                        //    messageProcessors.Add(new E210.eAdapterAdapter());
                        //    break;
                        //case "S":
                        //    messageProcessors.Add(new SEDE.eAdapterAdapter());
                        //    break;
                        //default:
                        //    throw new eAdapterHeaderException("Invalid recipient id");
                    }
                }

                // Send the message to each processor
                foreach (IeAdapterMessageProcessor messageProcessor in messageProcessors)
                {
                    switch (messageHeader.MessageType)
                    {
                        case MessageType.UniversalShipment:
                            try
                            {
                                messageProcessor.ProcessMessages(universalShipments);
                            }
                            catch (eAdapterProcessorException ex)
                            {
                                // Verify what action to take
                                ExceptionAction action = VerifyException(ex);

                                if (action == ExceptionAction.Ignore)
                                {
                                }
                                else if (action == ExceptionAction.TryAgain)
                                {
                                    messageProcessor.ProcessMessages(universalShipments);
                                }
                                else
                                {
                                    throw ex;
                                }
                            }
                            break;
                        case MessageType.UniversalConsolidation:
                            try
                            {
                                messageProcessor.ProcessMessagesConsolidations(universalConsolidations);
                            }
                            catch (eAdapterProcessorException ex)
                            {
                                // Verify what action to take
                                ExceptionAction action = VerifyException(ex);

                                if (action == ExceptionAction.Ignore)
                                {
                                }
                                else if (action == ExceptionAction.TryAgain)
                                {
                                    messageProcessor.ProcessMessagesConsolidations(universalConsolidations);
                                }
                                else
                                {
                                    throw ex;
                                }
                            }
                            break;
                        case MessageType.UniversalEvent:
                            try
                            {
                                messageProcessor.ProcessMessages(events);
                            }
                            catch (eAdapterProcessorException ex)
                            {
                                // Verify what action to take
                                ExceptionAction action = VerifyException(ex);

                                if (action == ExceptionAction.Ignore)
                                {
                                }
                                else if (action == ExceptionAction.TryAgain)
                                {
                                    messageProcessor.ProcessMessages(events);
                                }
                                else
                                {
                                    throw ex;
                                }
                            }
                            break;
                        case MessageType.UniversalTransactionInfo:
                            try
                            {
                                messageProcessor.ProcessMessages(transactions);
                            }
                            catch (eAdapterProcessorException ex)
                            {
                                // Verify what action to take
                                ExceptionAction action = VerifyException(ex);

                                if (action == ExceptionAction.Ignore)
                                {
                                }
                                else if (action == ExceptionAction.TryAgain)
                                {
                                    messageProcessor.ProcessMessages(transactions);
                                }
                                else
                                {
                                    throw ex;
                                }
                            }
                            break;
                        case MessageType.InterchangeShipment:
                            try
                            {
                                messageProcessor.ProcessMessages(interchangeShipments);
                            }
                            catch (eAdapterProcessorException ex)
                            {
                                // Verify what action to take
                                ExceptionAction action = VerifyException(ex);

                                if (action == ExceptionAction.Ignore)
                                {
                                }
                                else if (action == ExceptionAction.TryAgain)
                                {
                                    messageProcessor.ProcessMessages(interchangeShipments);
                                }
                                else
                                {
                                    throw ex;
                                }
                            }
                            break;
                    }
                }

                string newPath = Path.Combine(destinationFolder, PROCESSED_FOLDER, messageFile.Name.Replace(".xml", string.Format("_{0}_{1} - {2}.xml", messageHeader.RecipientID, messageHeader.MessageTypeCode, messageHeader.Reference)));

                if (File.Exists(newPath))
                {
                    File.Delete(newPath);
                }

                messageFile.MoveTo(newPath);
            }
            catch (eAdapterProcessorException ex)
            {
                this.LogErrorMoveFile(messageFile, ex.ProcessorName, ex, destinationFolder, fileName);
            }
            catch (eAdapterHeaderException ex)
            {
                this.LogErrorMoveFile(messageFile, "Header", ex, destinationFolder, fileName);
            }
            catch (eAdapterFormatException ex)
            {
                this.LogErrorMoveFile(messageFile, "Format", ex, destinationFolder, fileName);
            }
            catch (Exception ex)
            {
                this.LogErrorMoveFile(messageFile, "Exception", ex, destinationFolder, fileName);
            }
        }

        /// <summary>
        /// Verify what to do with the exception
        /// </summary>
        /// <param name="pException">Exception</param>
        /// <returns>ExceptionAction</returns>
        private static ExceptionAction VerifyException(Exception pException)
        {
            do
            {
                if (pException.Message.StartsWith(SQL_MISC_ERROR))
                {
                    return ExceptionAction.TryAgain;
                }
                else if (pException.Message.StartsWith(SQL_TIMEOUT))
                {
                    System.Threading.Thread.Sleep(60000);
                    return ExceptionAction.TryAgain;
                }
                else if (pException.Message.StartsWith(SQL_DUPLICATE_ERROR))
                {
                    return ExceptionAction.Ignore;
                }

                pException = pException.InnerException;
            }
            while (pException != null);

            return ExceptionAction.Throw;
        }

        /// <summary>
        /// Logs the error and moves the message to the failed folder
        /// </summary>
        /// <param name="pMessageFile">Message file</param>
        /// <param name="pPrefix">Prefix for the new file name</param>
        /// <param name="pException">Exception thrown</param>
        /// <param name="pDestinationFolder">Destination for the new file</param>
        /// <param name="pFileName">Message file name</param>
        private void LogErrorMoveFile(FileInfo pMessageFile, string pPrefix, Exception pException, string pDestinationFolder, string pFileName)
        {
            EventLog.WriteEntry(SOURCE, RetrieveInnerExceptionMessage(pException, pMessageFile.Name), EventLogEntryType.Error);
            pMessageFile.MoveTo(Path.Combine(pDestinationFolder, FAILED_FOLDER, pPrefix + "-" + pFileName));
            File.WriteAllText(Path.Combine(pDestinationFolder, FAILED_FOLDER, pPrefix + "-" + pFileName.Replace(".xml", ".txt")), pException.ToString());
        }

        /// <summary>
        /// Retrieves the inner exceptions as a concatenated string
        /// </summary>
        /// <param name="pException">Exception thrown</param>
        /// <param name="pInitialText">Initial text</param>
        /// <returns>Concatenated exceptions</returns>
        private string RetrieveInnerExceptionMessage(Exception pException, string pInitialText)
        {
            StringBuilder message = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(pInitialText))
            {
                message.Append("(").Append(pInitialText).Append("): ");
            }

            do
            {
                message.Append(pException.Message).Append(": ");

                pException = pException.InnerException;
            }
            while (pException != null);

            return message.ToString();
        }

        private void CombineExceptionsAndThrow(Dictionary<Guid, Exception> messageExceptionDictionary)
        {
            var errorBuilder = new StringBuilder();
            errorBuilder.Append(string.Format("{0} errors occurred during processing send request:{1}", messageExceptionDictionary.Count, System.Environment.NewLine));

            foreach (var error in messageExceptionDictionary.Values)
            {
                errorBuilder.AppendLine(string.Empty);
                errorBuilder.AppendLine(error.ToString());
            }

            var serializableExceptionData = new SerializableDictionary<Guid, string>();

            foreach (var exData in messageExceptionDictionary)
            {
                serializableExceptionData.Add(exData.Key, exData.Value.ToString());
            }

            var exception = new FaultException<ApplicationFault>(new ApplicationFault() { ErrorMessage = errorBuilder.ToString(), MessageExceptionDictionary = serializableExceptionData });
            throw exception;
        }
    }
}
