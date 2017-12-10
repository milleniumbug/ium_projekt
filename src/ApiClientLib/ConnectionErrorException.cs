using System;
using System.Runtime.Serialization;

namespace ApiClientLib
{
	public class ConnectionErrorException : Exception
	{
		public ConnectionErrorException()
		{
		}

		public ConnectionErrorException(string message) : base(message)
		{
		}

		public ConnectionErrorException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected ConnectionErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}

	public class ElementNotFound : ConnectionErrorException
	{
		public ElementNotFound()
		{
		}

		public ElementNotFound(string message) : base(message)
		{
		}

		public ElementNotFound(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected ElementNotFound(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}