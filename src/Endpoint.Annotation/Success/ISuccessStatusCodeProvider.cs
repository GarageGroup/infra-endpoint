using System;

namespace GarageGroup.Infra;

public interface ISuccessStatusCodeProvider<TSuccessStatusCode>
    where TSuccessStatusCode : notnull, Enum
{
    TSuccessStatusCode StatusCode { get; }
}