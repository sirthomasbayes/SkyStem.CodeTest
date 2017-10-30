using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skystem.Challenge.Service.Migrations;
using Skystem.Challenge.Core.Services;
using Skystem.Challenge.Service;
using System.Threading.Tasks;
using Skystem.Challenge.Core.Exceptions;

namespace Skystem.Challenge.App.Tests
{
	[TestClass]
	public class ItemEFServiceTests
	{
		private IDbSeeder DbMigrator = IoCContainer.DefaultContainer.GetInstance<IDbSeeder>();
		private IItemService ItemService = IoCContainer.DefaultContainer.GetInstance<IItemService>();

		[TestInitialize]
		public void Initialize()
		{
			DbMigrator.FlushAsync().Wait();
		}

		[TestMethod]
		public async Task ShouldAddItemToDatabaseAsync()
		{
			var item = await ItemService.AddItemAsync("First Test", "My first item");

			Assert.IsNotNull(item);
			Assert.AreNotEqual(0, item.Id);
			Assert.AreEqual("First Test", item.Name);
			Assert.AreEqual("My first item", item.Description);

			var retrievedItem = await ItemService.GetItemByIdAsync(item.Id);

			Assert.IsNotNull(retrievedItem);
			Assert.AreEqual(item.Id, retrievedItem.Id);
			Assert.AreEqual(item.Name, retrievedItem.Name);
			Assert.AreEqual(item.Description, retrievedItem.Description);
		}

		[TestMethod]
		public async Task ShouldUpdateDatabaseItemAsync()
		{
			var addedItem = await ItemService.AddItemAsync("Second Test", "My second item");
			var item = await ItemService.UpdateItemAsync(addedItem.Id, "New Name", "My updated item");

			Assert.IsNotNull(item);
			Assert.AreEqual(addedItem.Id, item.Id);
			Assert.AreEqual("New Name", item.Name);
			Assert.AreEqual("My updated item", item.Description);

			var retrievedItem = await ItemService.GetItemByIdAsync(item.Id);

			Assert.IsNotNull(retrievedItem);
			Assert.AreEqual(item.Id, retrievedItem.Id);
			Assert.AreEqual(item.Name, retrievedItem.Name);
			Assert.AreEqual(item.Description, retrievedItem.Description);
		}

		[TestMethod]
		public async Task RemoveItemFromDatabaseAsync()
		{
			var addedItem = await ItemService.AddItemAsync("Third Test", "Should remove this one");
			var item = await ItemService.RemoveItemAsync(addedItem.Id);

			Assert.IsNotNull(item);
			Assert.AreEqual(addedItem.Id, item.Id);
			Assert.AreEqual("Third Test", item.Name);
			Assert.AreEqual("Should remove this one", item.Description);

			var retrievedItem = await ItemService.GetItemByIdAsync(item.Id);

			Assert.IsNull(retrievedItem);
		}

		[TestMethod]
		public async Task ShouldThrowArgumentNullException()
		{
			ArgumentNullException ex = null;

			try { await ItemService.AddItemAsync(null, "Description"); }
			catch (Exception e) { ex = e as ArgumentNullException; }

			Assert.IsNotNull(ex);

			var item = await ItemService.AddItemAsync("Throw On Update", "Should throw exception on update.");
			ex = null;

			try { await ItemService.UpdateItemAsync(item.Id, null, item.Description); }
			catch (Exception e) { ex = e as ArgumentNullException; }

			Assert.IsNotNull(ex);
		}

		[TestMethod]
		public async Task ShouldThrowItemNotFoundException()
		{
			ItemNotFoundException ex = null;

			try { await ItemService.UpdateItemAsync(-1, "Hello", "World"); }
			catch (Exception e) { ex = e as ItemNotFoundException; }

			Assert.IsNotNull(ex);

			try { await ItemService.RemoveItemAsync(-1); }
			catch (Exception e) { ex = e as ItemNotFoundException; }

			Assert.IsNotNull(ex);
		}
	}
}
