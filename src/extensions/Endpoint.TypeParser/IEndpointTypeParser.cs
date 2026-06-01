using System;

namespace GarageGroup.Infra;

public interface IEndpointTypeParser<T>
{
    static abstract Result<T, Failure<Unit>> Parse(string? source);
}