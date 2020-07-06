using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BooksStore.Api.Middlewares
{
    public class DBHealthCheckProvider :IHealthCheck
    {

        private readonly string defaultQuery = "Select 1 from dbo.books;";
        private string _conString;

        public DBHealthCheckProvider(string conString)
        {
            _conString = conString ?? throw new ArgumentException("ConnectionString can not be null") ;
        }


        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            using (var connection = new SqlConnection(_conString))
            {
                try
                {
                    await connection.OpenAsync(cancellationToken);
                    var command = connection.CreateCommand();
                    command.CommandText = defaultQuery;
                    await command.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {

                    return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
                }
                return HealthCheckResult.Healthy();
            }
        }
    }

    public class APIHealthCheckProvider 
    {
        //public HealthCheckResult Check()
        //{
            
        //}
    }


}
