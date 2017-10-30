using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skystem.Challenge.Service.lib
{
	internal class DbVersion
	{
		public DbVersion()
		{
			CreatedDate = DateTime.UtcNow;
		}

		[Key]
		public Int32 Version { get; set; }

		public String Description { get; set; }

		public DateTime CreatedDate { get; set; }
	}
}
