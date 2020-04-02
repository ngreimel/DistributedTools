using DistributedToolsServer.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DistributedToolsServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddSignalR();

            services.AddSingleton<IRoomSessionRepository, RoomSessionRepository>();
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddTransient<ICurrentUserAccessor, CurrentUserAccessor>();
            services.AddTransient<IDecisionDelegationSession, DecisionDelegationSession>();
            services.AddTransient<IUserGroup, UserGroup>();
            services.AddTransient<IDecisionDelegationItemGroup, DecisionDelegationDecisionDelegationItemGroup>();
            services.AddTransient<IRoomCodeGenerator, RoomCodeGenerator>();
            services.AddTransient<IDecisionDelegationDataMapper, DecisionDelegationDataMapper>();
            services.AddTransient<IVotingSession, VotingSession>();
            services.AddTransient<IVotingSessionDataMapper, VotingSessionDataMapper>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<DecisionDelegationHub>("/decision-delegation-hub");
                endpoints.MapHub<VotingHub>("/voting-hub");
            });
        }
    }
}
