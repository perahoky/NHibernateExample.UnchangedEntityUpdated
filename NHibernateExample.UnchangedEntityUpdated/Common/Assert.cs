using System;
using System.Runtime.CompilerServices;

namespace NHibernateExample.UnchangedEntityUpdated.Common;
internal static class Assert
{
	public static void AssertArtgumentIsNotNull(this object? argument, [CallerArgumentExpression(nameof(argument))] string? argumentName = null)
	{
		ArgumentNullException.ThrowIfNull(argument, argumentName);
	}

	public static void AssertArtgumentIsNotEmpty(this Guid argument, [CallerArgumentExpression(nameof(argument))] string? argumentName = null)
	{
		if (argument.Equals(Guid.Empty))
		{
			throw new ArgumentException($"The guid argument named {argumentName} is empty.");
		}
	}

	public static void IsTrue(bool actual, [CallerArgumentExpression(nameof(actual))] string? nameofActual = null)
	{
		if (!actual)
		{
			throw new TestException($"The actual value of '{nameofActual}' was expected to be true but is false.");
		}
	}

	public static void IsTrue<T>(bool actual, string message, [CallerArgumentExpression(nameof(actual))] string? nameofActual = null)
	{
		if (!actual)
		{
			throw new TestException($"The actual value of '{nameofActual}' was expected to be true but is false.{Environment.NewLine}{message}");
		}
	}

	public static void IsNotNull(object? actual, string message, [CallerArgumentExpression(nameof(actual))] string? nameofActual = null)
	{
		if (actual is null)
		{
			throw new TestException($"The actual value of '{nameofActual}' was expected to be not null but is indeed null.{Environment.NewLine}{message}");
		}
	}

	public static void IsNull(object? actual, string message, [CallerArgumentExpression(nameof(actual))] string? nameofActual = null)
	{
		if (actual is not null)
		{
			throw new TestException($"The actual value of '{nameofActual}' was expected to be null but is not null.{Environment.NewLine}{message}");
		}
	}

	public static void AreEqual<T>(T? expected, T? actual, string? message = null, [CallerArgumentExpression(nameof(expected))] string? nameofExpected = null, [CallerArgumentExpression(nameof(actual))] string? nameofActual = null)
	{
		if (!object.Equals(expected, actual))
		{
			if (message is null)
			{
				throw new TestException($"The actual value '{actual?.ToString() ?? "null"}' of '{nameofActual}' is not equal to expected value '{expected?.ToString() ?? "null"}'.");
			}
			else
			{
				throw new TestException($"The actual value '{actual?.ToString() ?? "null"}' of '{nameofActual}' is not equal to expected value '{expected?.ToString() ?? "null"}'.{Environment.NewLine}{message}");
			}
		}
	}
}