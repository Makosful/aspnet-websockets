using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace WebsocketTesting.Middleware
{
    public static class WebSocketServerMiddlewareExtension
    {
        public static IServiceCollection AddWebSocketServices(this IServiceCollection services)
        {
            services.AddSingleton<WebSocketConnectionManager>();
            return services;
        }

        public static IApplicationBuilder UseWebSocketServer(this IApplicationBuilder app)
        {
            app.UseMiddleware<WebSocketServerMiddleware>();
            return app;
        }
    }
}