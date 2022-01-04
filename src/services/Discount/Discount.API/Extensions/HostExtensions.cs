﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Discount.API.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host, int? retry = 0)
        {
            int retryForAvailability = retry.Value;

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TContext>>();

                try
                {
                    logger.LogInformation("Migrating PostGres DB");

                    using var connection = new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
                    connection.Open();

                    using var command = new NpgsqlCommand()
                    {
                        Connection = connection
                    };

                    command.CommandText = "Drop table if exists Coupon";
                    command.ExecuteNonQuery();

                    command.CommandText = @"create table Coupon(Id Serial primary key,
                                                               ProductName varchar(24) not null,
                                                                Description text,
                                                                Amount int)";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO Coupon (ProductName,Description,Amount) VALUES ('IPhone X','IPhone Discount',150);";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO Coupon (ProductName,Description,Amount) VALUES ('Samsung 10','Samsung Discount',100);";
                    command.ExecuteNonQuery();

                    logger.LogInformation("Migration is done");

                }
                catch (NpgsqlException ex)
                {
                    logger.LogError(ex, "An error occured while migrating");

                    if (retryForAvailability < 50)
                    {
                        retryForAvailability++;
                        System.Threading.Thread.Sleep(2000);
                        MigrateDatabase<TContext>(host, retryForAvailability);

                    }
                }
            }
            return host;
        }
    }
}