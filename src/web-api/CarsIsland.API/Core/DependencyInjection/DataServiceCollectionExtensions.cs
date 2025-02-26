﻿using Azure.Cosmos;
using CarsIsland.Core.Entities;
using CarsIsland.Core.Interfaces;
using CarsIsland.Core.Services;
using CarsIsland.Infrastructure.Configuration.Interfaces;
using CarsIsland.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace CarsIsland.API.Core.DependencyInjection
{
    public static class DataServiceCollectionExtensions
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();

            var cosmoDbConfiguration = serviceProvider.GetRequiredService<ICosmosDbConfiguration>();
            CosmosClient cosmosClient = new CosmosClient(cosmoDbConfiguration.ConnectionString);
            CosmosDatabase database = cosmosClient.CreateDatabaseIfNotExistsAsync(cosmoDbConfiguration.DatabaseName)
                                                   .GetAwaiter()
                                                   .GetResult();
            database.CreateContainerIfNotExistsAsync(
                cosmoDbConfiguration.CarContainerName,
                cosmoDbConfiguration.CarContainerPartitionKeyPath,
                400)
                .GetAwaiter()
                .GetResult();

            database.CreateContainerIfNotExistsAsync(
                cosmoDbConfiguration.EnquiryContainerName,
                     cosmoDbConfiguration.EnquiryContainerPartitionKeyPath,
                400)
                .GetAwaiter()
                .GetResult();

            database.CreateContainerIfNotExistsAsync(
                cosmoDbConfiguration.CarReservationContainerName,
                     cosmoDbConfiguration.CarReservationPartitionKeyPath,
                400)
                .GetAwaiter()
                .GetResult();

            services.AddSingleton(cosmosClient);

            services.AddSingleton<IDataRepository<Car>, CarRepository>();
            services.AddSingleton<IDataRepository<Enquiry>, EnquiryRepository>();
            services.AddSingleton<ICarReservationRepository, CarReservationRepository>();

            services.AddSingleton<ICarReservationService, CarReservationService>();

            return services;
        }
    }
}
