using System;

namespace EventSpike.Approval.AggregateSourceImplementation.Persistence
{
    /// <summary>
    /// Represents a general failure of the persistence infrastructure.
    /// 
    /// </summary>
    [Serializable]
    public class PersistenceException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the PersistenceException class.
        /// 
        /// </summary>
        /// <param name="message">The message that describes the error.</param><param name="innerException">The message that is the cause of the current exception.</param>
        public PersistenceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}