using System;

#region RequestMessage
public interface RequestDataMessage
{
    Guid DataId { get; set; }
    string String { get; set; }
}
#endregion