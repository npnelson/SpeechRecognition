using Microsoft.Extensions.Configuration;
using NetToolBox.SpeechRecognition.Abstractions;
using NetToolBox.SpeechRecognition.Azure;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AzureSpeechRecognizerServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a ISpeechRecognizer with Azure implementation, configurationsection must contain values for AzureSpeechRecognizerSettings
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configurationSection"></param>
        /// <returns></returns>
        public static IServiceCollection AddAzureSpeechRecognizer(this IServiceCollection services, IConfigurationSection configurationSection)
        {
            services.Configure<AzureSpeechRecognizerSettings>(configurationSection);
            services.AddScoped<ISpeechRecognizer, AzureSpeechRecognizer>();
            return services;
        }
    }
}