using Skystem.Challenge.App.Models;
using Skystem.Challenge.Core.Exceptions;
using Skystem.Challenge.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Skystem.Challenge.App.Controllers
{
	[RoutePrefix("api/groups")]
	public class ItemGroupApiController : ApiController
	{
		public ItemGroupApiController(IItemGroupService itemGroupService)
		{
			ItemGroupService = itemGroupService;
		}

		private IItemGroupService ItemGroupService;

		[HttpGet]
		[Route("{id}")]
		public async Task<IHttpActionResult> GetGroupByIdAsync([FromUri]Int32 id, [FromUri]Boolean includeGroupedItems = false)
		{
			try
			{
				var group = await ItemGroupService.GetGroupByIdAsync(id);
				if (group == null) throw new ItemGroupNotFoundException(id);

				if (false == includeGroupedItems) return Ok(group);

				var items = await ItemGroupService.GetGroupItemsAsync(id);
				return Ok(new
				{
					Id = group.Id,
					Name = group.Name,
					Description = group.Description,
					Attributes = group.Attributes,
					Items = items
				});
			}
			catch (ItemGroupNotFoundException e) { return NotFound(); }
			catch (Exception e) { return InternalServerError(e); }
		}

		[HttpGet]
		[Route("")]
		public async Task<IHttpActionResult> GetGroupsAsync([FromUri]Boolean pageResults = false, [FromUri]Int32 page = 1, [FromUri]Int32 pageSize = 15)
		{
			try
			{
				var items = await ItemGroupService.GetGroupsAsync(pageResults, page, pageSize);
				return Ok(items);
			}
			catch (Exception e) { return InternalServerError(e); }
		}

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
