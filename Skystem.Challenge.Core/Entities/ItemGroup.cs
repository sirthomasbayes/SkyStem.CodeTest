using Skystem.Challenge.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skystem.Challenge.Core.Entities
{
	public class ItemGroup
	{
		public ItemGroup(Int32 id, String name, String description, IEnumerable<ItemGroupAttribute> attributes = null)
		{
			Assert.IsNotNullOrWhitespace(name);

			Id = id;
			Name = name;
			Description = description;

			Attributes = attributes ?? new List<ItemGroupAttribute>();
		}

		public Int32 Id { get; internal set; }

		public String Name { get; internal set; }

		public String Description { get; internal set; }

		public IEnumerable<ItemGroupAttribute> Attributes { get; internal set; }
	}
}
