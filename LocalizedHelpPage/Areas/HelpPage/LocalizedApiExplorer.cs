using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Description;

namespace LocalizedHelpPage.Areas.HelpPage
{
    /// <summary>
    /// Explores the API and provides localized description.
    /// </summary>
    public class LocalizedApiExplorer : IApiExplorer
    {
        /// <summary>
        /// Initializes a new instance of current class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public LocalizedApiExplorer(HttpConfiguration config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            this.config = config;
        }

        /// <summary>
        /// Creates an instance of API explorer.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns>New instance of API explorer.</returns>
        private static IApiExplorer CreateApiExplorer(HttpConfiguration config)
        {
            return new ApiExplorer(config);
        }

        #region Properties

        /// <summary>
        /// Gets the API descriptions.
        /// </summary>
        public Collection<ApiDescription> ApiDescriptions
        {
            get
            {
                IApiExplorer apiExplorer = null;
                if (!this.apiExplorers.TryGetValue(Thread.CurrentThread.CurrentCulture, out apiExplorer))
                {
                    apiExplorer = CreateApiExplorer(this.config);
                    this.apiExplorers.Add(Thread.CurrentThread.CurrentCulture, apiExplorer);
                }
                return apiExplorer.ApiDescriptions;
            }
        }

        #endregion

        #region Fields
        private readonly HttpConfiguration config = null;
        private readonly IDictionary<CultureInfo, IApiExplorer> apiExplorers = new Dictionary<CultureInfo, IApiExplorer>();

        #endregion
    }
}