using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using LocalizedHelpPage.Areas.HelpPage.ModelDescriptions;

namespace LocalizedHelpPage.Areas.HelpPage
{
    /// <summary>
    /// A custom <see cref="IDocumentationProvider"/> that reads the API documentation from multiple sources.
    /// </summary>
    public class MultipleSourceDocumentationProvider : IDocumentationProvider, IModelDocumentationProvider
    {
        /// <summary>
        /// Initializes a new instance of current class.
        /// </summary>
        /// <param name="sources">Sources to resolve documentation from.</param>
        public MultipleSourceDocumentationProvider(params IDocumentationProvider[] sources)
        {
            if (sources == null)
                throw new ArgumentNullException("sources");
            this.sources = sources;
        }

        /// <summary>
        /// Gets the documentation based on <see cref="System.Web.Http.Controllers.HttpControllerDescriptor"/>.
        /// </summary>
        /// <param name="controllerDescriptor">The controller descriptor.</param>
        /// <returns>The documentation for the controller.</returns>
        public string GetDocumentation(HttpControllerDescriptor controllerDescriptor)
        {
            return this.sources
                .Select(source => source.GetDocumentation(controllerDescriptor))
                .Where(documentation => !string.IsNullOrEmpty(documentation))
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets the documentation based on <see cref="System.Web.Http.Controllers.HttpActionDescriptor"/>.
        /// </summary>
        /// <param name="actionDescriptor">The action descriptor.</param>
        /// <returns>The documentation for the controller.</returns>
        public string GetDocumentation(HttpActionDescriptor actionDescriptor)
        {
            return this.sources
                .Select(source => source.GetDocumentation(actionDescriptor))
                .Where(documentation => !string.IsNullOrEmpty(documentation))
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets the action's response documentation based on <see cref="System.Web.Http.Controllers.HttpActionDescriptor"/>.
        /// </summary>
        /// <param name="actionDescriptor">The action descriptor.</param>
        /// <returns>The documentation for the controller.</returns>
        public string GetResponseDocumentation(HttpActionDescriptor actionDescriptor)
        {
            return this.sources
                .Select(source => source.GetResponseDocumentation(actionDescriptor))
                .Where(documentation => !string.IsNullOrEmpty(documentation))
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets the documentation based on <see cref="System.Web.Http.Controllers.HttpParameterDescriptor" />.
        /// </summary>
        /// <param name="parameterDescriptor">The parameter descriptor.</param>
        /// <returns>The documentation for the controller.</returns>
        public string GetDocumentation(HttpParameterDescriptor parameterDescriptor)
        {
            return this.sources
                .Select(source => source.GetDocumentation(parameterDescriptor))
                .Where(documentation => !string.IsNullOrEmpty(documentation))
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets the documentation based on <see cref="System.Type"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The documentation for the controller.</returns>
        public string GetDocumentation(Type type)
        {
            return this.sources
                .OfType<IModelDocumentationProvider>()
                .Select(source => source.GetDocumentation(type))
                .Where(documentation => !string.IsNullOrEmpty(documentation))
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets the documentation based on <see cref="System.Reflection.MemberInfo"/>.
        /// </summary>
        /// <param name="member">The member information.</param>
        /// <returns>The documentation for the controller.</returns>
        public string GetDocumentation(MemberInfo member)
        {
            return this.sources
                .OfType<IModelDocumentationProvider>()
                .Select(source => source.GetDocumentation(member))
                .Where(documentation => !string.IsNullOrEmpty(documentation))
                .FirstOrDefault();
        }

        #region Fields
        private readonly IEnumerable<IDocumentationProvider> sources = null;

        #endregion
    }
}