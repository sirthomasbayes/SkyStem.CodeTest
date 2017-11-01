using Skystem.Challenge.App.Models;
using Skystem.Challenge.App.Utilities;
using Skystem.Challenge.Core.Entities;
using Skystem.Challenge.Core.Exceptions;
using Skystem.Challenge.Core.Services;
using Skystem.Challenge.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Skystem.Challenge.App.Controllers
{
	/// <summary>
	/// API for managing ItemGroups
	/// </summary>
	[RoutePrefix("api/groups")]
	public class ItemGroupApiController : ApiController
	{
		/// <summary>
		/// API for managing ItemGroups
		/// </summary>
		/// <param name="itemGroupService"></param>
		public ItemGroupApiController(IItemGroupService itemGroupService)
		{
			ItemGroupService = itemGroupService;
		}

		private IItemGroupService ItemGroupService;

		/// <summary>
		/// Returns Group with Id = id
		/// </summary>
		/// <param name="id">Id of ItemGroup</param>
		/// <param name="includeGroupedItems">If true, returns all Items whose Attributes are a superset of the Group's Attributes.</param>
		/// <param name="matchSupersets">If set to true, will group Items whose Attributes are a superset of ItemGroup id's attributes.</param>
		/// <returns></returns>
		[HttpGet]
		[Route("{id}")]
		public async Task<IHttpActionResult> GetGroupByIdAsync([FromUri]Int32 id, [FromUri]Boolean includeGroupedItems = false, [FromUri]Boolean matchSupersets = true)
		{
			try
			{
				var group = await ItemGroupService.GetGroupByIdAsync(id);
				if (group == null) throw new ItemGroupNotFoundException(id);

				if (false == includeGroupedItems) return Ok(group);

				var items = await ItemGroupService.GetGroupItemsAsync(id, matchSupersets);
				return Ok(new HydratedItemGroup(group.Id, group.Name, group.Description, group.Attributes, items));
			}
			catch (ItemGroupNotFoundException e) { return NotFound(); }
			catch (Exception e) { return InternalServerError(e); }
		}

		/// <summary>
		/// Retrieves list of all Items.
		/// </summary>
		/// <param name="pageResults">If true, pages the list.</param>
		/// <param name="page">If pageResults = true, returns specified page.</param>
		/// <param name="pageSize">If pageResults = true, returns specified number of items.</param>
		/// <param name="includeGroupedItems">If true, returns all Items whose Attributes are a superset of the Group's Attributess.</param>
		/// <param name="matchSupersets">If set to true, will group Items whose Attributes are a superset of ItemGroup id's attributes.</param>
		/// <returns>200 - IEnumerable[Item] | 200 - PagedResult[ItemGroup] when pageResults = true | 500 - error</returns>
		[HttpGet]
		[Route("")]
		public async Task<IHttpActionResult> GetGroupsAsync([FromUri]Boolean pageResults = false, [FromUri]Int32 page = 1, [FromUri]Int32 pageSize = 15, [FromUri]Boolean includeGroupedItems = false, [FromUri]Boolean matchSupersets = true)
		{
			try
			{
				var groups = await ItemGroupService.GetGroupsAsync(pageResults, page, pageSize);
				if (false == includeGroupedItems) return Ok(groups.GetHttpReturnValue());

				var pageData = groups as PagedResult<ItemGroup>;
				var groupCollection = pageResults ? pageData.Collection : groups;

				var tasks = groupCollection.Select(x => ItemGroupService.GetGroupItemsAsync(x.Id, matchSupersets)).ToArray();
				var readyTasks = await Task.WhenAll(tasks);

				var hydratedGroups = groupCollection.Zip(readyTasks, (group, items) => new HydratedItemGroup(group.Id, group.Name, group.Description, group.Attributes, items));
				var result = pageResults ?
					new PagedResult<HydratedItemGroup>(pageData.Page, pageData.PageSize, pageData.MaxPage, hydratedGroups) :
					hydratedGroups;

				return Ok(result.GetHttpReturnValue());
			}
			catch (Exception e) { return InternalServerError(e); }
		}

		/// <summary>
		/// Adds new ItemGroup
		/// </summary>
		/// <param name="model">model.Name = ItemGroup's name | model.Description = ItemGroup's description</param>
		/// <returns>200 - ItemGroup | 500 - error</returns>
		[HttpPost]
		[Route("")]
		public async Task<IHttpActionResult> AddGroupAsync([FromBody]ItemGroupFormModel model)
		{
			try
			{
				var item = await ItemGroupService.AddItemGroupAsync(model.Name, model.Description);
				return Ok(item);
			}
			catch (Exception e) { return InternalServerError(e); }
		}

		/// <summary>
		/// Updates ItemGroup with Id = id
		/// </summary>
		/// <param name="id">Id of ItemGroup</param>
		/// <param name="model">model.Name = ItemGroup's name | model.Description = ItemGroup's description</param>
		/// <returns>200 - ItemGroup | 404 - ItemGroupNotFoundException | 500 - error</returns>
		[HttpPut]
		[Route("{id}")]
		public async Task<IHttpActionResult> UpdateGroupAsync([FromUri]Int32 id, [FromBody]ItemGroupFormModel model)
		{
			try
			{
				var item = await ItemGroupService.UpdateItemGroupAsync(id, model.Name, model.Description);
				return Ok(item);
			}
			catch (ItemGroupNotFoundException e) { return NotFound(); }
			catch (Exception e) { return InternalServerError(e); }
		}

		/// <summary>
		/// Deletes ItemGroup with Id = id
		/// </summary>
		/// <param name="id">Id of ItemGroup</param>
		/// <returns>200 - ItemGroup | 404 - ItemGroupNotFoundException | 500 - error</returns>
		[HttpDelete]
		[Route("{id}")]
		public async Task<IHttpActionResult> RemoveGroupAsync([FromUri]Int32 id)
		{
			try
			{
				var item = await ItemGroupService.RemoveItemGroupAsync(id);
				return Ok(item);
			}
			catch (ItemGroupNotFoundException e) { return NotFound(); }
			catch (Exception e) { return InternalServerError(e); }
		}
    }
}
