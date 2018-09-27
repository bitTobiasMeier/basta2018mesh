using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Platform
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc();

            AddWebSockets(app);
        }

        //ToDo: Call from Configure

        private void AddWebSockets(IApplicationBuilder app)
        {
            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
            };
            app.UseWebSockets(webSocketOptions);

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/data")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        await SendData(context, webSocket);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
                else
                {
                    await next();
                }
            });
        }

        private string Cook()
        {
            HttpClient client = new HttpClient();
            try
            {
                var restaurantsvcname = $"{Environment.GetEnvironmentVariable("RestaurantServiceName")}";
                var backendUrl =
                    $"http://{restaurantsvcname}:{Environment.GetEnvironmentVariable("RestaurantServicePort")}/api/restaurant/cook/Tagesmenü";


                using (HttpResponseMessage response = client.GetAsync(backendUrl).GetAwaiter().GetResult())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return response.Content.ReadAsStringAsync().Result;
                    }

                    return response.StatusCode.ToString();
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return ex.Message;
            }

        }

        private async Task SendData(HttpContext context, WebSocket webSocket)
        {
            using (webSocket)
            {
                while (true)
                {
                    var gekocht = Cook();
                    byte[] buffer = Encoding.UTF8.GetBytes(gekocht);

                    try
                    {
                        await webSocket.SendAsync(
                            new ArraySegment<byte>(buffer, 0, buffer.Length),
                            WebSocketMessageType.Text,
                            true,
                            CancellationToken.None);

                        if (webSocket.State != WebSocketState.Open)
                        {
                            break;
                        }
                    }
                    catch (WebSocketException)
                    {
                        // If the browser quit or the socket was closed, exit this loop so we can get a new browser socket.
                        break;
                    }

                    // wait a bit and continue. This determines the client refresh rate.
                    await Task.Delay(TimeSpan.FromMilliseconds(500), CancellationToken.None);
                }
            }
        }
    }
}
