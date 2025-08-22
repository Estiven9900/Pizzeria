using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Forms;
using PizzeriaOpita.App.Infra;
using PizzeriaOpita.App.Domain;
using PizzeriaOpita.App.UI;

namespace PizzeriaOpita.App
{
    internal static class Program
    {
        public static IServiceProvider? ServiceProvider;

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            var services = new ServiceCollection();

            // Repos
            services.AddSingleton<UsuarioRepository>();
            services.AddSingleton<PizzaRepository>();
            services.AddSingleton<PedidoRepository>();

            // Servicios app
            services.AddSingleton<AuthService>();
            services.AddTransient<PizzaService>();
            services.AddTransient<PedidoService>();

            // Forms (transient)
            services.AddTransient<LoginForm>();
            services.AddTransient<AdminForm>();
            services.AddTransient<AsistenteForm>();
            services.AddTransient<PizzeroForm>();

            ServiceProvider = services.BuildServiceProvider();

            Application.Run(ServiceProvider.GetRequiredService<LoginForm>());
        }
    }
}
