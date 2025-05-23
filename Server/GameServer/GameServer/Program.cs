using BG.GameServer.Internals;
using Dignus.DependencyInjection;
using Dignus.DependencyInjection.Extensions;
using Dignus.Log;
using System.Reflection;

namespace BG.GameServer
{
    internal class Program
    {
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogHelper.Error(e.ExceptionObject as Exception);
            Environment.Exit(1);
        }
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            LogBuilder.Configuration(LogConfigXmlReader.Load("DignusLog.config")).Build();

            IServiceProvider serviceProvider = RegisterDependencies();


            GameServerNode gameServer = serviceProvider.GetService<GameServerNode>();

            gameServer.Start(40000);

            Console.ReadKey();
        }

        public static IServiceProvider RegisterDependencies()
        {
            var serviceContainer = new ServiceContainer();

            serviceContainer.RegisterDependencies(Assembly.GetExecutingAssembly());

            return serviceContainer.Build();
        }
    }
}
