using Microsoft.Extensions.DependencyInjection;

namespace WishListik
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public App()
        {
            InitializeComponent();

            var services = new ServiceCollection();
            services.AddHttpClient();

            ServiceProvider = services.BuildServiceProvider();

            MainPage = new LoginPage();
        }
    }
}
