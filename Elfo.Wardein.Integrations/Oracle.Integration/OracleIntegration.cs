using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elfo.Wardein.Oracle;
using Oracle.ManagedDataAccess.Client;
using Warden;
using Warden.Integrations;
using Warden.Watchers;

namespace Elfo.Wardein.Integrations.Oracle.Integration
{
    public class OracleIntegration : IIntegration
    {
        private readonly OracleConnectionConfiguration configuration;
        private readonly OracleRepository oracleRepository;

        protected OracleIntegration(OracleConnectionConfiguration configuration)
        {
            this.configuration = configuration;
            oracleRepository = new OracleRepository(configuration);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string query, IDictionary<string, object> parameters = null)
        {
            try
            {
                return await oracleRepository.QueryAsync<T>(query, parameters);
            } catch (OracleException exception)
            {
                throw new IntegrationException("There was a SQL error while trying to execute the query.", exception);
            } catch (Exception exception)
            {
                throw new IntegrationException("There was an error while trying to access the Oracle database.",
                    exception);
            }
        }

       

        public async Task<int> ExecuteAsync(string command)
        {

            try
            {
                return await oracleRepository.ExecuteAsync(command);
            } catch (OracleException exception)
            {
               
                throw new IntegrationException("There was a SQL error while trying to execute the command.", exception);
            } catch (Exception exception)
            {
                throw new IntegrationException("There was an error while trying to access the Oracle database.",
                    exception);
            }
        }

        public async Task<int> ExecuteAsync(string command, IDictionary<string, object> parameters)
        {

            try
            {
                return await oracleRepository.ExecuteAsync(command, parameters);
            }
            catch (OracleException exception)
            {

                throw new IntegrationException("There was a SQL error while trying to execute the command.", exception);
            }
            catch (Exception exception)
            {
                throw new IntegrationException("There was an error while trying to access the Oracle database.",
                    exception);
            }
        }

        //public async Task SaveIterationAsync(IWardenIteration iteration)
        //{
        //    var wardenIterationCommand = "insert into WardenIterations values" +
        //                                 "(@wardenName, @ordinal, @startedAt, @completedAt, @executionTime, @isValid);" +
        //                                 "select cast(scope_identity() as bigint)";
        //    var wardenIterationParameters = new Dictionary<string, object>
        //    {
        //        ["wardenName"] = iteration.WardenName,
        //        ["ordinal"] = iteration.Ordinal,
        //        ["startedAt"] = iteration.StartedAt,
        //        ["completedAt"] = iteration.CompletedAt,
        //        ["executionTime"] = iteration.ExecutionTime,
        //        ["isValid"] = iteration.IsValid,
        //    };
        //    var iterationResultIds = await QueryAsync<long>(wardenIterationCommand, wardenIterationParameters);
        //    var iterationId = iterationResultIds.FirstOrDefault();
        //    if (iterationId <= 0)
        //        return;
        //    await SaveWardenCheckResultsAsync(iteration.Results, iterationId);
        //}

        //private async Task SaveWardenCheckResultsAsync(IEnumerable<IWardenCheckResult> results, long iterationId)
        //{
        //    foreach (var result in results)
        //    {
        //        var wardenCheckResultCommand = "insert into WardenCheckResults values (@wardenIteration_Id, @isValid, " +
        //                                       "@startedAt, @completedAt, @executionTime);select cast(scope_identity() as bigint)";
        //        var wardenCheckResultParameters = new Dictionary<string, object>
        //        {
        //            ["wardenIteration_Id"] = iterationId,
        //            ["isValid"] = result.IsValid,
        //            ["startedAt"] = result.StartedAt,
        //            ["completedAt"] = result.CompletedAt,
        //            ["executionTime"] = result.ExecutionTime
        //        };
        //        var wardenCheckResultIds = await QueryAsync<long>(wardenCheckResultCommand, wardenCheckResultParameters);
        //        var wardenCheckResultId = wardenCheckResultIds.FirstOrDefault();
        //        if (wardenCheckResultId <= 0)
        //            return;
        //        await SaveWatcherCheckResultAsync(result.WatcherCheckResult, wardenCheckResultId);
        //        await SaveExceptionAsync(result.Exception, wardenCheckResultId);
        //    }
        //}

        //private async Task SaveWatcherCheckResultAsync(IWatcherCheckResult result, long wardenCheckResultId)
        //{
        //    if (result == null)
        //        return;

        //    var watcherCheckResultCommand = "insert into WatcherCheckResults values " +
        //                                    "(@wardenCheckResult_Id, @watcherName, @watcherType, @description, @isValid)";
        //    var watcherCheckResultParameters = new Dictionary<string, object>
        //    {
        //        ["wardenCheckResult_Id"] = wardenCheckResultId,
        //        ["watcherName"] = result.WatcherName,
        //        ["watcherType"] = result.WatcherType.ToString().Split('.').Last(),
        //        ["description"] = result.Description,
        //        ["isValid"] = result.IsValid
        //    };
        //    await ExecuteAsync(watcherCheckResultCommand, watcherCheckResultParameters);
        //}

        //private async Task<long?> SaveExceptionAsync(Exception exception, long wardenCheckResultId, long? parentExceptionId = null)
        //{
        //    if (exception == null)
        //        return null;

        //    var exceptionCommand = "insert into Exceptions values (@wardenCheckResult_Id, @parentException_Id, " +
        //             "@message, @source, @stackTrace);select cast(scope_identity() as bigint)";
        //    var exceptionParameters = new Dictionary<string, object>
        //    {
        //        ["wardenCheckResult_Id"] = wardenCheckResultId,
        //        ["parentException_Id"] = parentExceptionId,
        //        ["message"] = exception.Message,
        //        ["source"] = exception.Source,
        //        ["stackTrace"] = exception.StackTrace
        //    };
        //    var exceptionIds = await QueryAsync<long>(exceptionCommand, exceptionParameters);
        //    var exceptionId = exceptionIds.FirstOrDefault();
        //    if (exceptionId <= 0)
        //        return null;
        //    if (exception.InnerException == null)
        //        return exceptionId;

        //    await SaveExceptionAsync(exception.InnerException, wardenCheckResultId, exceptionId);

        //    return exceptionId;
        //}

        public static OracleIntegration Create(string connectionString,
           Action<OracleConnectionConfiguration.Builder> configurator)
        {
            var config = new OracleConnectionConfiguration.Builder(connectionString);
            configurator?.Invoke(config);

            return Create(config.Build());
        }

        public static OracleIntegration Create(OracleConnectionConfiguration configuration)
            => new OracleIntegration(configuration);

    }
}
