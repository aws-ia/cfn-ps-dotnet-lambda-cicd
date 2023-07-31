using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;

using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using System;
using System.IO;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly:LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ServerlessExample
{

    public class Function
    {

        private static readonly HttpClient client = new HttpClient();

        private static async Task<string> GetCallingIP()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", "AWS Lambda .Net Client");

            var msg = await client.GetStringAsync("http://checkip.amazonaws.com/").ConfigureAwait(continueOnCapturedContext:false);

            return msg.Replace("\n","");
        }

        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest apigProxyEvent, ILambdaContext context)
        {
            context.Logger.LogLine("Entry\n");
            context.Logger.LogLine(System.Text.Json.JsonSerializer.Serialize(apigProxyEvent));
            try
            {
            if (apigProxyEvent?.Path?.Equals("/favicon.ico") == true)
            {
                return new APIGatewayProxyResponse
                {
                    Body = Convert.ToBase64String(File.ReadAllBytes("favicon.ico")),
                    StatusCode = 200,
                    IsBase64Encoded = true,
                    Headers = new Dictionary<string, string> { { "Content-Type", "image/vnd.microsoft.icon" } }
                };
            }
            } catch (Exception e) {
                context.Logger.LogLine(e.ToString());
            }

            var location = await GetCallingIP();
            var body = new Dictionary<string, string>
            {
                { "message", "hello world" },
                { "location", location }
            };

            return new APIGatewayProxyResponse
            {
                Body = System.Text.Json.JsonSerializer.Serialize(body),
                StatusCode = 200,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
    }
}
