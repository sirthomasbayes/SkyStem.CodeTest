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
	[RoutePrefix("api/items")]
    public class ItemApiController : ApiController
    {
		public ItemApiController(IItemService itemService)
		{
			ItemService = itemService;
		}

		private IItemService ItemService;

		[HttpGet]
		[Route("{id}")]
		public async Task<IHttpActionResult> GetItemByIdAsync([FromUri]Int32 id)
		{
			try
			{
				var item = await ItemService.GetItemByIdAsync(id);
				if (item == null) throw new ItemNotFoundException(id);

				return Ok(item);
			}
			catch (ItemNotFoundException e) { return NotFound(); }
			catch (Exception e) { return InternalServerError(e); }
		}

		[HttpGet]
		[Route("")]
		public async Task<IHttpActionResult> GetItemsAsync([FromUri]Boolean pageResults = false, [FromUri]Int32 page = 1, [FromUri]Int32 pageSize = 15)
		{
			try
			{
				var items = await ItemService.GetItemsAsync(pageResults, page, pageSize);
				return Ok(items);
			}
			catch (Exception e) { return InternalServerError(e); }
		}

		[HttpPost]
		[Route("")]
		public async Task<IHttpActionResult> AddItemAsync([FromBody]ItemFormModel model)
		{
			try
			{
				var item = await ItemService.AddItemAsync(model.Name, model.Description);
				return Ok(item);
			}
			catch (Exception e) { return InternalServerError(e); }
		}

		[HttpPut]
		[Route("{id}")]
		public async Task<IHttpActionResult> UpdateItemAsync([FromUri]Int32 id, [FromBody]ItemFormModel model)
		{
			try
			{
				var item = await ItemService.UpdateItemAsync(id, model.Name, model.Description);
				return Ok(item);
			}
			catch (ItemNotFoundException e) { return NotFound(); }
			catch (Exception e) { return InternalServerError(e); }
		}

		[HttpDelete]
		[Route("{id}")]
		public async Task<IHttpActionResult> RemoveItemAsync([FromUri]Int32 id)
		{
			try
			{
				var item = await ItemService.RemoveItemAsync(id);
				return Ok(item);
			}
			catch (ItemNotFoundException e) { return NotFound(); }
			catch (Exception e) { return InternalServerError(e); }
		}
    }
}
