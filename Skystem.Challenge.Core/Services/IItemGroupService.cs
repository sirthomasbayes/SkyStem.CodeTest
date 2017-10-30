using Skystem.Challenge.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skystem.Challenge.Core.Services
{
	public interface IItemGroupService
	{
		/// <summary>
		/// Returns ItemGroup with Id = id. Returns null if ItemGroup does not exist.
		/// </summary>
		/// <param name="id">Id of ItemGroup</param>
		/// <returns>ItemGroup</returns>
		Task<ItemGroup> GetGroupByIdAsync(Int32 id);

		/// <summary>
		/// Returns all ItemGroups.
		/// </summary>
		/// <param name="pageResults">If pageResults = true, pages list of ItemGroups.</param>
		/// <param name="page">If pageResults = true, specifies page to return.</param>
		/// <param name="pageSize">If pageResults = true, specifies number of items to return.</param>
		/// <returns>IEnumerable[ItemGroup] | PagedResult[ItemGroup]</returns>
		Task<IEnumerable<ItemGroup>> GetGroupsAsync(Boolean pageResults = false, Int32 page = 1, Int32 pageSize = 15);

		/// <summary>
		/// Returns all Items that belong to current ItemGroup.
		/// Items belong to ItemGroup only if an Item's Attributes is a superset (not strict) of the ItemGroup's attributes.
		/// </summary>
		/// <param name="id">Id of ItemGroup</param>
		/// <returns>IEnumerable[Item]</returns>
		Task<IEnumerable<Item>> GetGroupItemsAsync(Int32 id);

		/// <summary>
		/// Add ItemGroup with Name = name, Description = description
		/// </summary>
		/// <param name="name">Name of ItemGroup. Will throw ArgumentNullException if this is null or whitespace.</param>
		/// <param name="description">Description of ItemGroup.</param>
		/// <returns>ItemGroup</returns>
		Task<ItemGroup> AddItemGroupAsync(String name, String description);

		/// <summary>
		/// Updates Name and Description of ItemGroup with Id = id.
		/// </summary>
		/// <param name="id">Id of ItemGroup to change. Will throw ItemGroupNotFoundException if ItemGroup with Id = id does not exist.</param>
		/// <param name="name">Name to assign ItemGroup. Will throw ArgumentNullException if this is null or whitespace.</param>
		/// <param name="description">Description to assign ItemGroup.</param>
		/// <returns>ItemGroup</returns>
		Task<ItemGroup> UpdateItemGroupAsync(Int32 id, String name, String description);

		/// <summary>
		/// Removes ItemGroup with Id = id.
		/// </summary>
		/// <param name="id">Id of ItemGroup to remove. Will throw ItemGroupNotFoundException if ItemGroup with Id = id does not exist.</param>
		/// <returns>ItemGroup</returns>
		Task<ItemGroup> RemoveItemGroupAsync(Int32 id);
	}
}
