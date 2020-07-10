using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetToolBox.BlobStorage.Azure;
using NetToolBox.SpeechRecognition.Abstractions;
using NETToolBox.BlobStorage.Abstractions;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NetToolBox.SpeechRecognition.TestHarness
{
    class Program
    {
        static IConfiguration Configuration;
        static async Task Main(string[] args)
        {
            Configuration = new ConfigurationBuilder()
   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
   .AddUserSecrets("b72ade7e-7011-48ec-95fc-15582f45a97c")
   .Build();

            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            var recognizer = serviceProvider.GetRequiredService<ISpeechRecognizer>();
            var blobConnectionString = Configuration.GetValue<string>("BlobConnectionString");
            var blobFactory = serviceProvider.GetRequiredService<IBlobStorageFactory>();
            var blobClient = blobFactory.GetBlobStorage(new Uri(blobConnectionString));
            var blobStream = await blobClient.DownloadFileAsStreamAsync("speech.wav");

            var httpClient = new HttpClient();
            var httpStream = await httpClient.GetStreamAsync(new Uri(""));

            var recognizedText = await recognizer.RecognizeSpeechFromPullWavStreamAsync(httpStream);

            Console.WriteLine(recognizedText);
            Console.ReadLine();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(builder => builder.AddConsole());
            services.AddAzureSpeechRecognizer(Configuration.GetSection("AzureSpeechRecognizerSettings"));
            services.AddAzureBlobStorageFactory();
        }
    }
}
