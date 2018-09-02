﻿using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CefSharp;
using log4net;
using Newtonsoft.Json;

namespace MoonPad
{
    /// <inheritdoc />
    /// <summary>
    /// This implements the HTTP endpoints. These implementations can be easily
    /// migrated to a web application.
    /// </summary>
    internal class BrowserSchemeHandler : ResourceHandler
    {
        private static readonly ILog Log = LogManager.
            GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly FormWindow formWindow;

        public BrowserSchemeHandler(FormWindow formWindow)
        {
            this.formWindow = formWindow;
        }

        public override bool ProcessRequestAsync(IRequest request, ICallback callback)
        {
            var uri = new Uri(request.Url);
            var path = uri.AbsolutePath; // $"{uri.PathAndQuery}{uri.Fragment}";

            Log.DebugFormat("Request path: {0}", path);

            Task.Run(() =>
            {
                using (callback)
                {
                    try
                    {
                        switch (path)
                        {
                            case "/api/methods/example":
                            {
                                //var json = GetDataFromRequest(request);
                                //var data = JsonConvert.DeserializeObject<object>(json);
                                //formWindow.Invoker.InvokeAndWaitFor(() => DoSomething(data));
                                StatusCode = (int) HttpStatusCode.OK;
                                break;
                            }
                            default:
                            {
                                StatusCode = (int) HttpStatusCode.NotFound;
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"EXCEPTION {ex.Message}", ex);
                        StatusCode = (int) HttpStatusCode.InternalServerError;
                    }

                    callback.Continue();
                }
            });

            return true;
        }

        private static string GetDataFromRequest(IRequest request)
        {
            if (request.PostData == null) request.InitializePostData();

            var sb = new StringBuilder();

            foreach (var postDataElement in request.PostData.Elements)
            {
                sb.Append(Encoding.UTF8.GetString(postDataElement.Bytes));
            }

            return sb.ToString();
        }

        private static Stream GetStream(string str)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}