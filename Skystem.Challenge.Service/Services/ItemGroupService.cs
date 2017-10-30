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
	public class ItemGroupEFService : IItemGroupService
	{
		public async Task<ItemGroup> GetGroupByIdAsync(Int32 id)
		{
			using (var context = new SkystemDbContext())
			{
				var group = await context.ItemGroups
					.Include("Attributes.Attribute")
					.FirstOrDefaultAsync(x => x.Id == id);

				return group != null ? group.Map() : null;
			}
		}

		public async Task<IEnumerable<ItemGroup>> GetGroupsAsync(Boolean pageResults = false, Int32 page = 1, Int32 pageSize = 15)
		{
			using (var context = new SkystemDbContext())
			{
				if (false == pageResults) return (await context.ItemGroups
						.Include("Attributes.Attribute")
						.ToListAsync())
						.Select(x => x.Map());

				page = page >= 1 ? page : 1;
				pageSize = pageSize >= 1 ? pageSize : 1;

				var count = await context.ItemGroups.CountAsync();
				var attributes = (await context.ItemGroups
					.OrderBy(x => x.Id)
					.Skip((page - 1) * pageSize)
					.Take(pageSize)
					.Include("Attributes.Attribute")
					.ToListAsync())
					.Select(x => x.Map());

				return new PagedResult<ItemGroup>(page, pageSize,
					count / pageSize + (count % pageSize > 0 ? 1 : 0),
					attributes);
			}
		}

		public async Task<IEnumerable<Item>> GetGroupItemsAsync(Int32 id)
		{
			using (var context = new SkystemDbContext())
			{
				var group = await context.ItemGroups
					.Include("Attributes.Attribute")
					.FirstOrDefaultAsync(x => x.Id == id);

				if (group == null) throw new ItemGroupNotFoundException(id);

				var groupAttributes = group.Attributes.OrderBy(x => x.AttributeId);
				var attributeIds = groupAttributes.Select(x => x.AttributeId).ToArray();
				var attributeKvp = groupAttributes.ToLookup(x => x.AttributeId);

				var itemAttributesByItemId = (await context.ItemAttributes
					.Where(x => attributeIds.Contains(x.AttributeId))
					.ToListAsync())
					.GroupBy(x => x.ItemId);

				var candidateItemIds = itemAttributesByItemId.Where(x =>
				{
					var matchCount = 0;

					foreach (var attribute in x)
					{
						if (false == attributeKvp.Contains(attribute.AttributeId)) continue;
						if (attributeKvp[attribute.AttributeId].First().Value != attribute.Value) return false;
						matchCount++;
					}

					return matchCount == attributeKvp.Count();
				})
				.Select(x => x.Key)
				.ToArray();

				return (await context.Items.Where(x => candidateItemIds.Contains(x.Id)).ToListAsync())
					.Select(x => x.Map());
			}
		}

		public async Task<ItemGroup> AddItemGroupAsync(String name, String description)
		{
			using (var context = new SkystemDbContext())
			{
				var group = new ItemGroupEntity() { Name = name, Description = description };
				context.ItemGroups.Add(group);

				await context.SaveChangesAsync();
				return group.Map();
			}
		}

		public async Task<ItemGroup> UpdateItemGroupAsync(Int32 id, String name, String description)
		{
			using (var context = new SkystemDbContext())
			{
				var group = await context.ItemGroups.FirstOrDefaultAsync(x => x.Id == id);
				if (group == null) throw new ItemGroupNotFoundException(id);

				group.Name = name;
				group.Description = description;

				await context.SaveChangesAsync();
				return group.Map();
			}
		}

		public async Task<ItemGroup> RemoveItemGroupAsync(Int32 id)
		{
			using (var context = new SkystemDbContext())
			{
				var group = await context.ItemGroups.FirstOrDefaultAsync(x => x.Id == id);
				if (group == null) throw new ItemGroupNotFoundException(id);

				context.ItemGroups.Remove(group);

				await context.SaveChangesAsync();
				return group.Map();
			}
		}
	}
}
