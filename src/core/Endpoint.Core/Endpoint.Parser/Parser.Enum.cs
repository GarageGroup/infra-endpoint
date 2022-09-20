using System;

namespace GGroupp.Infra.Endpoint;

partial class EndpointParser
{
    public static Result<TEnum?, Failure<Unit>> ParseNullableEnum<TEnum>(string? source)
        where TEnum : struct, Enum
        =>
        ParseNullable(source, ParseEnum<TEnum>);

    public static Result<TEnum, Failure<Unit>> ParseEnum<TEnum>(string? source)
        where TEnum : struct, Enum
        =>
        Enum.TryParse<TEnum>(source, false, out var result) ? result : CreateFailure(source, typeof(TEnum).Name);
}