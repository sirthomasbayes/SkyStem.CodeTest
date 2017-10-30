using Skystem.Challenge.Core.Entities;
using Skystem.Challenge.Core.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skystem.Challenge.Service.lib
{
	[Table("ItemGroups")]
	internal class ItemGroupEntity : IMappableTo<ItemGroup>
	{
		public Int32 Id { get; set; }

		public String Name { get; set; }

		public String Description { get; set; }

		[ForeignKey("AttributeId")]
		public virtual ICollection<ItemGroupAttributeEntity> Attributes { get; set; } 

		public ItemGroup Map()
		{
			return new ItemGroup(Id, Name, Description, Attributes != null ? Attributes.Select(x => x.Map()) : new List<ItemGroupAttribute>());
		}
	}
}
