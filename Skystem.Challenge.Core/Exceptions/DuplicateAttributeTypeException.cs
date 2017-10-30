using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skystem.Challenge.Core.Exceptions
{
	public class DuplicateAttributeTypeException : Exception
	{
		public DuplicateAttributeTypeException(String name)
			:base($"AttributeType with name {name} already exists.")
		{
		}
	}
}
