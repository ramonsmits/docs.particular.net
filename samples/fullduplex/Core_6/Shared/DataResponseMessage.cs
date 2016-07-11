using System;

#region ResponseMessage
public interface DataResponseMessage
{
    Guid DataId { get; set; }
    string String { get; set; }
}
#endregion
