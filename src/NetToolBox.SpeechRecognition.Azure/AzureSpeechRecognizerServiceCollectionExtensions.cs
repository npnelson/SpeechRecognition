using Microsoft.Extensions.Configuration;
using NetToolBox.SpeechRecognition.Abstractions;
using NetToolBox.SpeechRecognition.Azure;
using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AzureSpeechRecognizerServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a ISpeechRecognizer with Azure implementation
        /// You must register options for AzureSpeechRecognizerSettings separately when using this overload
        /// This overload will be deprecated once Azure Functions support ASPNet core style configuration https://github.com/Azure/azure-functions-host/issues/4761
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddAzureSpeechRecognizer(this IServiceCollection services)
        {
            var optionsRegistered = services.Any(x => x.ServiceType.GenericTypeArguments.Any(x => x == typeof(AzureSpeechRecognizerSettings)));
            if (!optionsRegistered) throw new InvalidOperationException("You must register options for Azure SpeechRecognizerSettings");
            services.AddScoped<ISpeechRecognizer, AzureSpeechRecognizer>();
            return services;
        }
        /// <summary>
        /// Adds a ISpeechRecognizer with Azure implementation, configurationsection must contain values for AzureSpeechRecognizerSettings
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configurationSection"></param>
        /// <returns></returns>
        public static IServiceCollection AddAzureSpeechRecognizer(this IServiceCollection services, IConfigurationSection configurationSection)
        {

            services.Configure<AzureSpeechRecognizerSettings>(configurationSection);
            services.AddAzureSpeechRecognizer();
            return services;
        }
    }
}
