using Skystem.Challenge.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skystem.Challenge.Core.Entities
{
	public class AttributeType
	{
		public AttributeType(Int32 id, String name)
		{
			Assert.IsNotNullOrWhitespace(name);

			Id = id;
			Name = name;
		}

		public Int32 Id { get; internal set; }

		public String Name { get; internal set; }
	}

	public class ItemAttribute
	{
		public ItemAttribute(Int32 itemId, Int32 attributeId, String name, String value)
		{
			Assert.IsNotNullOrWhitespace(name);
			Assert.IsNotNullOrWhitespace(value);

			ItemId = itemId;
			AttributeId = attributeId;

			Name = name;
			Value = value;
		}

		public Int32 ItemId { get; internal set; }

		public Int32 AttributeId { get; internal set; }

		public String Name { get; internal set; }

		public String Value { get; internal set; }
	}

	public class ItemGroupAttribute
	{
		public ItemGroupAttribute(Int32 groupId, Int32 attributeId, String name, String value)
		{
			Assert.IsNotNullOrWhitespace(name);
			Assert.IsNotNullOrWhitespace(value);

			GroupId = groupId;
			AttributeId = attributeId;

			Name = name;
			Value = value;
		}

		public Int32 GroupId { get; internal set; }

		public Int32 AttributeId { get; internal set; }

		public String Name { get; internal set; }

		public String Value { get; internal set; }
	}
}
