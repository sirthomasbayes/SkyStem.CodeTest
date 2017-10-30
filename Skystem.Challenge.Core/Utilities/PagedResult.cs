using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skystem.Challenge.Core.Utilities
{
	public class PagedResult<T> : IEnumerable<T>
	{
		public PagedResult(Int32 page, Int32 pageSize, Int32 maxPage, IEnumerable<T> collection)
		{
			Assert.IsNotNull(collection);

			Page = page;
			PageSize = pageSize;
			MaxPage = maxPage;
			Collection = collection;
		}

		public Int32 Page { get; internal set; }

		public Int32 PageSize { get; internal set; }

		public Int32 MaxPage { get; internal set; }

		public IEnumerable<T> Collection { get; internal set; }

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return Collection.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Collection.GetEnumerator();
		}
	}
}
