using Skystem.Challenge.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skystem.Challenge.Service.lib
{
	internal class SkystemDbContext : DbContext
	{
		public SkystemDbContext()
			:base("SkystemDb")
		{
		}

		public DbSet<DbVersion> DbVersions { get; set; }

		public DbSet<ItemEntity> Items { get; set; } 
		public DbSet<ItemAttributeEntity> ItemAttributes { get; set; }

		public DbSet<ItemGroupEntity> ItemGroups { get; set; }
		public DbSet<ItemGroupAttributeEntity> ItemGroupAttributes { get; set; }

		public DbSet<AttributeTypeEntity> AttributeTypes { get; set; }
	}
}
