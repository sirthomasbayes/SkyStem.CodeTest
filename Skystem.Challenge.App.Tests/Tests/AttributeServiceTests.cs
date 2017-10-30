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
	public class AttributeEFServiceTests
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
		public async Task ShouldAddAttributeTypeToDatabaseAsync()
		{
			var attribute = await AttributeService.AddAttributeTypeAsync("FirstAttribute");

			Assert.IsNotNull(attribute);
			Assert.AreNotEqual(0, attribute.Id);
			Assert.AreEqual("FirstAttribute", attribute.Name);

			var retrievedAttribute = await AttributeService.GetAttributeByNameAsync("FirstAttribute");

			Assert.AreEqual(attribute.Id, retrievedAttribute.Id);
			Assert.AreEqual(attribute.Name, retrievedAttribute.Name);

			retrievedAttribute = await AttributeService.GetAttributeByIdAsync(attribute.Id);

			Assert.AreEqual(attribute.Id, retrievedAttribute.Id);
			Assert.AreEqual(attribute.Name, retrievedAttribute.Name);
		}

		[TestMethod]
		public async Task ShouldAddAttributeToItemAsync()
		{
			var attribute = await AttributeService.AddAttributeTypeAsync("NewItemAttribute");
			var item = await ItemService.AddItemAsync("Attribute Test", "Will have attribute.");

			var assignedItem = await AttributeService.AssignAttributeToItemAsync(item.Id, attribute.Id, "Attribute Value!");

			Assert.AreEqual(item.Id, assignedItem.Id);
			Assert.AreEqual(1, assignedItem.Attributes.Count());

			var itemAttribute = assignedItem.Attributes.First();

			Assert.AreEqual(attribute.Id, itemAttribute.AttributeId);
			Assert.AreEqual("NewItemAttribute", itemAttribute.Name);
			Assert.AreEqual("Attribute Value!", itemAttribute.Value);

			var retrievedItem = await ItemService.GetItemByIdAsync(item.Id);

			itemAttribute = retrievedItem.Attributes.First();

			Assert.AreEqual(attribute.Id, itemAttribute.AttributeId);
			Assert.AreEqual("NewItemAttribute", itemAttribute.Name);
			Assert.AreEqual("Attribute Value!", itemAttribute.Value);
		}

		[TestMethod]
		public async Task ShouldUpdateAttributeOnItemAsync()
		{
			var attribute = await AttributeService.AddAttributeTypeAsync("UpdateItemAttribute");
			var item = await ItemService.AddItemAsync("Update Attribute Test", "Will have updated attribute.");

			var assignedItem = await AttributeService.AssignAttributeToItemAsync(item.Id, attribute.Id, "Attribute Value!");
			var updatedItem = await AttributeService.AssignAttributeToItemAsync(item.Id, attribute.Id, "New Value!");

			Assert.AreEqual(item.Id, assignedItem.Id);
			Assert.AreEqual(1, assignedItem.Attributes.Count());

			var itemAttribute = updatedItem.Attributes.First();

			Assert.AreEqual(attribute.Id, itemAttribute.AttributeId);
			Assert.AreEqual("UpdateItemAttribute", itemAttribute.Name);
			Assert.AreEqual("New Value!", itemAttribute.Value);

			var retrievedItem = await ItemService.GetItemByIdAsync(item.Id);

			itemAttribute = retrievedItem.Attributes.First();

			Assert.AreEqual(attribute.Id, itemAttribute.AttributeId);
			Assert.AreEqual("UpdateItemAttribute", itemAttribute.Name);
			Assert.AreEqual("New Value!", itemAttribute.Value);
		}

		[TestMethod]
		public async Task ShouldAddAttributeToGroupAsync()
		{
			var attribute = await AttributeService.AddAttributeTypeAsync("NewGroupAttribute");
			var item = await ItemGroupService.AddItemGroupAsync("Attribute Test", "Will have attribute.");

			var assignedItem = await AttributeService.AssignAttributeToGroupAsync(item.Id, attribute.Id, "Attribute Value!");
			var test = assignedItem.Attributes.ToList();

			Assert.AreEqual(item.Id, assignedItem.Id);
			Assert.AreEqual(1, assignedItem.Attributes.Count());

			var itemAttribute = assignedItem.Attributes.First();

			Assert.AreEqual(attribute.Id, itemAttribute.AttributeId);
			Assert.AreEqual("NewGroupAttribute", itemAttribute.Name);
			Assert.AreEqual("Attribute Value!", itemAttribute.Value);

			var retrievedItem = await ItemGroupService.GetGroupByIdAsync(item.Id);

			itemAttribute = assignedItem.Attributes.First();

			Assert.AreEqual(attribute.Id, itemAttribute.AttributeId);
			Assert.AreEqual("NewGroupAttribute", itemAttribute.Name);
			Assert.AreEqual("Attribute Value!", itemAttribute.Value);
		}

		[TestMethod]
		public async Task ShouldUpdateAttributeOnGroupAsync()
		{
			var attribute = await AttributeService.AddAttributeTypeAsync("UpdateGroupAttribute");
			var item = await ItemGroupService.AddItemGroupAsync("Update Attribute Test", "Will have updated attribute.");

			var assignedItem = await AttributeService.AssignAttributeToGroupAsync(item.Id, attribute.Id, "Attribute Value!");
			var updatedItem = await AttributeService.AssignAttributeToGroupAsync(item.Id, attribute.Id, "New Value!");

			Assert.AreEqual(item.Id, assignedItem.Id);
			Assert.AreEqual(1, assignedItem.Attributes.Count());

			var itemAttribute = updatedItem.Attributes.First();

			Assert.AreEqual(attribute.Id, itemAttribute.AttributeId);
			Assert.AreEqual("UpdateGroupAttribute", itemAttribute.Name);
			Assert.AreEqual("New Value!", itemAttribute.Value);

			var retrievedItem = await ItemGroupService.GetGroupByIdAsync(item.Id);

			itemAttribute = assignedItem.Attributes.First();

			Assert.AreEqual(attribute.Id, itemAttribute.AttributeId);
			Assert.AreEqual("UpdateGroupAttribute", itemAttribute.Name);
			Assert.AreEqual("Attribute Value!", itemAttribute.Value);
		}

		[TestMethod]
		public async Task ShouldThrowDuplicateAttributeTypeExceptionAsync()
		{
			var attribute = await AttributeService.AddAttributeTypeAsync("DuplicateTest");
			DuplicateAttributeTypeException ex = null;

			try { await AttributeService.AddAttributeTypeAsync("DuplicateTest"); }
			catch (Exception e) { ex = e as DuplicateAttributeTypeException; }

			Assert.IsNotNull(ex);
		}

		[TestMethod]
		public async Task ShouldThrowItemNotFoundExceptionAsync()
		{
			var exceptionAttribute = await AttributeService.AddAttributeTypeAsync("ItemNotFound");
			ItemNotFoundException ex = null;

			try { await AttributeService.AssignAttributeToItemAsync(-1, exceptionAttribute.Id, "Hello"); }
			catch (Exception e) { ex = e as ItemNotFoundException; }

			Assert.IsNotNull(ex);
		}

		[TestMethod]
		public async Task ShouldThrowItemGroupNotFoundExceptionAsync()
		{
			var exceptionAttribute = await AttributeService.AddAttributeTypeAsync("ItemGroupNotFound");
			ItemGroupNotFoundException ex = null;

			try { await AttributeService.AssignAttributeToGroupAsync(-1, exceptionAttribute.Id, "Hello"); }
			catch (Exception e) { ex = e as ItemGroupNotFoundException; }

			Assert.IsNotNull(ex);
		}

		[TestMethod]
		public async Task ShouldThrowAttributeTypeNotFoundExceptionAsync()
		{
			var exceptionItem = await ItemService.AddItemAsync("ExceptionItem", null);
			AttributeTypeNotFoundException ex = null;

			try { await AttributeService.AssignAttributeToItemAsync(exceptionItem.Id, -1, "blah"); }
			catch (Exception e) { ex = e as AttributeTypeNotFoundException; }

			Assert.IsNotNull(ex);

			var exceptionGroup = await ItemGroupService.AddItemGroupAsync("ExceptionGroup", null);

			try { await AttributeService.AssignAttributeToGroupAsync(exceptionGroup.Id, -1, "blah"); }
			catch (Exception e) { ex = e as AttributeTypeNotFoundException; }

			Assert.IsNotNull(ex);
		}
	}
}
