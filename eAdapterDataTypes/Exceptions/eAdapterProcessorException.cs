namespace OTE.eAdapter.DataTypes
{
    using System;
    
    /// <summary>
    /// The exception that is thrown when there is an error on a message processor
    /// </summary>
    public class eAdapterProcessorException : Exception
    {
        /// <summary>
        /// Processor name
        /// </summary>
        string processorName = string.Empty;

        /// <summary>
        /// Gets the Processor Name
        /// </summary>
        public string ProcessorName
        {
            get
            {
                return this.processorName;
            }
        }

        public eAdapterProcessorException()
            : base()
        {
        }

        public eAdapterProcessorException(string processorName)
            : base()
        {
            this.processorName = processorName;
        }

        public eAdapterProcessorException(string processorName, string message)
            : base(message)
        {
            this.processorName = processorName;
        }

        public eAdapterProcessorException(string processorName, string message, Exception innerException)
            : base(message, innerException)
        {
            this.processorName = processorName;
        }
    }
}
