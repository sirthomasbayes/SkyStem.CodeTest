using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skystem.Challenge.Core.Utilities
{
	public interface IMappableTo<T>
	{
		T Map();
	}
}
