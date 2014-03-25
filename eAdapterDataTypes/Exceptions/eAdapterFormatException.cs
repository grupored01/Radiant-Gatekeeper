namespace OTE.eAdapter.DataTypes
{
    using System;

    /// <summary>
    /// The exception that is thrown when there is an error on the format of the message
    /// </summary>
    public class eAdapterFormatException : Exception
    {
        public eAdapterFormatException()
            : base()
        {
        }

        public eAdapterFormatException(string message)
            : base(message)
        {
        }

        public eAdapterFormatException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
