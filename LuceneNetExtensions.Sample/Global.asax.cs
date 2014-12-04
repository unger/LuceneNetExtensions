namespace LuceneNetExtensions.Sample
{
    using System.Web.Mvc;
    using System.Web.Routing;

    using LuceneNetExtensions.Sample.App_Start;

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // Register Inversion of Control dependencies
            IoCConfig.RegisterDependencies();

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
