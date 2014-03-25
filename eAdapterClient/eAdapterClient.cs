namespace WcfRadiant
{
    using System;
    using System.IO;
    using System.Xml;
    using CargoWise.eHub.Adapter;
    using CargoWise.eHub.Common;

    /// <summary>
    /// eAdapter web service client class
    /// </summary>
    public class eAdapterClient
    {
        private string serviceAddress = string.Empty;

        /// <summary>
        /// Web Service Address
        /// </summary>
        public string ServiceAddress
        {
            get 
            { 
                return serviceAddress; 
            }
        }
        private string recipientId = string.Empty;

        /// <summary>
        /// Recipient Id
        /// </summary>
        public string RecipientId
        {
            get 
            { 
                return recipientId; 
            }
        }
        private string senderId = string.Empty;

        /// <summary>
        /// Sender Id
        /// </summary>
        public string SenderId
        {
            get 
            { 
                return senderId; 
            }
        }
        private string password = string.Empty;

        /// <summary>
        /// Password
        /// </summary>
        public string Password
        {
            get 
            { 
                return password; 
            }
        }

        /// <summary>
        /// Creates a new instance of eAdapterClient
        /// </summary>
        /// <param name="pServiceAddress">Web Service Address</param>
        /// <param name="pRecipientId">Recipient Id</param>
        /// <param name="pSenderId">Sender Id</param>
        /// <param name="pPassword">Password</param>
        public eAdapterClient(string pServiceAddress, string pRecipientId, string pSenderId, string pPassword)
        {
            this.serviceAddress = pServiceAddress;
            this.recipientId = pRecipientId;
            this.senderId = pSenderId;
            this.password = pPassword;
        }

        /// <summary>
        /// Pings the web service
        /// </summary>
        /// <returns></returns>
        public bool Ping()
        {
            using (var adapter = new eHubAdapter(RetrieveConfiguration(this.serviceAddress), this.senderId, this.password))
            {
                return true;
            }
        }

        /// <summary>
        /// Sends a message to the web service
        /// </summary>
        /// <param name="pMessageStream">Message file stream</param>
        public void SendMessage(FileStream pMessageStream)
        {
            string messageNamespace = RetrieveMessageNamespace(pMessageStream);

            using (var adapter = new eHubAdapter(RetrieveConfiguration(this.serviceAddress), this.senderId, this.password))
            {
                adapter.Outbox.AddMessage(
                    new eHubMessage(
                        Guid.NewGuid(),
                        this.senderId,
                        this.recipientId,
                        MessageSchemaType.Xml,
                        RetrieveApplicationCode(messageNamespace),
                        RetrieveSchemaName(messageNamespace),
                        pMessageStream
                        ));

                adapter.SendMessages();
            }
        }

        /// <summary>
        /// Sends a message to the web service
        /// </summary>
        /// <param name="pMessageFilePath">Message path</param>
        public void SendMessage(string pMessageFilePath)
        {
            using (var fileStream = new FileStream(pMessageFilePath, FileMode.Open, FileAccess.Read))
            {
                this.SendMessage(fileStream);
            }
        }

        /// <summary>
        /// Retrieves the namespace from the message
        /// </summary>
        /// <param name="pMessageFile">Message stream</param>
        /// <returns>Namespace</returns>
        private static string RetrieveMessageNamespace(Stream pMessageFile)
        {
            using (var reader = XmlReader.Create(pMessageFile))
            {
                ReadToNextElement(reader);

                return reader.NamespaceURI;
            }
        }

        /// <summary>
        /// Reads the next element from the XmlReader
        /// </summary>
        /// <param name="pReader">Message's XmlReader</param>
        private static void ReadToNextElement(XmlReader pReader)
        {
            while (pReader.Read())
            {
                if (pReader.NodeType == XmlNodeType.Element)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Retrieves the application code for the namespace
        /// </summary>
        /// <param name="pMessageNamespace">Message's namespace</param>
        /// <returns>Application code</returns>
        private static string RetrieveApplicationCode(string pMessageNamespace)
        {
            switch (pMessageNamespace)
            {
                case "http://www.cargowise.com/Schemas/Universal":
                case "http://www.cargowise.com/Schemas/Universal/2011/11":
                    return "UDM";
                case "http://www.cargowise.com/Schemas/Native":
                    return "NDM";
                case "http://www.edi.com.au/EnterpriseService/":
                    return "XMS";
                default: return "";
            }
        }

        /// <summary>
        /// Retrieves the schema name for the namespace
        /// </summary>
        /// <param name="pMessageNamespace">Message's namespace</param>
        /// <returns>Schema name</returns>
        private static string RetrieveSchemaName(string pMessageNamespace)
        {
            switch (pMessageNamespace)
            {
                case "http://www.cargowise.com/Schemas/Native":
                case "http://www.cargowise.com/Schemas/Universal":
                case "http://www.cargowise.com/Schemas/Universal/2011/11":
                    return pMessageNamespace + "#UniversalInterchange";
                case "http://www.edi.com.au/EnterpriseService/":
                    return pMessageNamespace + "#XmlInterchange";
                default:
                    return pMessageNamespace;
            }
        }

        /// <summary>
        /// Retrieves the configuration for the web service
        /// </summary>
        /// <param name="pSrviceAddress">Web Service Address</param>
        /// <returns>eAdapterHttpConfiguration or eAdapterHttpsConfiguration</returns>
        private static IServiceConfiguration RetrieveConfiguration(string pSrviceAddress)
        {
            if (pSrviceAddress.StartsWith("https"))
            {
                return new eAdapterHttpsConfiguration(pSrviceAddress);
            }
            else
            {
                return new eAdapterHttpConfiguration(pSrviceAddress);
            }
        }
    }
}
