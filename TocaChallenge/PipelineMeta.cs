namespace TocaChallenge
{
    public class PipelineMetaRequest
    {
        public string WorkflowId { get; set; }
        public string WorkflowName { get; set; }
        public string WorkflowOwnerId { get; set; }
        public string ExecutorUserId { get; set; }
        public string ProjectId { get; set; }
        public string PipelineId { get; set; }
        public string ProjectName { get; set; }
        public string JobId { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public int ExecutionType { get; set; }
    }

    public class PipelineMetaResponse
    {
        public string SessionId { get; set; }
        public string ServiceId { get; set; }
        public string Status { get; set; }
        public string Exception { get; set; }
        public PipelineMetaContent Content { get; set; }

    }

    public class PipelineMetaContent
    {
        public string PipelineId { get; set; }
    }

    public enum JobStatus
    {
        NotProcessed = 0,
        Queued = 1,
        BotsLocked = 2,
        Processing = 3,
        PreProcessed = 4,
        Failed = 5,
        Complete = 6
    }
}
