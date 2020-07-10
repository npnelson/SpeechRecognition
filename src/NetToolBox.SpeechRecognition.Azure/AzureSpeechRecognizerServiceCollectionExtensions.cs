using Microsoft.Extensions.Configuration;
using NetToolBox.SpeechRecognition.Abstractions;
using NetToolBox.SpeechRecognition.Azure;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AzureSpeechRecognizerServiceCollectionExtensions
    {
        public static IServiceCollection AddAzureSpeechRecognizer(this IServiceCollection services)
        {
            //TODO: Do some checking to make sure options have been registered            
            services.AddScoped<ISpeechRecognizer, AzureSpeechRecognizer>();
            return services;
        }
        public static IServiceCollection AddAzureSpeechRecognizer(this IServiceCollection services, IConfigurationSection configurationSection)
        {
            //TODO: Do some checking to make sure configsection is valid
            services.Configure<AzureSpeechRecognizerSettings>(configurationSection);
            services.AddAzureSpeechRecognizer();
            return services;
        }
    }
}
