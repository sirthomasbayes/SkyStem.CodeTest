using Skystem.Challenge.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skystem.Challenge.Core.Services
{
	public interface IItemService
	{
		/// <summary>
		/// Returns Item with Id = id. Returns null if Item does not exist.
		/// </summary>
		/// <param name="id">Id of Item.</param>
		/// <returns>Item</returns>
		Task<Item> GetItemByIdAsync(Int32 id);

		/// <summary>
		/// Returns all Items.
		/// </summary>
		/// <param name="pageResults">If pageResults = true, pages list of Items.</param>
		/// <param name="page">If pageResults = true, specifies page to return.</param>
		/// <param name="pageSize">If pageResults = true, specifies number of items to return.</param>
		/// <returns>IEnumerable[Item] | PagedResult[Item]</returns>
		Task<IEnumerable<Item>> GetItemsAsync(Boolean pageResults = false, Int32 page = 1, Int32 pageSize = 15);

		/// <summary>
		/// Adds Item with Name = name, Description = description.
		/// </summary>
		/// <param name="name">Name of Item. Will throw ArgumentNullException if this is null or whitespace.</param>
		/// <param name="description">Description of Item.</param>
		/// <returns>Item</returns>
		Task<Item> AddItemAsync(String name, String description);

		/// <summary>
		/// Updates Name and Description of Item with Id = id.
		/// </summary>
		/// <param name="id">Id of Item. If Item does not exist, will throw ItemNotFoundException.</param>
		/// <param name="name">Name to assign Item. Will throw ArgumentNullException if this is null or whitespace.</param>
		/// <param name="description">Description to assign Item.</param>
		/// <returns>Item</returns>
		Task<Item> UpdateItemAsync(Int32 id, String name, String description);

		/// <summary>
		/// Removes Item with Id = id.
		/// </summary>
		/// <param name="id">Id of Item to remove. Will throw ItemNotFoundException if Item does not exist.</param>
		/// <returns>Item</returns>
		Task<Item> RemoveItemAsync(Int32 id);
	}
}
