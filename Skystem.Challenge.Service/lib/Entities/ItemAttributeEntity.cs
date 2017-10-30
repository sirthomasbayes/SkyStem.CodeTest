using Skystem.Challenge.Core.Entities;
using Skystem.Challenge.Core.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skystem.Challenge.Service.lib
{
	[Table("AttributeTypes")]
	internal class AttributeTypeEntity : IMappableTo<AttributeType>
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Int32 Id { get; set; }

		[StringLength(255)]
		[Index(IsClustered = false, IsUnique = true)]
		public String Name { get; set; }

		public AttributeType Map()
		{
			return new AttributeType(Id, Name);
		}
	}

	[Table("ItemAttributes")]
	internal class ItemAttributeEntity : IMappableTo<ItemAttribute>
	{
		[Key, Column(Order = 0)]
		public Int32 ItemId { get; set; }

		[Key, Column(Order = 1)]
		[Index(IsClustered = false)]
		public Int32 AttributeId { get; set; }

		public String Value { get; set; }

		[ForeignKey("ItemId")]
		public virtual ItemEntity Item { get; set; }

		[ForeignKey("AttributeId")]
		public virtual AttributeTypeEntity Attribute { get; set; }

		public ItemAttribute Map()
		{
			return new ItemAttribute(ItemId, AttributeId, Attribute.Name, Value);
		}
	}

	[Table("ItemGroupAttributes")]
	internal class ItemGroupAttributeEntity : IMappableTo<ItemGroupAttribute>
	{
		[Key, Column(Order = 0)]
		public Int32 GroupId { get; set; }

		[Key, Column(Order = 1)]
		[Index(IsClustered = false)]
		public Int32 AttributeId { get; set; }

		public String Value { get; set; }

		[ForeignKey("GroupId")]
		public virtual ItemGroupEntity Group { get; set; }

		[ForeignKey("AttributeId")]
		public virtual AttributeTypeEntity Attribute { get; set; }

		public ItemGroupAttribute Map()
		{
			return new ItemGroupAttribute(GroupId, AttributeId, Attribute.Name, Value);
		}
	}
}
