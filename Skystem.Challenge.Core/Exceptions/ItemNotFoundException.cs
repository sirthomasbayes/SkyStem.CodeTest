using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skystem.Challenge.Core.Exceptions
{
	public class ItemNotFoundException : Exception
	{
		public ItemNotFoundException(Int32 id)
			:base($"Item with id {id} was not found.")
		{
		}
	}
}
