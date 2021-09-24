using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace TocaChallenge
{
    class Program
    {
        private const string url = "https://clientgateway.test1.genesis.tocabot.io";
        private static string jwt;
        private const string username = "tom.grabauskas@gmail.com";
        private const string projectId = "f5555a42-ce57-427d-a560-cbfa53ed9cf3";
        private static string pipelineId;
        private static string jobId = Guid.NewGuid().ToString();

        private static JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        static void Main(string[] args)
        {
            UserCredentials uc = new UserCredentials();
            uc.Username = username;
            uc.Password = "b2441c2c22e9449a8a13c465bf9b9bf3";

            var jwtResponseString = GetJwtToken(uc);
            jwt = JsonSerializer.Deserialize<JwtResponse>(jwtResponseString, jsonSerializerOptions).Content.Token;
            var workflowResponseString = GetWorkflowData();
            var workflowResponse = JsonSerializer.Deserialize<WorkflowResponse>(workflowResponseString, jsonSerializerOptions);

            foreach (WorkflowContent wfc in workflowResponse.Content)
            {
                RunWorkflow(wfc);
            }
        }

        private static string GetJwtToken(UserCredentials uc)
        {
            var endpoint = "/api/1/Authentication/validate";

            var httpRequest = (HttpWebRequest)WebRequest.Create(url + endpoint);
            httpRequest.Method = "POST";
            httpRequest.Accept = "application/json";
            httpRequest.ContentType = "application/json";

            var data = JsonSerializer.Serialize(uc);

            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }

            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            string result;

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            return result;
        }

        private static string GetWorkflowData()
        {
            var workflowId = "f26566cc-e29b-42c1-bbd3-49dfa9a547c5";
            var endpoint = "/api/1/Workflow/get";
            
            var httpRequest = (HttpWebRequest)WebRequest.Create(url + endpoint);
            httpRequest.Method = "POST";
            httpRequest.Accept = "application/json";
            httpRequest.ContentType = "application/plain";
            httpRequest.Headers.Add("Authorization", $"Bearer {jwt}");

            var data = JsonSerializer.Serialize(new WorkflowRequest() { WorkflowId = workflowId });

            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }

            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            string result;

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            return result;
        }

        private static void RunWorkflow(WorkflowContent wfc)
        {
            pipelineId = UpsertPipelineMeta(wfc.WorkflowId, wfc.Name, wfc.AssociatedUserId, JobStatus.Processing);
            ValidateWorkflow(wfc);

            foreach(NodeDataArray nd in wfc.NodeDataArray)
            {
                Workflow wf = nd.Category == "Start" ? wfc.Workflow.Where(w => w.Type == "Start").FirstOrDefault() :
                    nd.Category == "End" ? wfc.Workflow.Where(w => w.Type == "End").FirstOrDefault() :
                    nd.Category == "Activity" ? wfc.Workflow.Where(w => w.Id == nd.Parameters.Where(p => p.Key == "ActivityId").FirstOrDefault().Value.ToString()).FirstOrDefault() : null;

                UpsertPipelineNode(nd, wf);
            }
        }

        private static string UpsertPipelineMeta(string workflowId, string workflowName, string workflowOwnerId, JobStatus jobStatus,
            string exceptionMessage = null)
        {
            var endpoint = $"/api/2/Pipeline/UpsertPipeline";

            var httpRequest = (HttpWebRequest)WebRequest.Create(url + endpoint);
            httpRequest.Method = "POST";
            httpRequest.Accept = "application/json";
            httpRequest.ContentType = "application/json";
            httpRequest.Headers.Add("Authorization", $"Bearer {jwt}");

            var piplineMeta = new PipelineMetaRequest()
            {
                WorkflowId = workflowId,
                WorkflowName = workflowName,
                WorkflowOwnerId = workflowOwnerId,
                ExecutorUserId = workflowOwnerId,
                ProjectId = projectId,
                PipelineId = pipelineId != null ? pipelineId : null,
                JobId = jobId,
                Status = jobStatus.ToString(),
                Message = exceptionMessage != null ? exceptionMessage : null,
                ExecutionType = 0
            };

            var data = JsonSerializer.Serialize(piplineMeta, new JsonSerializerOptions() { IgnoreNullValues = true });

            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }

            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            string result;

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            return JsonSerializer.Deserialize<PipelineMetaResponse>(result, jsonSerializerOptions).Content.PipelineId;
        }

        private static string UpsertPipelineNode(NodeDataArray nd, Workflow wf)
        {
            var endpoint = $"/api/2/Pipeline/UpsertNode";

            var httpRequest = (HttpWebRequest)WebRequest.Create(url + endpoint);
            httpRequest.Method = "POST";
            httpRequest.Accept = "application/json";
            httpRequest.ContentType = "application/json";
            httpRequest.Headers.Add("Authorization", $"Bearer {jwt}");

            var piplineNode = new PipelineNodeRequest()
            {
                PipelineId = pipelineId,
                JobId = jobId,
                NodeId = wf != null ? wf.Id : null,
                ActivityId = nd.Category == "Activity" ? nd.Parameters.Where(p => p.Key == "ActivityId").FirstOrDefault().Value.ToString() : null,
                Status = "Processing",
                Type = nd.Category
            };

            var data = JsonSerializer.Serialize(piplineNode, new JsonSerializerOptions() { IgnoreNullValues = true });

            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }

            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            string result;

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            return JsonSerializer.Deserialize<PipelineNodeResponse>(result, jsonSerializerOptions).Content.PipelineNodeId;
        }

        private static void ValidateWorkflow(WorkflowContent wfc)
        {
            string exceptionMessage = null;

            if (wfc == null)
            {
                exceptionMessage = "Could not retrieve Workflow. Please Check the Assigned Workflow is Valid";
            }

            if(wfc.NodeDataArray.Count == 0)
            {
                exceptionMessage = "The workflow contains no nodes to validate or execute.";
                throw new Exception(exceptionMessage);
            }

            if (wfc.NodeDataArray.Where(n => n.Category == "Start").ToList().Count == 0)
            {
                exceptionMessage = "No start node defined.";
            }

            if (wfc.NodeDataArray.Where(n => n.Category == "Start").ToList().Count > 1)
            {
                exceptionMessage = "More than one start node defined";
            }

            if (wfc.NodeDataArray.Where(n => n.Category == "End").ToList().Count == 0)
            {
                exceptionMessage = "No start node defined.";
            }

            if (wfc.NodeDataArray.Where(n => n.Category == "Parallel").ToList().Count % 2 == 1)
            {
                exceptionMessage = "Odd number of parallel nodes found. Check that all parallel nodes are closed.";
            }

            if(exceptionMessage != null)
            {
                UpsertPipelineMeta(wfc.WorkflowId, wfc.Name, wfc.AssociatedUserId, JobStatus.Failed, exceptionMessage);
                throw new Exception(exceptionMessage);
            }
        }
    }
}
