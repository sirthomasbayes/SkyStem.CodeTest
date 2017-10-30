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
	[Table("Items")]
	internal class ItemEntity : IMappableTo<Item>
	{
		[Key]
		public Int32 Id { get; set; }

		public String Name { get; set; }

		public String Description { get; set; }

		[ForeignKey("AttributeId")]
		public virtual ICollection<ItemAttributeEntity> Attributes { get; set; }

		public Item Map()
		{
			return new Item(Id, Name, Description, Attributes != null ? Attributes.Select(x => x.Map()) : new List<ItemAttribute>());
		}
	}
}
