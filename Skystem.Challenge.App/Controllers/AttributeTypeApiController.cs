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
	/// API endpoints for managing AttributeTypes and assigning
	/// attributes to Items and ItemGroups.
	/// </summary>
	[RoutePrefix("api/attributes")]
    public class AttributeTypeApiController : ApiController
    {
		/// <summary>
		/// API endpoints for managing AttributeTypes and assigning
		/// attributes to Items and ItemGroups.
		/// </summary>
		/// <param name="attributeService"></param>
		public AttributeTypeApiController(IAttributeService attributeService)
		{
			AttributeService = attributeService;
		}

		private IAttributeService AttributeService;

		/// <summary>
		/// Gets AttributeType by Id.
		/// </summary>
		/// <param name="id">Id of AttributeType</param>
		/// <returns>200 - AttributeType | 404 - null | 500 - error</returns>
		[HttpGet]
		[Route("{id}")]
		public async Task<IHttpActionResult> GetAttributeByIdAsync([FromUri]Int32 id)
		{
			try
			{
				var attribute = await AttributeService.GetAttributeByIdAsync(id);
				return Ok(attribute);
			}
			catch (AttributeTypeNotFoundException e) { return NotFound(); }
			catch (Exception e) { return InternalServerError(e); }
		}

		/// <summary>
		/// Retrieves list of all AttributeTypes.
		/// </summary>
		/// <param name="pageResults">If true, pages the list.</param>
		/// <param name="page">If pageResults = true, returns specified page.</param>
		/// <param name="pageSize">If pageResults = true, returns specified number of items.</param>
		/// <returns>200 - IEnumerable[AttributeType] | 200 - PagedResult[AttributeType] | 500 - error</returns>
		[HttpGet]
		[Route("")]
		public async Task<IHttpActionResult> GetAttributesAsync([FromUri]Boolean pageResults = false, [FromUri]Int32 page = 1, [FromUri]Int32 pageSize = 15)
		{
			try
			{
				var attributes = await AttributeService.GetAttributesAsync(pageResults, page, pageSize);
				return Ok(attributes);
			}
			catch (Exception e) { return InternalServerError(e); }
		}

		/// <summary>
		/// Adds AttributeType with Name = name.
		/// </summary>
		/// <param name="name">Name of AttributeType</param>
		/// <returns>200 - AttributeType | 500 - error</returns>
		[HttpPut]
		[Route("")]
		public async Task<IHttpActionResult> AddAttributeAsync([FromUri]String name)
		{
			try
			{
				var attribute =
					await AttributeService.GetAttributeByNameAsync(name) ??
					await AttributeService.AddAttributeTypeAsync(name);

				return Ok(attribute);
			}
			catch (DuplicateAttributeTypeException e) { return await AddAttributeAsync(name); }	// if another thread adds an AttributeType with Name = name before us,
																								// just retry, next call should succeed.
			catch (Exception e) { return InternalServerError(e); }
		}

		/// <summary>
		/// Assigns AttributeType with Id = id to Item Id = itemId
		/// </summary>
		/// <param name="id">Id of AttributeType</param>
		/// <param name="itemId">Id of Item</param>
		/// <param name="value">Value of attribute</param>
		/// <returns>200 - Item with new Attribute</returns>
		[HttpPut]
		[Route("{id}/assign-to/item/{itemId}")]
		public async Task<IHttpActionResult> AssignToItemAsync([FromUri]Int32 id, [FromUri]Int32 itemId, [FromUri]String value)
		{
			try
			{
				var item = await AttributeService.AssignAttributeToItemAsync(itemId, id, value);
				return Ok(item);
			}
			catch (AttributeTypeNotFoundException e) { return NotFound(); }
			catch (ItemNotFoundException e) { return NotFound(); }
			catch (Exception e) { return InternalServerError(e); }
		}

		/// <summary>
		/// Assigns AttributeType with Id = id to Group Id = groupId
		/// </summary>
		/// <param name="id">Id of AttributeType</param>
		/// <param name="groupId">Id of Group</param>
		/// <param name="value">Value of attribute</param>
		/// <returns>200 - ItemGroup with new Attribute</returns>
		[HttpPut]
		[Route("{id}/assign-to/group/{groupId}")]
		public async Task<IHttpActionResult> AssignToGroupAsync([FromUri]Int32 id, [FromUri]Int32 groupId, [FromUri]String value)
		{
			try
			{
				var group = await AttributeService.AssignAttributeToGroupAsync(groupId, id, value);
				return Ok(group);
			}
			catch (AttributeTypeNotFoundException e) { return NotFound(); }
			catch (ItemGroupNotFoundException e) { return NotFound(); }
			catch (Exception e) { return InternalServerError(e); }
		}
	}
}
