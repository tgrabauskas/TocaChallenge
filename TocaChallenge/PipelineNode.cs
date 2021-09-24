namespace TocaChallenge
{
    public class PipelineNodeRequest
    {
        public string PipelineId { get; set; }
        public string JobId { get; set; }
        public string NodeId { get; set; }
        public string PipelineNodeId { get; set; }
        public string ActivityId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string BotId { get; set; }
        public string Type { get; set; }
        public object NodeMeta { get; set; }
        public string Status { get; set; }
    }

    public class PipelineNodeResponse
    {
        public string SessionId { get; set; }
        public string ServiceId { get; set; }
        public string Status { get; set; }
        public string Exception { get; set; }
        public PipelineNodeContent Content { get; set; }

    }

    public class PipelineNodeContent
    {
        public string PipelineNodeId { get; set; }
    }
}
