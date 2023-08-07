using System;
using System.Runtime.Serialization;

namespace NHibernateExample.UnchangedEntityUpdated.Common;

[Serializable]
public class TestException : Exception
{
	public TestException()
	{
	}

	public TestException(string message)
		: base(message)
	{
	}

	public TestException(object? expected, object? actual, string nameofExpectedArgument, string nameofActualArgument)
		: base($"The actual value '{actual ?? "null"}' of '{nameofActualArgument}' is not equal to expected value '{expected ?? "null"}'.")
	{
	}

	public TestException(string message, object? expected, object? actual, string nameofExpectedArgument, string nameofActualArgument)
		: base($"The actual value '{actual}' of '{nameofActualArgument}' is not equal to expected value '{expected}'.{Environment.NewLine}{message}")
	{
	}

	public TestException(string message, Exception inner)
		: base(message, inner)
	{
	}

	protected TestException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}