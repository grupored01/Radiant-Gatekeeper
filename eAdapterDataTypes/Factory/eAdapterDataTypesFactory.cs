namespace OTE.eAdapter.DataTypes
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;
    using System.Linq;
    using OTE.eAdapter.DataTypes.Universal;  
    

    /// <summary>
    /// Factory methods for eAdapter Data Types
    /// </summary>
    public static class eAdapterDataTypesFactory
    {
        internal const string CARGOWISE_NATIVE_NAMESPACE = "http://www.cargowise.com/Schemas/Native/2011/11";
        internal const string CARGOWISE_UNIVERSAL_NAMESPACE = "http://www.cargowise.com/Schemas/Universal/2011/11";
        internal const string CARGOWISE_INTERCHANGE_NAMESPACE = "http://www.edi.com.au/EnterpriseService/";
        private const string UNIVERSAL_WRAPPER = "<?xml version='1.0' encoding='utf-8'?><UniversalInterchange xmlns='{0}' version='1.0'><Header><SenderID>{1}</SenderID><RecipientID>{2}</RecipientID></Header><Body>{3}</Body></UniversalInterchange>";
        private const string NATIVE_REQUEST_WRAPPER = "<?xml version='1.0' encoding='utf-8' ?><Native xmlns='{0}'><Body>{1}</Body></Native> ";
        private const string SENDER = "OTXOTEPHX";
        private const string RECIPIENT = "T";

        /// <summary>
        /// Creates an Universal Event Message
        /// </summary>
        /// <param name="pShipmentNumber">Shipment number</param>
        /// <param name="pEventDate">Event Date</param>
        /// <param name="pEventType">Event Type</param>
        /// <param name="pEventReference">Event Reference</param>
        /// <param name="pIsEstimate">Is Estimate?</param>
        /// <returns>A wrapped Universal Event Message</returns>
        public static string CreateUniversalEvent(string pShipmentNumber, DateTime pEventDate, string pEventType, string pEventReference, bool pIsEstimate = false)
        {
            try
            {
                Event universalEvent = new Event();

                universalEvent.DataContext = new DataContext();

                universalEvent.DataContext.CodesMappedToTarget = true;
                universalEvent.DataContext.CodesMappedToTargetSpecified = true;

                string type = "ForwardingShipment";

                if (pShipmentNumber.StartsWith("C", StringComparison.InvariantCultureIgnoreCase))
                {
                    type = "ForwardingConsol";
                }

                universalEvent.DataContext.DataTargetCollection = new DataTarget[] { new DataTarget() { Key = pShipmentNumber, Type = type } };

                universalEvent.EventTime = pEventDate.ToString(System.Globalization.DateTimeFormatInfo.InvariantInfo.SortableDateTimePattern.ToString());

                universalEvent.EventType = pEventType;

                universalEvent.EventReference = pEventReference;

                universalEvent.IsEstimate = pIsEstimate;

                universalEvent.IsEstimateSpecified = true;

                universalEvent.ContextCollection = new EventContext[] {};

                UniversalEventData eventData = new UniversalEventData() { Event = universalEvent };

                return WrapMessage<UniversalEventData>(eventData, WrapperType.Universal);
            }
            catch (eAdapterFormatException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new eAdapterFormatException("Unable to create the universal event message", ex);
            }
        }

        /// <summary>
        /// Creates a Native request for shipments
        /// </summary>
        /// <param name="pShipmentNumber">Shipment Number</param>
        /// <returns>Native request</returns>
        public static string CreateNativeShipmentRequest(string pShipmentNumber)
        {
            NativeRequest request = new NativeRequest(NativeType.Shipment);

            request.ConditionGroups.Add(new NativeCriteriaGroup("JobShipment", "UniqueConsignRef", pShipmentNumber));

            return request.ToXml();
        }

        /// <summary>
        /// Creates a Native request for Organizations
        /// </summary>
        /// <param name="pOrganizationCode">Organization code</param>
        /// <returns>Native request</returns>
        public static string CreateNativeOrganizationRequest(string pOrganizationCode)
        {
            NativeRequest request = new NativeRequest(NativeType.Organization);

            request.ConditionGroups.Add(new NativeCriteriaGroup("OrgHeader", "Code", pOrganizationCode));

            return request.ToXml();
        }

        /// <summary>
        /// Creates a Native request for UNLOCO
        /// </summary>
        /// <param name="pUNLOCOCode">UNLOCO code</param>
        /// <returns>Native request</returns>
        public static string CreateNativeUNLOCORequest(string pUNLOCOCode)
        {
            NativeRequest request = new NativeRequest(NativeType.UNLOCO);

            request.ConditionGroups.Add(new NativeCriteriaGroup("RefUNLOCO", "Code", pUNLOCOCode));

            return request.ToXml();
        }

        /// <summary>
        /// Unwraps an eAdapter data type from a Message
        /// </summary>
        /// <typeparam name="T">eAdapter Data Type</typeparam>
        /// <param name="pMessage">Message</param>
        /// <returns>An eAdapter object</returns>
        public static List<T> UnwrapMessage<T>(string pMessage) where T : class
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                XmlNamespaceManager namespaces = new XmlNamespaceManager(xml.NameTable);

                WrapperType wrapper = RetrieveWrapperType(pMessage);

                switch (wrapper)
                {
                    case WrapperType.Native:
                    case WrapperType.NativeResponse:
                        namespaces.AddNamespace("ns", CARGOWISE_NATIVE_NAMESPACE);
                        break;
                    case WrapperType.Universal:
                        namespaces.AddNamespace("ns", CARGOWISE_UNIVERSAL_NAMESPACE);
                        break;
                    case WrapperType.Interchange:
                        namespaces.AddNamespace("ns", CARGOWISE_INTERCHANGE_NAMESPACE);
                        break;
                }

                List<T> value = new List<T>();

                using (TextReader textReader = new StringReader(pMessage))
                {
                    xml.Load(textReader);

                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                    XmlNodeList nodes = null;

                    switch (wrapper)
                    {
                        case WrapperType.Native:
                            nodes = xml.SelectNodes("//ns:Response/ns:Data/ns:DataItem", namespaces);
                            break;
                        case WrapperType.NativeResponse:
                            nodes = xml.SelectNodes("//ns:Response/ns:Data/ns:DataItem", namespaces);
                            break;
                        case WrapperType.Universal:
                            nodes = xml.SelectNodes("//ns:UniversalInterchange/ns:Body", namespaces);
                            break;
                        case WrapperType.Interchange:
                            nodes = xml.SelectNodes("//ns:XmlInterchange/ns:Payload", namespaces);
                            break;
                    }

                    foreach (XmlNode node in nodes)
                    {
                        using (StringReader stringReader = new StringReader(node.InnerXml))
                        {
                            using (XmlReader xmlReader = XmlReader.Create(stringReader))
                            {
                                value.Add((T)xmlSerializer.Deserialize(xmlReader));
                            }
                        }
                    }
                }

                return value;
            }
            catch (eAdapterFormatException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new eAdapterFormatException("Unable to unwrap the message", ex);
            }
        }

        /// <summary>
        /// Wraps an eAdapter object
        /// </summary>
        /// <typeparam name="T">eAdapter Data Type</typeparam>
        /// <param name="peAdapterDataTypes">eAdapter object</param>
        /// <param name="pWrapType">Wrapper Type</param>
        /// <returns>A wrapped eAdapter message</returns>
        public static string WrapMessage<T>(T peAdapterDataTypes, WrapperType pWrapType) where T : class
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                StringWriter stringWriter = new StringWriter();

                xmlSerializer.Serialize(stringWriter, peAdapterDataTypes);

                return WrapMessage(RemoveXmlHeader(stringWriter.ToString()), pWrapType);
            }
            catch (eAdapterFormatException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new eAdapterFormatException("Unable to wrap the message", ex);
            }
        }

        /// <summary>
        /// Wraps an eAdapter object
        /// </summary>
        /// <param name="pMessage">eAdapter message</param>
        /// <param name="pWrapType">Wrapper Type</param>
        /// <returns>A wrapped eAdapter message</returns>
        public static string WrapMessage(string pMessage, WrapperType pWrapType)
        {
            try
            {
                string value = string.Empty;

                switch (pWrapType)
                {
                    case WrapperType.Universal:
                        value = string.Format(UNIVERSAL_WRAPPER, CARGOWISE_UNIVERSAL_NAMESPACE, SENDER, RECIPIENT, pMessage);
                        break;
                    case WrapperType.Native:
                        value = string.Format(NATIVE_REQUEST_WRAPPER, CARGOWISE_NATIVE_NAMESPACE, pMessage);
                        break;
                }

                return value;
            }
            catch (eAdapterFormatException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new eAdapterFormatException("Unable to wrap the message", ex);
            }
        }

        /// <summary>
        /// Retrieves the wrapper type from an eAdapter message
        /// </summary>
        /// <param name="pMessage">eAdapter message</param>
        /// <returns>The wrapper type</returns>
        public static WrapperType RetrieveWrapperType(string pMessage)
        {
            try
            {
                WrapperType wrapperType = WrapperType.Native;

                //Descomentar esto   
                //string wrapperStart = RemoveXmlHeader(pMessage).Left(30).Replace(Environment.NewLine, string.Empty).ToLowerInvariant();

                //if (wrapperStart.StartsWith("<universalinterchange"))
                //{
                //    wrapperType = WrapperType.Universal;
                //}
                //else if (wrapperStart.StartsWith("<native"))
                //{
                //    wrapperType = WrapperType.Native;
                //}
                //else if (wrapperStart.StartsWith("<response"))
                //{
                //    wrapperType = WrapperType.NativeResponse;
                //}
                //else if (wrapperStart.StartsWith("<xmlinterchange"))
                //{
                //    wrapperType = WrapperType.Interchange;
                //}
                //else
                //{
                //    throw new eAdapterFormatException(string.Format("Unexpected wrapper type: {0}", wrapperStart.SubstringTo(" ")));
                //}

                return wrapperType;
            }
            catch (eAdapterFormatException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new eAdapterFormatException("Unable to parse the message", ex);
            }
        }

        /// <summary>
        /// Removes the Xml header from an Xml
        /// </summary>
        /// <param name="pMessage">Xml</param>
        /// <returns>An Xml without the xml header</returns>
        private static string RemoveXmlHeader(string pMessage)
        {
            try
            {
                return System.Text.RegularExpressions.Regex.Replace(pMessage, "<\\?xml.*\\?>", string.Empty).Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" ", string.Empty).Replace("xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" ", string.Empty);
            }
            catch (eAdapterFormatException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new eAdapterFormatException("Unable to remove the xml header from the message", ex);
            }
        }
    }
}
