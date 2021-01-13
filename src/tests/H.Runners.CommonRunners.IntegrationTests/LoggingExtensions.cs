using System;
using System.Threading;
using System.Threading.Tasks;
using H.Core;
using H.Core.Runners;

namespace H.Runners.IntegrationTests
{
    public static class LoggingExtensions
    {
        public static T WithLogging<T>(this T module) where T : IModule
        {
            module = module ?? throw new ArgumentNullException(nameof(module));
            module.CommandReceived += (_, value) =>
            {
                Console.WriteLine($"{nameof(module.CommandReceived)}: {value}");
            };
            module.AsyncCommandReceived += (_, value, _) =>
            {
                Console.WriteLine($"{nameof(module.AsyncCommandReceived)}: {value}");

                return Task.FromResult<IValue>(Value.Empty);
            };
            module.ExceptionOccurred += (_, value) =>
            {
                Console.WriteLine($"{nameof(module.ExceptionOccurred)}: {value}");
            };

            return module;
        }
    }
}
