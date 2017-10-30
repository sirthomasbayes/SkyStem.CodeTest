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
	[RoutePrefix("api/attributes")]
    public class AttributeTypeApiController : ApiController
    {
		public AttributeTypeApiController(IAttributeService attributeService)
		{
			AttributeService = attributeService;
		}

		private IAttributeService AttributeService;

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
			catch (Exception e) { return InternalServerError(e); }
		}

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
