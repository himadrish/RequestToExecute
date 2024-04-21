using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json.Serialization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
//
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;

//using System.Threading.Tasks;
//using Newtonsoft.Json;
//using System.Web
//using Microsoft.Azure.WebJobs;
//using Microsoft.Azure.WebJobs.Extensions.Http;
//using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
//using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
//using Microsoft.OpenApi.Models;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;


namespace RequestToExecute
{
    //async call
    public static class ExecuteRequest
    {
        const double revenuePerkW = 0.12;
        const double technicianCost = 250;
        const double turbineCost = 100;

        [FunctionName("GetDummyCall")]
        [OpenApiOperation(operationId: "Run")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody("application/json", typeof(RequestBodyModel),
            Description = "JSON request body containing { hours, capacity}")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string),
            Description = "The OK response message containing a JSON result.")]
        public static async Task<IActionResult> Run(
            [Microsoft.Azure.Functions.Worker.HttpTrigger(Microsoft.Azure.Functions.Worker.AuthorizationLevel.Function, "post", Route = null)] System.Web.HttpRequest req,
            ILogger log)
        {
            //get the request body
            string requestBody = await new StreamReader(req.GetBufferlessInputStream()).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            int? capacity = data?.capacity;
            int? hours = data?.hours;

            // Return bad request if capacity or hours are not passed in
            if (capacity == null || hours == null)
            {
                return new BadRequestObjectResult("Please pass capacity and hours in the request body");
            }
            // Formulas to calculate revenue and cost
            double? revenueOpportunity = capacity * revenuePerkW * 24;
            double? costToFix = (hours * technicianCost) + turbineCost;
            string repairTurbine;

            if (revenueOpportunity > costToFix)
            {
                repairTurbine = "Yes";
            }
            else
            {
                repairTurbine = "No";
            };

            return (ActionResult)new OkObjectResult(new
            {
                message = repairTurbine,
                revenueOpportunity = "$" + revenueOpportunity,
                costToFix = "$" + costToFix
            });
        }
    }
    public class RequestBodyModel
    {
        public int Hours { get; set; }
        public int Capacity { get; set; }
    }

    //sync call
    //public class ExecuteRequest
    //{
    //    private readonly ILogger _logger;
    //    private Uri defaultUri = new Uri("https://jsonplaceholder.typicode.com/todos/1");

    //    public ExecuteRequest(ILoggerFactory loggerFactory)
    //    {
    //        _logger = loggerFactory.CreateLogger<ExecuteRequest>();
    //    }

    //    [Function("GetDummyCall")]
    //    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
    //    {
    //        _logger.LogInformation("HTTP trigger AZ function to process a request.");
    //        string responseMsg = string.Empty;

    //        var response = req.CreateResponse();
    //        response.Headers.Add("Content-Type", "application/json");

    //        if (response.StatusCode == HttpStatusCode.OK)
    //        {
    //            // Get request body data.
    //            string requestBody = new StreamReader(req.Body).ReadToEnd();
    //            dynamic data = JsonConvert.DeserializeObject(requestBody);
    //            int? deptID = data?.DepartmentId;
    //            int? empCount = data?.Employees.Count;
    //            string deptName = data?.DepartmentName;
    //            responseMsg = "DeptID: " + deptID + ", Dept Name: " + deptName + ", Empcount: " + empCount;
    //        }
    //        else
    //        {
    //            responseMsg = "No data found";
    //        }

    //        response.WriteString(responseMsg);

    //        return response;
    //    }
    //}



}
