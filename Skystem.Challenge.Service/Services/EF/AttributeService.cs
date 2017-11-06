using Skystem.Challenge.Core.Entities;
using Skystem.Challenge.Core.Exceptions;
using Skystem.Challenge.Core.Services;
using Skystem.Challenge.Core.Utilities;
using Skystem.Challenge.Service.lib;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skystem.Challenge.Service
{
	public class AttributeEFService : IAttributeService
	{
		public async Task<AttributeType> GetAttributeByIdAsync(Int32 id)
		{
			using (var context = new SkystemDbContext())
			{
				var attribute = await context.AttributeTypes.FirstOrDefaultAsync(x => x.Id == id);
				return attribute != null ? attribute.Map() : null;
			}
		}

		public async Task<AttributeType> GetAttributeByNameAsync(String name)
		{
			using (var context = new SkystemDbContext())
			{
				var attribute = await context.AttributeTypes.FirstOrDefaultAsync(x => x.Name == name);
				return attribute != null ? attribute.Map() : null;
			}
		}

		public async Task<IEnumerable<AttributeType>> GetAttributesAsync(Boolean pageResults = false, Int32 page = 1, Int32 pageSize = 15)
		{
			using (var context = new SkystemDbContext())
			{
				if (false == pageResults) return (await context.AttributeTypes.ToListAsync()).Select(x => x.Map());

				page = page >= 1 ? page : 1;
				pageSize = pageSize >= 1 ? pageSize : 1;

				var count = await context.AttributeTypes.CountAsync();
				var attributes = (await context.AttributeTypes
					.OrderBy(x => x.Id)
					.Skip((page - 1) * pageSize)
					.Take(pageSize)
					.ToListAsync())
					.Select(x => x.Map());

				return new PagedResult<AttributeType>(page, pageSize,
					count / pageSize + (count % pageSize > 0 ? 1 : 0),
					attributes);
			}
		}

		public async Task<AttributeType> AddAttributeTypeAsync(String attributeName)
		{
			Assert.IsNotNullOrWhitespace(attributeName);
			Assert.IsLengthBounded(attributeName, 255);

			var attribute = new AttributeTypeEntity() { Name = attributeName };
			using (var context = new SkystemDbContext())
			{
				try
				{
					context.AttributeTypes.Add(attribute);
					await context.SaveChangesAsync();
				}
				catch (Exception e)
				{
					if (e.InnerException != null &&
						e.InnerException.InnerException != null)
					{
						var sqlException = e.InnerException.InnerException as SqlException;
						if (sqlException != null && sqlException.Number == 2601) throw new DuplicateAttributeTypeException(attributeName);
					}

					throw e;
				}
			}

			return attribute.Map();
		}

		public async Task<Item> AssignAttributeToItemAsync(Int32 itemId, Int32 attributeId, String value)
		{
			using (var context = new SkystemDbContext())
			{
				var unmappedItem = await context.Items
					.Include("Attributes.Attribute")
					.FirstOrDefaultAsync(x => x.Id == itemId);

				var item = unmappedItem != null ? unmappedItem.Map() : null;

				var attribute = await context.AttributeTypes.FirstOrDefaultAsync(x => x.Id == attributeId);

				if (item == null) throw new ItemNotFoundException(itemId);
				if (attribute == null) throw new AttributeTypeNotFoundException(attributeId);

				var itemAttribute = await context.ItemAttributes
					.Include(x => x.Attribute)
					.FirstOrDefaultAsync(x => x.AttributeId == attributeId && x.ItemId == item.Id);

				if (itemAttribute == null)
				{
					itemAttribute = new ItemAttributeEntity() { ItemId = item.Id, AttributeId = attributeId, Value = value, Attribute = attribute };
					context.ItemAttributes.Add(itemAttribute);
				}
				else itemAttribute.Value = value;

				await context.SaveChangesAsync();
				return new Item(item.Id,
					item.Name,
					item.Description,
					item.Attributes);
			}
		}

		public async Task<ItemGroup> AssignAttributeToGroupAsync(Int32 itemGroupId, Int32 attributeId, String value)
		{
			using (var context = new SkystemDbContext())
			{
				var group = await context.ItemGroups
					.Include("Attributes.Attribute")
					.FirstOrDefaultAsync(x => x.Id == itemGroupId);

				var itemGroup = group != null ? group.Map() : null;

				var attribute = await context.AttributeTypes.FirstOrDefaultAsync(x => x.Id == attributeId);

				if (itemGroup == null) throw new ItemGroupNotFoundException(itemGroupId);
				if (attribute == null) throw new AttributeTypeNotFoundException(attributeId);

				var groupAttribute = await context.ItemGroupAttributes
					.Include(x => x.Attribute)
					.FirstOrDefaultAsync(x => x.AttributeId == attributeId && x.GroupId == itemGroupId);

				if (groupAttribute == null)
				{
					groupAttribute = new ItemGroupAttributeEntity() { GroupId = itemGroup.Id, AttributeId = attributeId, Value = value, Attribute = attribute };
					context.ItemGroupAttributes.Add(groupAttribute);
				}
				else groupAttribute.Value = value;

				await context.SaveChangesAsync();
				return new ItemGroup(itemGroup.Id,
					itemGroup.Name,
					itemGroup.Description,
					itemGroup.Attributes);
			}
		}
	}
}
