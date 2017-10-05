using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Schedule.Startup))]
namespace Schedule
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
