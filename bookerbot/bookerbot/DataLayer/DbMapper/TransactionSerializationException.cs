using System.Runtime.Serialization;

namespace bookerbot.DataLayer.DbMapper
{
    [Serializable]
    public class TransactionSerializationException : Exception
    {
        public TransactionSerializationException(string message, Exception innerException):base(message, innerException)
        {
        }
        
        protected TransactionSerializationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}