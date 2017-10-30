using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skystem.Challenge.Core.Exceptions
{
	public class ItemGroupNotFoundException : Exception
	{
		public ItemGroupNotFoundException(Int32 id)
			:base($"ItemGroup with id {id} was not found.")
		{
		}
	}
}
