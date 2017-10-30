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
	public class ItemEFService : IItemService
	{
		public async Task<Item> GetItemByIdAsync(Int32 id)
		{
			using (var context = new SkystemDbContext())
			{
				var item = await context.Items
					.Where(x => x.Id == id)
					.Include("Attributes.Attribute")
					.FirstOrDefaultAsync();

				return item != null ? item.Map() : null;
			}
		}

		public async Task<IEnumerable<Item>> GetItemsAsync(Boolean pageResults = false, Int32 page = 1, Int32 pageSize = 15)
		{
			using (var context = new SkystemDbContext())
			{
				if (false == pageResults) return (await context.Items
						.Include("Attributes.Attribute")
						.ToListAsync())
						.Select(x => x.Map());

				page = page >= 1 ? page : 1;
				pageSize = pageSize >= 1 ? pageSize : 1;

				var count = await context.Items.CountAsync();
				var items = (await context.Items
					.OrderBy(x => x.Id)
					.Skip((page - 1) * pageSize)
					.Take(pageSize)
					.ToListAsync())
					.Select(x => x.Map());

				return new PagedResult<Item>(
					page,
					pageSize,
					count / pageSize + (count % pageSize > 0 ? 1 : 0),
					items);
			}
		}

		public async Task<Item> AddItemAsync(String name, String description)
		{
			Assert.IsNotNullOrWhitespace(name);

			var item = new ItemEntity() { Name = name, Description = description };
			using (var context = new SkystemDbContext())
			{
				context.Items.Add(item);
				await context.SaveChangesAsync();
			}

			return item.Map();
		}

		public async Task<Item> UpdateItemAsync(Int32 id, String name, String description)
		{
			using (var context = new SkystemDbContext())
			{
				var item = await context.Items.FirstOrDefaultAsync(x => x.Id == id);
				if (item == null) throw new ItemNotFoundException(id);

				item.Name = name;
				item.Description = description;

				await context.SaveChangesAsync();
				return item.Map();
			}
		}

		public async Task<Item> RemoveItemAsync(Int32 id)
		{
			using (var context = new SkystemDbContext())
			{
				var item = await context.Items.FirstOrDefaultAsync(x => x.Id == id);
				if (item == null) throw new ItemNotFoundException(id);

				context.Items.Remove(item);

				await context.SaveChangesAsync();
				return item.Map();
			}
		}
	}
}
