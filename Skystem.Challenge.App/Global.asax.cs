using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;

using SimpleInjector;
using SimpleInjector.Lifestyles;
using SimpleInjector.Integration.WebApi;
using SimpleInjector.Integration.Web.Mvc;
using SimpleInjector.Integration.Web;
using Skystem.Challenge.Core.Services;
using Skystem.Challenge.Service;
using System.Reflection;
using Skystem.Challenge.Service.Migrations;

namespace Skystem.Challenge.App
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
			var container = new Container();
			container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();

			// Register your types, for instance:
			container.Register<IItemService, ItemEFService>(Lifestyle.Singleton);
			container.Register<IItemGroupService, ItemGroupEFService>(Lifestyle.Singleton);
			container.Register<IAttributeService, AttributeEFService>(Lifestyle.Singleton);
			container.Register<IDbSeeder, EFMigrator>(Lifestyle.Singleton);

			// This is an extension method from the integration package.
			container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
			container.RegisterWebApiControllers(GlobalConfiguration.Configuration);

			container.Verify();

			DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
			GlobalConfiguration.Configuration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);

			//container.GetInstance<IDbMigrator>().FlushDatabaseAsync().Wait();
			container.GetInstance<IDbSeeder>().SeedAsync().Wait();

			// Code that runs on application startup
			AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);            
        }
    }
}