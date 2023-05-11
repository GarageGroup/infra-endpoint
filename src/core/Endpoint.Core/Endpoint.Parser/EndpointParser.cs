using System;
using System.Globalization;

namespace GarageGroup.Infra.Endpoint;

public static partial class EndpointParser
{
    private static IFormatProvider InvariantFormatProvider
        =>
        CultureInfo.InvariantCulture;

    private static NumberStyles DecimalNumberStyle
        =>
        NumberStyles.Number;

    private static NumberStyles FloatNumberStyle
        =>
        NumberStyles.Float | NumberStyles.AllowThousands;

    private static NumberStyles IntegerNumberStyle
        =>
        NumberStyles.Integer;

    private static DateTimeStyles DefaultDateTimeStyle
        =>
        DateTimeStyles.None;

    private static Result<T?, Failure<Unit>> ParseNullable<T>(string? source, Func<string, Result<T, Failure<Unit>>> parser)
        where T : struct
        =>
        string.IsNullOrEmpty(source) ? null : parser.Invoke(source).MapSuccess(ToNullable);

    private static Failure<Unit> CreateFailure(string? value, string type)
        =>
        Failure.Create($"Value '{value}' is not a {type} value");

    private static T? ToNullable<T>(T source)
        where T : struct
        =>
        source;
}