using Microsoft.Extensions.Configuration;
using StackifyLib;
using System.IO;

namespace Lhi.NsbDemo.Orders.Sagas.Utils
{
    public static class AppSettings
    {
        static IConfigurationRoot _configuration = null;
        static AppSettings()
        {
            
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();
            //TODO: refactor AppSettings class for Stackify
            _configuration.ConfigureStackifyLogging();
        }

        public static string ElasticSearchUrl
        {
            get => _configuration.GetValue<string>("elasticsearch_url");
        }

        public static string RabbitMQUrl
        {
            get => _configuration.GetValue<string>("rabbitmq_url");
        }

        public static string SQLConnectionString
        {
            get => _configuration.GetValue<string>("sql_connection_string");
        }

        public static string StackifyApiKey
        {
            get => _configuration.GetValue<string>("Stackify.ApiKey");
        }

        public static string StackifyApiName
        {
            get => _configuration.GetValue<string>("Stackify.ApiName");
        }

        public static string StackifyEnvironment
        {
            get => _configuration.GetValue<string>("Stackify.Environment");
        }

        public static string Prometheus
        {
            get => _configuration.GetValue<string>("prometheus");
        }
    }
            
} 
