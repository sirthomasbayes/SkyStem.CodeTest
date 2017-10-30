using Skystem.Challenge.Core.Entities;
using Skystem.Challenge.Core.Exceptions;
using Skystem.Challenge.Core.Services;
using Skystem.Challenge.Core.Utilities;
using Skystem.Challenge.Service.lib;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skystem.Challenge.Service
{
	public class AttributeService : IAttributeService
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
			var attribute = new AttributeTypeEntity() { Name = attributeName };
			using (var context = new SkystemDbContext())
			{
				context.AttributeTypes.Add(attribute);
				await context.SaveChangesAsync();
			}

			return attribute.Map();
		}

		public async Task<Item> AssignAttributeToItemAsync(Item item, AttributeType attribute, String value)
		{
			using (var context = new SkystemDbContext())
			{
				if (false == await context.Items.AnyAsync(x => x.Id == item.Id)) throw new ItemNotFoundException(item.Id);
				if (false == await context.AttributeTypes.AnyAsync(x => x.Id == attribute.Id)) throw new AttributeTypeNotFoundException(attribute.Name);

				var itemAttribute = new ItemAttributeEntity() { ItemId = item.Id, AttributeId = attribute.Id, Value = value };
				context.ItemAttributes.Add(itemAttribute);
				await context.SaveChangesAsync();

				return new Item(item.Id,
					item.Name,
					item.Description,
					item.Attributes.Concat(new List<ItemAttribute>() { itemAttribute.Map() }));
			}
		}

		public async Task<ItemGroup> AssignAttributeToGroupAsync(ItemGroup itemGroup, AttributeType attribute, String value)
		{
			using (var context = new SkystemDbContext())
			{
				if (false == await context.ItemGroups.AnyAsync(x => x.Id == itemGroup.Id)) throw new ItemGroupNotFoundException(itemGroup.Id);
				if (false == await context.AttributeTypes.AnyAsync(x => x.Id == attribute.Id)) throw new AttributeTypeNotFoundException(attribute.Name);

				var groupAttribute = new ItemGroupAttributeEntity() { GroupId = itemGroup.Id, AttributeId = attribute.Id, Value = value };
				context.ItemGroupAttributes.Add(groupAttribute);
				await context.SaveChangesAsync();

				return new ItemGroup(itemGroup.Id,
					itemGroup.Name,
					itemGroup.Description,
					itemGroup.Attributes.Concat(new List<ItemGroupAttribute>() { groupAttribute.Map() }));
			}
		}
	}
}
