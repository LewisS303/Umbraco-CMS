﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc.Html;
using Umbraco.Core;
using Umbraco.Core.Models;
using System.Web.Mvc;
using Umbraco.Web.Templates;
using System.IO;
using System.Web.Routing;
using Umbraco.Web.Mvc;

namespace Umbraco.Web
{
    
    public static class GridTemplateExtensions
    {
        public static MvcHtmlString GetGridHtml(this HtmlHelper html, IPublishedProperty property, string framework = "bootstrap3")
        {
            var asString = property.Value as string;
            if (asString.IsNullOrWhiteSpace()) return new MvcHtmlString(string.Empty);

            var view = "Grid/" + framework;
            return html.Partial(view, property.Value);
        }

        public static MvcHtmlString GetGridHtml(this HtmlHelper html, IPublishedContent contentItem)
        {
            return html.GetGridHtml(contentItem, "bodyText", "bootstrap3");
        }

        public static MvcHtmlString GetGridHtml(this HtmlHelper html, IPublishedContent contentItem, string propertyAlias)
        {
            Mandate.ParameterNotNullOrEmpty(propertyAlias, "propertyAlias");

            return html.GetGridHtml(contentItem, propertyAlias, "bootstrap3");
        }

        public static MvcHtmlString GetGridHtml(this HtmlHelper html, IPublishedContent contentItem, string propertyAlias, string framework)
        {
            Mandate.ParameterNotNullOrEmpty(propertyAlias, "propertyAlias");

            var view = "Grid/" + framework;
            var prop = contentItem.GetProperty(propertyAlias);
            if (prop == null) throw new NullReferenceException("No property type found with alias " + propertyAlias);
            var model = prop.Value;

            var asString = model as string;
            if (asString.IsNullOrWhiteSpace()) return new MvcHtmlString(string.Empty);

            return html.Partial(view, model);
        }


        [Obsolete("This should not be used, GetGridHtml extensions on HtmlHelper should be used instead")]
        public static MvcHtmlString GetGridHtml(this IPublishedProperty property, string framework = "bootstrap3")
        {
            var asString = property.Value as string;
            if (asString.IsNullOrWhiteSpace()) return new MvcHtmlString(string.Empty);
            
            var view = "Grid/" + framework;
            return new MvcHtmlString(RenderPartialViewToString(view, property.Value));
        }

        [Obsolete("This should not be used, GetGridHtml extensions on HtmlHelper should be used instead")]
        public static MvcHtmlString GetGridHtml(this IPublishedContent contentItem)
        {
            return GetGridHtml(contentItem, "bodyText", "bootstrap3");
        }

        [Obsolete("This should not be used, GetGridHtml extensions on HtmlHelper should be used instead")]
        public static MvcHtmlString GetGridHtml(this IPublishedContent contentItem, string propertyAlias)
        {
            Mandate.ParameterNotNullOrEmpty(propertyAlias, "propertyAlias");

            return GetGridHtml(contentItem, propertyAlias, "bootstrap3");    
        }

        [Obsolete("This should not be used, GetGridHtml extensions on HtmlHelper should be used instead")]
        public static MvcHtmlString GetGridHtml(this IPublishedContent contentItem, string propertyAlias, string framework)
        {
            Mandate.ParameterNotNullOrEmpty(propertyAlias, "propertyAlias");

            var view = "Grid/" + framework;
            var prop = contentItem.GetProperty(propertyAlias);
            if (prop == null) throw new NullReferenceException("No property type found with alias " + propertyAlias);
            var model = prop.Value;

            var asString = model as string;
            if (asString.IsNullOrWhiteSpace()) return new MvcHtmlString(string.Empty);

            return new MvcHtmlString(RenderPartialViewToString(view, model));
        }

        [Obsolete("This should not be used, GetGridHtml extensions on HtmlHelper should be used instead")]
        private static string RenderPartialViewToString(string viewName, object model)
        {

            using (var sw = new StringWriter())
            {
                var cc = new ControllerContext
                             {
                                 RequestContext =
                                     new RequestContext(
                                     UmbracoContext.Current.HttpContext,
                                     new RouteData() { Route = RouteTable.Routes["Umbraco_default"] })
                             };

                var routeHandler = new RenderRouteHandler(ControllerBuilder.Current.GetControllerFactory(), UmbracoContext.Current);
                var routeDef = routeHandler.GetUmbracoRouteDefinition(cc.RequestContext, UmbracoContext.Current.PublishedContentRequest);
                cc.RequestContext.RouteData.Values.Add("action", routeDef.ActionName);
                cc.RequestContext.RouteData.Values.Add("controller", routeDef.ControllerName);

                var partialView = ViewEngines.Engines.FindPartialView(cc, viewName);
                var viewData = new ViewDataDictionary();
                var tempData = new TempDataDictionary();
                
                viewData.Model = model;

                var viewContext = new ViewContext(cc, partialView.View, viewData, tempData, sw);
                partialView.View.Render(viewContext, sw);
                partialView.ViewEngine.ReleaseView(cc, partialView.View);
                
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}