using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace DurableFunctionsSandbox
{
    public class DurableSandboxFunction
    {
        [FunctionName("DurableSandboxFunction_QueueStart")]
        public async Task Run(
        [QueueTrigger("sandbox-myqueue-items", Connection = "StorageConnectionString")] string myQueueItem,
        [DurableClient] IDurableOrchestrationClient starter,
        ILogger log)
        {
            // Function input comes from the request content.
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
            string instanceId = await starter.StartNewAsync("DurableSandboxFunction", null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
        }

        /// <summary>
        /// Orchestrator Function
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [FunctionName("DurableSandboxFunction")]
        public async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();

            // オーケストレーター内で30秒待機させる TODO: 30秒間隔でTriggerを発火させたい。
            //await context.CreateTimer(context.CurrentUtcDateTime.AddSeconds(30), CancellationToken.None);

            // Replace "hello" with the name of your Durable Activity Function.
            outputs.Add(await context.CallActivityAsync<string>("DurableSandboxFunction_Hello", "Tokyo"));
            outputs.Add(await context.CallActivityAsync<string>("DurableSandboxFunction_Hello", "Seattle"));
            outputs.Add(await context.CallActivityAsync<string>("DurableSandboxFunction_Hello", "London"));

            return outputs;
        }

        /// <summary>
        /// Activity Function
        /// </summary>
        /// <param name="name"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("DurableSandboxFunction_Hello")]
        public string SayHello([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($"Saying hello to {name}.");
            return $"Hello {name}!";
        }


    }
}