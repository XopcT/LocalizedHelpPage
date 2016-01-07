using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace LocalizedHelpPage.Areas.HelpPage
{
    public class HelpPageAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "HelpPage";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "HelpPage_Default",
                "{culture}/Help/{action}/{apiId}",
                defaults: new { culture = "en-US", controller = "Help", action = "Index", apiId = UrlParameter.Optional }).RouteHandler = new MultiCultureMvcRouteHandler();

            HelpPageConfig.Register(GlobalConfiguration.Configuration);
        }

        /// <summary>
        /// Handler for multiculture routes.
        /// </summary>
        private class MultiCultureMvcRouteHandler : MvcRouteHandler
        {
            /// <summary>
            /// Returns the HTTP handler by using the specified HTTP context.
            /// </summary>
            /// <param name="requestContext">The request context.</param>
            /// <returns>The HTTP handler.</returns>
            protected override IHttpHandler GetHttpHandler(RequestContext requestContext)
            {
                string cultureName = requestContext.RouteData.Values["culture"].ToString();
                CultureInfo culture = new CultureInfo(cultureName);
                Thread.CurrentThread.CurrentUICulture = culture;
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(culture.Name);
                return base.GetHttpHandler(requestContext);
            }
        }
    }
}