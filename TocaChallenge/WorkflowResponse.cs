using System;
using System.Collections.Generic;

public class Parameter
{
    public string Key { get; set; }
    public string Type { get; set; }
    public object Value { get; set; }
}

public class NodeDataArray
{
    public string Category { get; set; }
    public string Text { get; set; }
    public int Key { get; set; }
    public string Loc { get; set; }
    public List<Parameter> Parameters { get; set; }
}

public class Anchors
{
    public double X1 { get; set; }
}

public class LinkData
{
    public List<List<double>> Path { get; set; }
    public int LabelPart { get; set; }
    public List<double> LabelOffset { get; set; }
    public Anchors Anchors { get; set; }
}

public class LinkDataArray
{
    public int From { get; set; }
    public int To { get; set; }
    public object FromPort { get; set; }
    public object ToPort { get; set; }
    public object Points { get; set; }
    public LinkData LinkData { get; set; }
}

public class Pointer
{
    public string PointsTo { get; set; }
    public string Expression { get; set; }
}

public class Workflow
{
    public string Type { get; set; }
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<Pointer> Pointers { get; set; }
    public List<Parameter> Parameters { get; set; }
    public int Status { get; set; }
}

public class WorkflowContent
{
    public string Id { get; set; }
    public string WorkflowId { get; set; }
    public string AssociatedUserId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<NodeDataArray> NodeDataArray { get; set; }
    public List<LinkDataArray> LinkDataArray { get; set; }
    public List<Workflow> Workflow { get; set; }
    public object ChronStamp { get; set; }
    public string ProjectId { get; set; }
    public List<object> Configuration { get; set; }
    public DateTime Created { get; set; }
    public string CreatedBy { get; set; }
    public DateTime Updated { get; set; }
    public string UpdatedBy { get; set; }
    public bool Deactivated { get; set; }
}

public class WorkflowResponse
{
    public object SessionId { get; set; }
    public object ServiceId { get; set; }
    public string Status { get; set; }
    public object Exception { get; set; }
    public List<WorkflowContent> Content { get; set; }
}

