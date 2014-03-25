namespace OTE.eAdapter.DataTypes
{
    using System;

    /// <summary>
    /// The exception that is thrown when there is an error on the header of the message
    /// </summary>
    public class eAdapterHeaderException : Exception
    {
        public eAdapterHeaderException()
            : base()
        {
        }

        public eAdapterHeaderException(string message)
            : base(message)
        {
        }

        public eAdapterHeaderException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
