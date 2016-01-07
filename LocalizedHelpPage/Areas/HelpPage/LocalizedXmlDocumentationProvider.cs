using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Xml.XPath;
using LocalizedHelpPage.Areas.HelpPage.ModelDescriptions;

namespace LocalizedHelpPage.Areas.HelpPage
{
    /// <summary>
    /// A custom <see cref="IDocumentationProvider"/> that reads the API documentation from a localized XML documentation file.
    /// </summary>
    public class LocalizedXmlDocumentationProvider : IDocumentationProvider, IModelDocumentationProvider
    {
        private XPathNavigator _documentNavigator;
        private const string TypeExpression = "/doc/members/member[@name='T:{0}']";
        private const string MethodExpression = "/doc/members/member[@name='M:{0}']";
        private const string PropertyExpression = "/doc/members/member[@name='P:{0}']";
        private const string FieldExpression = "/doc/members/member[@name='F:{0}']";
        private const string ParameterExpression = "param[@name='{0}']";
        private const string xmlNamespace = @"http://www.w3.org/XML/1998/namespace";

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizedXmlDocumentationProvider"/> class.
        /// </summary>
        /// <param name="documentPath">The physical path to XML document.</param>
        public LocalizedXmlDocumentationProvider(string documentPath)
        {
            if (documentPath == null)
            {
                throw new ArgumentNullException("documentPath");
            }
            XPathDocument xpath = new XPathDocument(documentPath);
            _documentNavigator = xpath.CreateNavigator();
        }

        /// <summary>
        /// Gets the documentation based on <see cref="System.Web.Http.Controllers.HttpControllerDescriptor"/>.
        /// </summary>
        /// <param name="controllerDescriptor">The controller descriptor.</param>
        /// <returns>The documentation for the controller.</returns>
        public string GetDocumentation(HttpControllerDescriptor controllerDescriptor)
        {
            XPathNavigator typeNode = GetTypeNode(controllerDescriptor.ControllerType);
            return GetTagValue(typeNode, "summary");
        }

        /// <summary>
        /// Gets the documentation based on <see cref="System.Web.Http.Controllers.HttpActionDescriptor"/>.
        /// </summary>
        /// <param name="actionDescriptor">The action descriptor.</param>
        /// <returns>The documentation for the controller.</returns>
        public virtual string GetDocumentation(HttpActionDescriptor actionDescriptor)
        {
            XPathNavigator methodNode = GetMethodNode(actionDescriptor);
            return GetTagValue(methodNode, "summary");
        }

        /// <summary>
        /// Gets the documentation based on <see cref="System.Web.Http.Controllers.HttpParameterDescriptor" />.
        /// </summary>
        /// <param name="parameterDescriptor">The parameter descriptor.</param>
        /// <returns>The documentation for the controller.</returns>
        public virtual string GetDocumentation(HttpParameterDescriptor parameterDescriptor)
        {
            ReflectedHttpParameterDescriptor reflectedParameterDescriptor = parameterDescriptor as ReflectedHttpParameterDescriptor;
            if (reflectedParameterDescriptor != null)
            {
                XPathNavigator methodNode = GetMethodNode(reflectedParameterDescriptor.ActionDescriptor);
                if (methodNode != null)
                {
                    string parameterName = reflectedParameterDescriptor.ParameterInfo.Name;
                    string nodeName = String.Format(CultureInfo.InvariantCulture, ParameterExpression, parameterName);
                    return GetTagValue(methodNode, nodeName);
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the action's response documentation based on <see cref="System.Web.Http.Controllers.HttpActionDescriptor"/>.
        /// </summary>
        /// <param name="actionDescriptor">The action descriptor.</param>
        /// <returns>The documentation for the controller.</returns>
        public string GetResponseDocumentation(HttpActionDescriptor actionDescriptor)
        {
            XPathNavigator methodNode = GetMethodNode(actionDescriptor);
            return GetTagValue(methodNode, "returns");
        }

        /// <summary>
        /// Gets the documentation based on <see cref="System.Reflection.MemberInfo"/>.
        /// </summary>
        /// <param name="member">The member information.</param>
        /// <returns>The documentation for the controller.</returns>
        public string GetDocumentation(MemberInfo member)
        {
            string memberName = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", GetTypeName(member.DeclaringType), member.Name);
            string expression = member.MemberType == MemberTypes.Field ? FieldExpression : PropertyExpression;
            string selectExpression = String.Format(CultureInfo.InvariantCulture, expression, memberName);
            XPathNavigator propertyNode = _documentNavigator.SelectSingleNode(selectExpression);
            return GetTagValue(propertyNode, "summary");
        }

        /// <summary>
        /// Gets the documentation based on <see cref="System.Type"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The documentation for the controller.</returns>
        public string GetDocumentation(Type type)
        {
            XPathNavigator typeNode = GetTypeNode(type);
            return GetTagValue(typeNode, "summary");
        }

        private XPathNavigator GetMethodNode(HttpActionDescriptor actionDescriptor)
        {
            ReflectedHttpActionDescriptor reflectedActionDescriptor = actionDescriptor as ReflectedHttpActionDescriptor;
            if (reflectedActionDescriptor != null)
            {
                string selectExpression = String.Format(CultureInfo.InvariantCulture, MethodExpression, GetMemberName(reflectedActionDescriptor.MethodInfo));
                return _documentNavigator.SelectSingleNode(selectExpression);
            }

            return null;
        }

        private static string GetMemberName(MethodInfo method)
        {
            string name = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", GetTypeName(method.DeclaringType), method.Name);
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length != 0)
            {
                string[] parameterTypeNames = parameters.Select(param => GetTypeName(param.ParameterType)).ToArray();
                name += String.Format(CultureInfo.InvariantCulture, "({0})", String.Join(",", parameterTypeNames));
            }

            return name;
        }

        private static string GetTagValue(XPathNavigator parentNode, string tagName)
        {
            if (parentNode != null)
            {
                XPathNavigator node = parentNode.Select(tagName)
                    .Cast<XPathNavigator>()
                    .Select(nextNode => new
                    {
                        Node = nextNode,
                        Culture = nextNode.GetAttribute("lang", xmlNamespace)
                    })
                    .Where(nextNode => nextNode.Culture == Thread.CurrentThread.CurrentCulture.Name || string.IsNullOrEmpty(nextNode.Culture))
                    .OrderByDescending(nextNode => nextNode.Culture == Thread.CurrentThread.CurrentCulture.Name)
                    .ThenByDescending(nextNode => string.IsNullOrEmpty(nextNode.Culture))
                    .Select(nextNode => nextNode.Node)
                    .FirstOrDefault();
                if (node != null)
                {
                    return node.Value.Trim();
                }
            }

            return null;
        }

        private XPathNavigator GetTypeNode(Type type)
        {
            string controllerTypeName = GetTypeName(type);
            string selectExpression = String.Format(CultureInfo.InvariantCulture, TypeExpression, controllerTypeName);
            return _documentNavigator.SelectSingleNode(selectExpression);
        }

        private static string GetTypeName(Type type)
        {
            string name = type.FullName;
            if (type.IsGenericType)
            {
                // Format the generic type name to something like: Generic{System.Int32,System.String}
                Type genericType = type.GetGenericTypeDefinition();
                Type[] genericArguments = type.GetGenericArguments();
                string genericTypeName = genericType.FullName;

                // Trim the generic parameter counts from the name
                genericTypeName = genericTypeName.Substring(0, genericTypeName.IndexOf('`'));
                string[] argumentTypeNames = genericArguments.Select(t => GetTypeName(t)).ToArray();
                name = String.Format(CultureInfo.InvariantCulture, "{0}{{{1}}}", genericTypeName, String.Join(",", argumentTypeNames));
            }
            if (type.IsNested)
            {
                // Changing the nested type name from OuterType+InnerType to OuterType.InnerType to match the XML documentation syntax.
                name = name.Replace("+", ".");
            }

            return name;
        }
    }
}
