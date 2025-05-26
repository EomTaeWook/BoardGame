using BG.GameServer.Internals;
using BG.GameServer.Models;
using BG.GameServer.Network;
using Dignus.DependencyInjection;
using Dignus.DependencyInjection.Extensions;
using Dignus.Log;
using System.Reflection;
using System.Text.Json;

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
            var config = serviceProvider.GetService<Config>();
            gameServer.Start(config.ServerPort);
            LogHelper.Info($"BG.Server Start. port : {config.ServerPort}");
            Console.ReadKey();
        }

        private static IServiceProvider RegisterDependencies()
        {
            var serviceContainer = new ServiceContainer();

            serviceContainer.RegisterDependencies(Assembly.GetExecutingAssembly());

            serviceContainer.RegisterType<HeartBeat, HeartBeat>();

            LoadConfig(serviceContainer);

            return serviceContainer.Build();
        }
        private static void LoadConfig(ServiceContainer serviceContainer)
        {
            var stage = Environment.GetEnvironmentVariable("stage");
            var filePath = "config.json";
            if (!string.IsNullOrEmpty(stage))
            {
                filePath = $"config.{stage}.json";
            }
            var configJson = File.ReadAllText(filePath);

            var config = JsonSerializer.Deserialize<Config>(configJson);

            serviceContainer.RegisterType(config);
        }
    }
}
