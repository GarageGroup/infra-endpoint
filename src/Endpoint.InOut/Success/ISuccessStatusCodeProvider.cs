using System;

namespace GGroupp.Infra;

public interface ISuccessStatusCodeProvider<TSuccessStatusCode>
    where TSuccessStatusCode : notnull, Enum
{
    TSuccessStatusCode StatusCode { get; }
}