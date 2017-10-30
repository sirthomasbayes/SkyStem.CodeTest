using Skystem.Challenge.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skystem.Challenge.Core.Services
{
	public interface IAttributeService
	{
		/// <summary>
		/// Returns AttributeType with Id = id. Returns null if AttributeType does not exist.
		/// </summary>
		/// <param name="id">Id of AttributeType</param>
		/// <returns>AttributeType</returns>
		Task<AttributeType> GetAttributeByIdAsync(Int32 id);

		/// <summary>
		/// Returns AttributeType with Name = name. Returns null if AttributeType does not exist.
		/// </summary>
		/// <param name="name">Name of AttributeType.</param>
		/// <returns>AttributeType</returns>
		Task<AttributeType> GetAttributeByNameAsync(String name);

		/// <summary>
		/// Returns all AttributeTypes. 
		/// </summary>
		/// <param name="pageResults">If true, will page results.</param>
		/// <param name="page">If pageResults = true, returns page specified by this parameter.</param>
		/// <param name="pageSize">If pageResults = true, returns pageSize number of elements.</param>
		/// <returns>IEnumerable[AttributeType] | PagedResult[AttributeType]</returns>
		Task<IEnumerable<AttributeType>> GetAttributesAsync(Boolean pageResults = false, Int32 page = 1, Int32 pageSize = 15);

		/// <summary>
		/// Adds new AttributeType with Name = attributeName. Will throw exception if AttributeType with Name = attributeName already exists.
		/// </summary>
		/// <param name="attributeName">Name of new AttributeType. Will throw ArgumentNullException if this is null or whitespace.</param>
		/// <returns>AttributeType</returns>
		Task<AttributeType> AddAttributeTypeAsync(String attributeName);

		/// <summary>
		/// Assigns attribute with Value value to Item item.
		/// </summary>
		/// <param name="itemId">Id of Item to assign attribute.</param>
		/// <param name="attributeId">Id of Attribute to assign.</param>
		/// <param name="value">Value of attribute.</param>
		/// <returns>Item with new ItemAttribute</returns>
		Task<Item> AssignAttributeToItemAsync(Int32 itemId, Int32 attributeId, String value);

		/// <summary>
		/// Assigns attribute with Value value to ItemGroup itemGroup.
		/// </summary>
		/// <param name="itemGroupId">Id of ItemGroup to assign attribute.</param>
		/// <param name="attributeId">Id of Attribute to assign.</param>
		/// <param name="value">Value of attribute.</param>
		/// <returns>Item with new ItemGroupAttribute</returns>
		Task<ItemGroup> AssignAttributeToGroupAsync(Int32 itemGroupId, Int32 attributeId, String value);
	}
}
