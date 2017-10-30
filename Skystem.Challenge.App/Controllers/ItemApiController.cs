using Skystem.Challenge.App.Models;
using Skystem.Challenge.App.Utilities;
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
	/// <summary>
	/// API for managing Items
	/// </summary>
	[RoutePrefix("api/items")]
    public class ItemApiController : ApiController
    {
		/// <summary>
		/// API for managing Items
		/// </summary>
		/// <param name="itemService"></param>
		public ItemApiController(IItemService itemService)
		{
			ItemService = itemService;
		}

		private IItemService ItemService;

		/// <summary>
		/// Returns Item with Id = id.
		/// </summary>
		/// <param name="id">Id of Item</param>
		/// <returns>200 - Item | 404 - null | 500 - error</returns>
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

		/// <summary>
		/// Retrieves list of all Items.
		/// </summary>
		/// <param name="pageResults">If true, pages the list.</param>
		/// <param name="page">If pageResults = true, returns specified page.</param>
		/// <param name="pageSize">If pageResults = true, returns specified number of items.</param>
		/// <returns>200 - IEnumerable[Item] | 200 - PagedResult[Items] when pageResults = true | 500 - error</returns>
		[HttpGet]
		[Route("")]
		public async Task<IHttpActionResult> GetItemsAsync([FromUri]Boolean pageResults = false, [FromUri]Int32 page = 1, [FromUri]Int32 pageSize = 15)
		{
			try
			{
				var items = await ItemService.GetItemsAsync(pageResults, page, pageSize);
				return Ok(items.GetHttpReturnValue());
			}
			catch (Exception e) { return InternalServerError(e); }
		}

		/// <summary>
		/// Adds new Item
		/// </summary>
		/// <param name="model">model.Name = name of Item | model.Description = description of Item</param>
		/// <returns>200 - Item | 500 - error</returns>
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

		/// <summary>
		/// Updates Item with Id = id
		/// </summary>
		/// <param name="id">Id of item</param>
		/// <param name="model">model.Name = name of Item | model.Description = description of Item</param>
		/// <returns>200 - Item | 404 - ItemNotFoundException | 500 - other error</returns>
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

		/// <summary>
		/// Deletes Item with Id = id
		/// </summary>
		/// <param name="id">Id of Item</param>
		/// <returns>200 - Item | 404 - ItemNotFoundException | 500 - other error</returns>
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
