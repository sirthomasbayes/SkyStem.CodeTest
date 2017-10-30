using Skystem.Challenge.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Skystem.Challenge.App.Utilities
{
	public static class IEnumerableExtensions
	{
		public static Object GetHttpReturnValue<T>(this IEnumerable<T> items)
		{
			var pagedItems = items as PagedResult<T>;
			if (pagedItems == null) return items;

			return new
			{
				Page = pagedItems.Page,
				PageSize = pagedItems.PageSize,
				MaxPage = pagedItems.MaxPage,
				Collection = pagedItems.Collection
			};
		}
	}
}