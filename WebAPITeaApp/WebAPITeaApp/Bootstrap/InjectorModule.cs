using Ninject.Modules;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPITeaApp.Bootstrap
{
    public class InjectorModule : NinjectModule
    {
        public override void Load()
        {
            BindTranlators();
            BindLogger();
        }

        private void BindTranlators()
        {
        }

        private void BindLogger()
        {
            this.Bind<ILogger>().ToMethod(x => LogManager.GetLogger("Trace"));
        }




    }
}