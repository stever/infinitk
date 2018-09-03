using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using CefSharp;
using log4net;
using MoonPad.Properties;

namespace MoonPad
{
    /// <summary>
    /// All content requested by the web browser component is embedded in as
    /// resources. This maps HTTP requests to those resource objects.
    /// </summary>
    internal class BrowserSchemeHandlerFactory : ISchemeHandlerFactory
    {
        private static readonly ILog Log = LogManager.
            GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly FormWindow formWindow;
        private readonly string defaultPage;
        private readonly string schemeName;
        private readonly string hostName;

        private readonly IDictionary<string, object> resources = new Dictionary<string, object>
        {
            {"/fonts/PragmataProMono.woff2", Resources.htdocs_fonts_PragmataProMono_woff2},
            {"/js/jquery-3.2.1.min.js", Resources.htdocs_js_jquery_3_2_1_min_js},
            {"/term/index.css", Resources.htdocs_term_index_css},
            {"/term/index.html", Resources.htdocs_term_index_html},
            {"/term/index.js", Resources.htdocs_term_index_js},
            {"/term/jquery.terminal.min.css", Resources.htdocs_term_jquery_terminal_min_css},
            {"/term/jquery.terminal.min.js", Resources.htdocs_term_jquery_terminal_min_js},
        };

        public BrowserSchemeHandlerFactory(FormWindow formWindow, string schemeName, string hostName, string defaultPage)
        {
            this.formWindow = formWindow;
            this.defaultPage = defaultPage;
            this.schemeName = schemeName;
            this.hostName = hostName;
        }

        // ReSharper disable once ParameterHidesMember
        public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
        {
            try
            {
                if (this.schemeName != null && !schemeName.Equals(this.schemeName, StringComparison.OrdinalIgnoreCase))
                {
                    return GetResponse(HttpStatusCode.NotFound,
                        $"SchemeName {schemeName} does not match the expected " +
                        $"SchemeName of {this.schemeName}.");
                }

                var uri = new Uri(request.Url);

                if (hostName != null && !uri.Host.Equals(hostName, StringComparison.OrdinalIgnoreCase))
                {
                    return GetResponse(HttpStatusCode.NotFound,
                        $"HostName {uri.Host} does not match the expected " +
                        $"HostName of {hostName}.");
                }

                var path = uri.AbsolutePath;

                if (string.IsNullOrEmpty(path.Substring(1)))
                {
                    path = defaultPage;
                }

                if (resources.ContainsKey(path))
                {
                    Log.DebugFormat("Resource request path: {0}", path);
                    return GetResource(path);
                }

                if (path.StartsWith("/api/"))
                {
                    return new BrowserSchemeHandler(formWindow);
                }

                if (path.StartsWith("/manual/"))
                {
                    Log.DebugFormat("Manual request path: {0}", path);
                    path = path.Substring("/manual/".Length);
#if DEBUG
                    path = Path.Combine(@"..\..\..\..\..\..\docs\Help & Manual\Software Manual\HTML\", path);
#else
                    path = Path.Combine(@"..\..\docs\HTML\", path);
#endif
                    return GetFile(path);
                }

                return GetResponse(HttpStatusCode.NotFound, "Not Found");
            }
            catch (Exception ex)
            {
                Log.Error($"EXCEPTION {ex.Message}", ex);
                throw;
            }
        }

        private IResourceHandler GetResource(string path)
        {
            var resource = resources[path];
            Debug.Assert(resource != null);

            byte[] bytes;
            switch (resource)
            {
                case byte[] byteArray:
                    bytes = byteArray;
                    break;
                case string str:
                    bytes = Encoding.UTF8.GetBytes(str);
                    break;
                case Bitmap _:
                    bytes = (byte[])new ImageConverter().ConvertTo(resource, typeof(byte[]));
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unexpected resource type for file: {path} ({resource.GetType().Name})");
            }

            Debug.Assert(bytes != null);
            var stream = new MemoryStream(bytes);
            var fileExtension = Path.GetExtension(path);
            var mimeType = ResourceHandler.GetMimeType(fileExtension);
            return ResourceHandler.FromStream(stream, mimeType);
        }

        private static IResourceHandler GetFile(string path)
        {
            var fileExtension = Path.GetExtension(path);
            var mimeType = ResourceHandler.GetMimeType(fileExtension);
            return ResourceHandler.FromFilePath(path, mimeType);
        }

        private static IResourceHandler GetResponse(HttpStatusCode statusCode, string msg = null)
        {
            if (msg == null) msg = "";
            var stream = ResourceHandler.GetMemoryStream(msg, Encoding.UTF8);
            var resourceHandler = ResourceHandler.FromStream(stream);
            resourceHandler.StatusCode = (int)statusCode;
            return resourceHandler;
        }
    }
}
