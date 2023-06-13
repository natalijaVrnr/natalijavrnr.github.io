using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using WebScraper_ClassLibrary;
using System.Data;
using Dapper;
using Microsoft.OpenApi.Models;
using RestSharp;
using System.Linq;

namespace FunctionApp_DataAccess.Api
{
    public class SqlApi
    {
        private static string _connectionString;

        public SqlApi(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("Default");
        }

        [FunctionName("GetAllParticipants")] // gets all the participants - both qualified and non-qualified
        public async Task<IActionResult> GetAllParticipants(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "getallparticipants")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function GetAllParticipants processed a request.");


            try
            {
                string sqlCommandText = "SELECT * FROM allParticipants ORDER BY yearOfParticipating DESC, country ASC";

                List<ParticipantModel> participants = await GetDataFromSql<ParticipantModel>(sqlCommandText, _connectionString);

                return new OkObjectResult(participants);
            }
            catch (Exception ex)
            {
                log.LogError($"{ex.Message}");
                return new BadRequestObjectResult(ex);
            }

        }

        [FunctionName("GetAllParticipantsSorted")]
        public async Task<IActionResult> GetAllParticipantsSorted(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "getallparticipantssorted/{orderByProperties}")] HttpRequest req,
            ILogger log, string orderByProperties)
        {
            log.LogInformation("C# HTTP trigger function GetAllParticipantsSorted processed a request.");

            string[] orderByEls = orderByProperties.Split('x')
                  .Select(x => x)
                  .ToArray();

            try
            {
                string sqlCommandText = @$"SELECT * FROM allParticipants 
                    ORDER BY yearOfParticipating {orderByEls[0]}";

                if (orderByEls[1] != "null")
                {
                    sqlCommandText += $", country {orderByEls[1]}";
                }

                if (orderByEls[2] != "null") 
                {
                    sqlCommandText += $", artist {orderByEls[2]}";
                }
                if (orderByEls[3] != "null") 
                {
                    sqlCommandText += $", song {orderByEls[3]}";
                }
                if (orderByEls[4] != "null")
                {
                    sqlCommandText += $", qualifiedForFinal {orderByEls[4]}";
                }

                List<ParticipantModel> participants = await GetDataFromSql<ParticipantModel>(sqlCommandText, _connectionString);

                return new OkObjectResult(participants);
            }
            catch (Exception ex)
            {
                log.LogError($"{ex.Message}");
                return new BadRequestObjectResult(ex);
            }

        }

        [FunctionName("GetAllFinalists")] 
        public async Task<IActionResult> GetAllFinalists(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "getallfinalists")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function GetAllFinalists processed a request.");


            try
            {
                string sqlCommandText = @"select allParticipants.yearOfParticipating, finalResults.place, 
allParticipants.country, allParticipants.artist, allParticipants.song, finalResults.score
                                        from allParticipants
                                        join finalResults on allParticipants.id = finalResults.participantId
                                        order by allParticipants.yearOfParticipating DESC, finalResults.place ASC";

                List<FinalistDataModel> participants = await GetDataFromSql<FinalistDataModel>(sqlCommandText, _connectionString);

                return new OkObjectResult(participants);
            }
            catch (Exception ex)
            {
                log.LogError($"{ex.Message}");
                return new BadRequestObjectResult(ex);
            }

        }

        [FunctionName("GetParticipantById")] // gets all the participants from the given country
        public async Task<IActionResult> GetParticipantById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "getparticipantbyid")] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var userData = JsonConvert.DeserializeObject<GetParticipantByIdRequest>(requestBody);

            log.LogInformation("C# HTTP trigger function GetParticipantsById processed a request.");

            string sqlCommandText = $"SELECT * FROM allParticipants WHERE id = {userData.Id}";

            try
            {
                List<ParticipantModel> participants = await GetDataFromSql<ParticipantModel>(sqlCommandText, _connectionString);

                return new OkObjectResult(participants);
            }
            catch (Exception ex)
            {
                log.LogError($"{ex.Message}");
                return new BadRequestObjectResult(ex);
            }

        }


        //[FunctionName("GetParticipantsByCountryAndYear")] // gets all the participants from the given country
        //public async Task<IActionResult> GetParticipantsByCountryAndYear(
        //    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "getparticipantsbycountryandyear")] GetParticipantsByCountryAndYearRequest req,
        //    ILogger log)
        //{
        //    log.LogInformation("C# HTTP trigger function GetParticipantsByCountryAndYear processed a request.");

        //    string sqlCommandText = "";

        //    try
        //    {
        //        if (!string.IsNullOrEmpty(req.Country) && !string.IsNullOrEmpty(req.Year)) 
        //        {
        //            sqlCommandText = $"SELECT * FROM allParticipants WHERE country like '{req.Country}%' AND yearOfParticipating = {req.Year}";
        //        }

        //        else 
        //        {
        //            if (!string.IsNullOrEmpty(req.Country)) 
        //            {
        //                sqlCommandText = $"SELECT * FROM allParticipants WHERE country like '{req.Country}%' ORDER BY yearOfParticipating DESC";
        //            }

        //            else if (!string.IsNullOrEmpty(req.Year))
        //            {
        //                sqlCommandText = $"SELECT * FROM allParticipants WHERE yearOfParticipating = {req.Year} ORDER BY country ASC";
        //            }
        //        }


        //        List<ParticipantModel> participants = await GetDataFromSql<ParticipantModel>(sqlCommandText, _connectionString);

        //        return new OkObjectResult(participants);
        //    }
        //    catch (Exception ex)
        //    {
        //        log.LogError($"{ex.Message}");
        //        return new BadRequestObjectResult(ex);
        //    }

        //}

        //[FunctionName("GetAllQualifiedParticipants")] // gets all the participants - both qualified and non-qualified
        //public async Task<IActionResult> GetAllQualifiedParticipants(
        //    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "getallqualifiedparticipants")]
        //    ILogger log)
        //{
        //    log.LogInformation("C# HTTP trigger function GetAllQualifiedParticipants processed a request.");

        //    try
        //    {
        //        string sqlCommandText = "SELECT * FROM allParticipants WHERE qualifiedForFinal = 'true' ORDER BY yearOfParticipating DESC, country ASC";

        //        List<ParticipantModel> participants = await GetDataFromSql<ParticipantModel>(sqlCommandText, _connectionString);

        //        return new OkObjectResult(participants);
        //    }
        //    catch (Exception ex)
        //    {
        //        log.LogError($"{ex.Message}");
        //        return new BadRequestObjectResult(ex);
        //    }

        //}


        public static async Task<List<T>> GetDataFromSql<T> (string sqlCommandText, string connectionString) 
        {
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                var result =  await connection.QueryAsync<T>(sqlCommandText);

                return result.AsList();
            }

        }


    }
}
