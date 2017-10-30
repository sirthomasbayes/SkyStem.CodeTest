using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SimpleInjector;
using SimpleInjector.Lifestyles;
using Skystem.Challenge.Service.Migrations;
using Skystem.Challenge.Core.Services;
using Skystem.Challenge.Service;

namespace Skystem.Challenge.App.Tests
{
	/// <summary>
	/// IoCContainers used to inject dependencies for IntegrationTests.
	/// Note: If we choose to implement Services, for instance, with a different
	///		  data store or data access framework, simply implement the required 
	///		  services, add another Container, and replace the DefaultContainer 
	///		  with the desired Container to test.
	/// </summary>
	public static class IoCContainer
	{
		private static Container _efContainer;
		public static Container EFContainer
		{
			get
			{
				if (_efContainer == null)
				{
					_efContainer = new Container();
					_efContainer.Register<IItemService, ItemEFService>(Lifestyle.Singleton);
					_efContainer.Register<IItemGroupService, ItemGroupEFService>(Lifestyle.Singleton);
					_efContainer.Register<IAttributeService, AttributeEFService>(Lifestyle.Singleton);
					_efContainer.Register<IDbSeeder, EFMigrator>(Lifestyle.Singleton);
				}

				return _efContainer;
			}
		}

		public static Container DefaultContainer => EFContainer;
	}
}
