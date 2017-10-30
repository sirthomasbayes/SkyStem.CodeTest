using Skystem.Challenge.Core.Services;
using Skystem.Challenge.Service.lib;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Skystem.Challenge.Service.Migrations
{
	/// <summary>
	/// IDbSeeder specifically for EntityFramework.
	/// </summary>
	public class EFMigrator : IDbSeeder
	{
		public EFMigrator()
		{
		}

		private static SemaphoreSlim _semaphore = new SemaphoreSlim(1);

		public async Task SeedAsync()
		{
			using (var context = new SkystemDbContext())
			{
				#region Critical Section
				// ensure only one thread 
				// can seed the db.
				await _semaphore.WaitAsync();

				// If we've seeded the database already, break.
				if (await context.DbVersions.AnyAsync()) return;

				var version = new DbVersion() { Version = 1, Description = "Seeding database." };
				context.DbVersions.Add(version);
				await context.SaveChangesAsync();

				_semaphore.Release();
				#endregion

				// Add 10 items
				var items = Enumerable.Range(1, 10).Select(x => new ItemEntity() { Name = $"Object_{x}", Description = $"Item {x} added to database." });
				foreach (var item in items) context.Items.Add(item);
				await context.SaveChangesAsync();

				items = await context.Items.ToListAsync();

				// add 5 attributes
				var attributes = Enumerable.Range(1, 5).Select(x => new AttributeTypeEntity() { Name = $"Attribute_{x}" });
				foreach (var attribute in attributes) context.AttributeTypes.Add(attribute);
				await context.SaveChangesAsync();

				attributes = await context.AttributeTypes.ToListAsync();
				var attributeArray = attributes.ToArray();
				var rand = new Random();

				// random values that can be assigned as values to attributes
				var randomValues = new String[] { "Red", "Blue", "Green" };

				// assign between 2 - 3 attributes to every item
				foreach (var item in items)
				{
					Shuffle(attributeArray, rand);

					var chosenAttributes = attributeArray.Take(rand.Next(2, 4));
					foreach (var itemAttribute in chosenAttributes) context.ItemAttributes.Add(new ItemAttributeEntity()
					{
						AttributeId = itemAttribute.Id,
						ItemId = item.Id,
						Value = randomValues[rand.Next(0, 3)]
					});
				}

				await context.SaveChangesAsync();

				// see if any items match on *every* attribute
				// Note: Implementation of GroupItemsAsync() groups items
				//		 whose (attribute,value) sets form a superset of
				//		 of the group.
				var matchAttributes = items.Select(x => new
				{
					Key = String.Join("+", x.Attributes
						.OrderBy(attr => attr.AttributeId)
						.Select(attr => $"{attr.AttributeId}_{attr.Value}")),
					Value = x
				})
				.GroupBy(x => x.Key)
				.Where(x => x.Count() > 1);

				// if we can't find a group,
				// make one, and add two items
				// that are part of this group
				if (matchAttributes.Count() == 0)
				{
					var forcedAttributes = attributeArray.Take(2);

					var firstItem = new ItemEntity() { Name = "GroupedItem_1", Description = "First forced grouped item." };
					var secondItem = new ItemEntity() { Name = "GroupedItem_2", Description = "Second forced grouped item." };
					var group = new ItemGroupEntity() { Name = "Forced_Group", Description = "Forced grouping." };

					context.Items.Add(firstItem);
					context.Items.Add(secondItem);
					context.ItemGroups.Add(group);

					await context.SaveChangesAsync();

					var firstItemAttributes = forcedAttributes.Select(x => new ItemAttributeEntity() { AttributeId = x.Id, ItemId = firstItem.Id, Value = "Match!" });
					var secondItemAttributes = forcedAttributes.Select(x => new ItemAttributeEntity() { AttributeId = x.Id, ItemId = secondItem.Id, Value = "Match!" });
					var groupAttributes = forcedAttributes.Select(x => new ItemGroupAttributeEntity() { AttributeId = x.Id, GroupId = group.Id, Value = "Match!" });

					foreach (var attribute in firstItemAttributes.Concat(secondItemAttributes)) context.ItemAttributes.Add(attribute);
					foreach (var attribute in groupAttributes) context.ItemGroupAttributes.Add(attribute);

					await context.SaveChangesAsync();
					return;
				}

				var groupCount = 1;

				foreach (var matchAttribute in matchAttributes)
				{
					var group = new ItemGroupEntity() { Name = $"Group {groupCount++}", Description = matchAttribute.Key };
					context.ItemGroups.Add(group);
					await context.SaveChangesAsync();

					foreach (var attribute in matchAttribute.First().Value.Attributes) context.ItemGroupAttributes.Add(new ItemGroupAttributeEntity() { GroupId = group.Id, AttributeId = attribute.AttributeId, Value = attribute.Value });
					await context.SaveChangesAsync();
				}
			}
		}

		public async Task UpdateAsync(Nullable<Int32> version = null)
		{
		}

		public async Task FlushAsync()
		{
			using (var context = new SkystemDbContext())
			{
				context.DbVersions.RemoveRange(context.DbVersions);
				context.ItemGroupAttributes.RemoveRange(context.ItemGroupAttributes);
				context.ItemAttributes.RemoveRange(context.ItemAttributes);
				context.AttributeTypes.RemoveRange(context.AttributeTypes);
				context.ItemGroups.RemoveRange(context.ItemGroups);
				context.Items.RemoveRange(context.Items);

				await context.SaveChangesAsync();
			}
		}

		private void Shuffle<T>(T[] array, Random randomizer = null)
		{
			randomizer = randomizer ?? new Random();

			for (var i = 0; i < array.Length; i++)
			{
				var index = randomizer.Next(i, array.Length);
				var temp = array[i];

				array[i] = array[index];
				array[index] = temp;
			}
		}
	}
}
