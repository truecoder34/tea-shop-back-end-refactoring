using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Microsoft.Owin;
using Ninject;
using Ninject.Web.Common.OwinHost;
using Ninject.Web.WebApi.OwinHost;
using Owin;
using WebAPITeaApp.Bootstrap;


[assembly: OwinStartup(typeof(WebAPITeaApp.Startup))]

namespace WebAPITeaApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();

            // DI initializtion
            IKernel ninjectKernel = new StandardKernel(new InjectorModule());
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            //app.UseNinjectMiddleware(() => ninjectKernel);
            app.UseNinjectMiddleware(() => ninjectKernel).UseNinjectWebApi(config);
            ConfigureAuth(app);
        }
    }
}
