using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skystem.Challenge.Service.Migrations;
using Skystem.Challenge.Core.Services;
using Skystem.Challenge.Service;
using System.Threading.Tasks;
using Skystem.Challenge.Core.Exceptions;
using System.Linq;
using System.Data.Entity.Core;

namespace Skystem.Challenge.App.Tests
{
	[TestClass]
	public class ItemGroupEFServiceTests
	{
		private IDbSeeder DbMigrator = IoCContainer.DefaultContainer.GetInstance<IDbSeeder>();
		private IAttributeService AttributeService = IoCContainer.DefaultContainer.GetInstance<IAttributeService>();

		private IItemService ItemService = IoCContainer.DefaultContainer.GetInstance<IItemService>();
		private IItemGroupService ItemGroupService = IoCContainer.DefaultContainer.GetInstance<IItemGroupService>();

		[TestInitialize]
		public void Initialize()
		{
			DbMigrator.FlushAsync().Wait();
		}

		[TestMethod]
		public async Task ShouldAddGroupToDatabaseAsync()
		{
			var group = await ItemGroupService.AddItemGroupAsync("First Group", "My first group.");

			Assert.IsNotNull(group);
			Assert.AreEqual("First Group", group.Name);
			Assert.AreEqual("My first group.", group.Description);

			var retrievedGroup = await ItemGroupService.GetGroupByIdAsync(group.Id);

			Assert.IsNotNull(retrievedGroup);
			Assert.AreEqual(group.Name, retrievedGroup.Name);
			Assert.AreEqual(group.Description, retrievedGroup.Description);
		}

		[TestMethod]
		public async Task ShouldUpdateGroupAsync()
		{
			var groupToUpdate = await ItemGroupService.AddItemGroupAsync("Group to update.", "Another group.");
			var group = await ItemGroupService.UpdateItemGroupAsync(groupToUpdate.Id, "New name", "New description");

			Assert.IsNotNull(group);
			Assert.AreEqual(groupToUpdate.Id, group.Id);
			Assert.AreEqual("New name", group.Name);
			Assert.AreEqual("New description", group.Description);

			var retrievedGroup = await ItemGroupService.GetGroupByIdAsync(group.Id);

			Assert.IsNotNull(retrievedGroup);
			Assert.AreEqual(group.Name, retrievedGroup.Name);
			Assert.AreEqual(group.Description, retrievedGroup.Description);
		}

		[TestMethod]
		public async Task ShouldRemoveGroupAsync()
		{
			var groupToRemove = await ItemGroupService.AddItemGroupAsync("Group to remove.", "Bye bye group.");
			var group = await ItemGroupService.RemoveItemGroupAsync(groupToRemove.Id);

			Assert.IsNotNull(group);
			Assert.AreEqual(groupToRemove.Id, group.Id);
			Assert.AreEqual("Group to remove.", group.Name);
			Assert.AreEqual("Bye bye group.", group.Description);

			var retrievedGroup = await ItemGroupService.GetGroupByIdAsync(group.Id);
			Assert.IsNull(retrievedGroup);
		}

		[TestMethod]
		public async Task ShouldReturnItemsWithAllMatchingAttributes()
		{
			var group = await ItemGroupService.AddItemGroupAsync("FullGroup", "Group with items.");

			var attribute1 = await AttributeService.AddAttributeTypeAsync("AllMatchedAttribute1");
			var attribute2 = await AttributeService.AddAttributeTypeAsync("AllMatchedAttribute2");
			var attribute3 = await AttributeService.AddAttributeTypeAsync("AllUnmatchedAttribute1");

			group = await AttributeService.AssignAttributeToGroupAsync(group.Id, attribute1.Id, "Value1");
			group = await AttributeService.AssignAttributeToGroupAsync(group.Id, attribute2.Id, "Value2");

			var item1 = await ItemService.AddItemAsync("FirstMatchedItem", "First matched item.");
			var item2 = await ItemService.AddItemAsync("SecondMatchedItem", "Second matched item.");

			item1 = await AttributeService.AssignAttributeToItemAsync(item1.Id, attribute1.Id, "Value1");
			item1 = await AttributeService.AssignAttributeToItemAsync(item1.Id, attribute2.Id, "Value2");

			item2 = await AttributeService.AssignAttributeToItemAsync(item2.Id, attribute1.Id, "Value1");
			item2 = await AttributeService.AssignAttributeToItemAsync(item2.Id, attribute2.Id, "Value2");
			item2 = await AttributeService.AssignAttributeToItemAsync(item2.Id, attribute3.Id, "Value3");

			var matches = await ItemGroupService.GetGroupItemsAsync(group.Id, false);

			Assert.AreEqual(1, matches.Count());
			Assert.AreEqual(matches.First().Id, item1.Id);
			Assert.AreNotEqual(matches.First().Id, item2.Id);
		}

		[TestMethod]
		public async Task ShouldReturnItemsWithSupersetOfMatchingAttributes()
		{
			var group = await ItemGroupService.AddItemGroupAsync("FullGroup", "Group with items.");

			var attribute1 = await AttributeService.AddAttributeTypeAsync("MatchedAttribute1");
			var attribute2 = await AttributeService.AddAttributeTypeAsync("MatchedAttribute2");
			var attribute3 = await AttributeService.AddAttributeTypeAsync("UnmatchedAttribute1");

			group = await AttributeService.AssignAttributeToGroupAsync(group.Id, attribute1.Id, "Value1");
			group = await AttributeService.AssignAttributeToGroupAsync(group.Id, attribute2.Id, "Value2");

			var item1 = await ItemService.AddItemAsync("FirstMatchedItem", "First matched item.");
			var item2 = await ItemService.AddItemAsync("SecondMatchedItem", "Second matched item.");

			item1 = await AttributeService.AssignAttributeToItemAsync(item1.Id, attribute1.Id, "Value1");
			item1 = await AttributeService.AssignAttributeToItemAsync(item1.Id, attribute2.Id, "Value2");

			item2 = await AttributeService.AssignAttributeToItemAsync(item2.Id, attribute1.Id, "Value1");
			item2 = await AttributeService.AssignAttributeToItemAsync(item2.Id, attribute2.Id, "Value2");
			item2 = await AttributeService.AssignAttributeToItemAsync(item2.Id, attribute3.Id, "Value3");

			var matches = await ItemGroupService.GetGroupItemsAsync(group.Id);

			Assert.AreEqual(2, matches.Count());
			Assert.AreEqual(2, matches.First().Attributes.Aggregate(0, (acc, attr) =>
			{
				if ((attr.AttributeId == attribute1.Id && attr.Value == "Value1") || (attr.AttributeId == attribute2.Id && attr.Value == "Value2")) return acc + 1;
				return acc;
			}));
			Assert.AreEqual(2, matches.Last().Attributes.Aggregate(0, (acc, attr) =>
			{
				if ((attr.AttributeId == attribute1.Id && attr.Value == "Value1") || (attr.AttributeId == attribute2.Id && attr.Value == "Value2")) return acc + 1;
				return acc;
			}));
		}

		[TestMethod]
		public async Task ShouldNotReturnItemsWithAnyNonMatchingAttributes()
		{
			var group = await ItemGroupService.AddItemGroupAsync("FullGroup", "Group with items.");

			var attribute1 = await AttributeService.AddAttributeTypeAsync("RandomAttribute1");
			var attribute2 = await AttributeService.AddAttributeTypeAsync("RandomAttribute2");
			var attribute3 = await AttributeService.AddAttributeTypeAsync("MaybeMatchedAttribute1");

			group = await AttributeService.AssignAttributeToGroupAsync(group.Id, attribute1.Id, "Value1");
			group = await AttributeService.AssignAttributeToGroupAsync(group.Id, attribute2.Id, "Value2");
			group = await AttributeService.AssignAttributeToGroupAsync(group.Id, attribute3.Id, "Value3");

			var item1 = await ItemService.AddItemAsync("FirstMatchedItem", "First matched item.");
			var item2 = await ItemService.AddItemAsync("SecondMatchedItem", "Second matched item.");

			item1 = await AttributeService.AssignAttributeToItemAsync(item1.Id, attribute1.Id, "Wub-a");
			item1 = await AttributeService.AssignAttributeToItemAsync(item1.Id, attribute2.Id, "Lub-a");

			item2 = await AttributeService.AssignAttributeToItemAsync(item2.Id, attribute1.Id, "Dub");
			item2 = await AttributeService.AssignAttributeToItemAsync(item2.Id, attribute2.Id, "Dub");
			item2 = await AttributeService.AssignAttributeToItemAsync(item2.Id, attribute3.Id, "Value3");

			var matches = await ItemGroupService.GetGroupItemsAsync(group.Id);

			Assert.AreEqual(0, matches.Count());
		}

		[TestMethod]
		public async Task ShouldThrowArgumentNullException()
		{
			ArgumentNullException ex = null;

			try { await ItemGroupService.AddItemGroupAsync(null, "Description"); }
			catch (Exception e) { ex = e as ArgumentNullException; }

			Assert.IsNotNull(ex);

			var item = await ItemGroupService.AddItemGroupAsync("Throw On Update", "Should throw exception on update.");
			ex = null;

			try { await ItemGroupService.UpdateItemGroupAsync(item.Id, null, item.Description); }
			catch (Exception e) { ex = e as ArgumentNullException; }

			Assert.IsNotNull(ex);
		}

		[TestMethod]
		public async Task ShouldThrowItemNotFoundException()
		{
			ItemGroupNotFoundException ex = null;

			try { await ItemGroupService.UpdateItemGroupAsync(-1, "Hello", "World"); }
			catch (Exception e) { ex = e as ItemGroupNotFoundException; }

			Assert.IsNotNull(ex);

			try { await ItemGroupService.RemoveItemGroupAsync(-1); }
			catch (Exception e) { ex = e as ItemGroupNotFoundException; }

			Assert.IsNotNull(ex);
		}
	}
}
