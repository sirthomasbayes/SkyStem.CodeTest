using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skystem.Challenge.Core.Exceptions
{
	public class AttributeTypeNotFoundException : Exception
	{
		public AttributeTypeNotFoundException(Int32 id)
			:base($"Attribute with id {id} was not found.")
		{
		}

		public AttributeTypeNotFoundException(String name)
			:base($"Attribute with name {name} was not found.")
		{
		}
	}
}
