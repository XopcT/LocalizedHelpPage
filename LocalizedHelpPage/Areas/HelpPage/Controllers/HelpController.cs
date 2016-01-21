using System;
using System.Threading;
using System.Web.Http;
using System.Web.Mvc;
using LocalizedHelpPage.Areas.HelpPage.ModelDescriptions;
using LocalizedHelpPage.Areas.HelpPage.Models;

namespace LocalizedHelpPage.Areas.HelpPage.Controllers
{
    /// <summary>
    /// The controller that will handle requests for the help page.
    /// </summary>
    public partial class HelpController : Controller
    {
        private const string ErrorViewName = "Error";

        /// <summary>
        /// Initializes a new instance of current class.
        /// </summary>
        public HelpController()
            : this(GlobalConfiguration.Configuration)
        {
        }

        /// <summary>
        /// Initializes a new instance of current class.
        /// </summary>
        /// <param name="config">Configuration to initialize with.</param>
        public HelpController(HttpConfiguration config)
        {
            Configuration = config;
        }

        /// <summary>
        /// Configuration for the controller.
        /// </summary>
        public HttpConfiguration Configuration { get; private set; }

        /// <summary>
        /// Provides main Help Page.
        /// </summary>
        public ActionResult Index()
        {
            ViewBag.DocumentationProvider = Configuration.Services.GetDocumentationProvider();
            return View(Configuration.Services.GetApiExplorer().ApiDescriptions);
        }

        /// <summary>
        /// Provides Help Page for an API.
        /// </summary>
        /// <param name="apiId">API to get help for.</param>
        public ActionResult Api(string apiId)
        {
            if (!String.IsNullOrEmpty(apiId))
            {
                HelpPageApiModel apiModel = Configuration.GetHelpPageApiModel(apiId);
                if (apiModel != null)
                {
                    return View(apiModel);
                }
            }

            return View(ErrorViewName);
        }

        /// <summary>
        /// Provides Help Page for a model.
        /// </summary>
        /// <param name="modelName">Model to get help for.</param>
        public ActionResult ResourceModel(string modelName)
        {
            if (!String.IsNullOrEmpty(modelName))
            {
                modelName = string.Format("{0}({1})", modelName, Thread.CurrentThread.CurrentCulture.Name);
                ModelDescriptionGenerator modelDescriptionGenerator = Configuration.GetModelDescriptionGenerator();
                ModelDescription modelDescription;
                if (modelDescriptionGenerator.GeneratedModels.TryGetValue(modelName, out modelDescription))
                {
                    return View(modelDescription);
                }
            }

            return View(ErrorViewName);
        }
    }
}