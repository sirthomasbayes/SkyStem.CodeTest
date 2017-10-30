using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skystem.Challenge.Core.Utilities
{
	public static class Assert
	{
		public static void IsNotNull(Object obj, String errorMessage = "Value cannot be null")
		{
			if (obj == null) throw new ArgumentNullException(errorMessage);
		}

		public static void IsNotNullOrWhitespace(String str, String errorMessage = "String value must contain at least one non-whitespace character")
		{
			if (String.IsNullOrWhiteSpace(str)) throw new ArgumentNullException(errorMessage);
		}

		public static void IsLengthBounded(String str, Int32 length, String errorMessage = null)
		{
			errorMessage = errorMessage ?? $"String value's length must be at most {length} characters";
			if (str.Length > length) throw new ArgumentException(errorMessage);
		}
	}
}
