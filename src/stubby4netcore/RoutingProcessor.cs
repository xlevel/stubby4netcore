using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace stubby4netcore
{
    public static class RoutingProcessor
    {
        public static IRouter Build(IApplicationBuilder app, IEnumerable<Configuration.Data.EndPoint> endpointConfig)
        {
            var routeBuilder = new RouteBuilder(app);

            foreach (var endpoint in endpointConfig)
            {
                routeBuilder.MapGet(endpoint.Request.Url, context =>
                {
                    context.Response.StatusCode = endpoint.Response.Status;
                    context.Response.Headers.Add("Content-Type", endpoint.Response.Headers["Content-Type"]);

                return context.Response.WriteAsync(string.Empty);
                });
            }

            var routes = routeBuilder.Build();
            return routes;
        }
    }
}