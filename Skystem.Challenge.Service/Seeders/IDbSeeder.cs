using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skystem.Challenge.Service.Migrations
{
	/// <summary>
	/// Module used to run database migrations. 
	/// </summary>
	public interface IDbSeeder
	{
		/// <summary>
		/// Seeds Database with set of test values.
		/// </summary>
		/// <returns></returns>
		Task SeedAsync();

		/// <summary>
		/// Removes all items from database.
		/// </summary>
		/// <returns></returns>
		Task FlushAsync();
	}
}
