using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointParser
{
    public static Result<IReadOnlyCollection<T>, Failure<Unit>> ParseCollection<T>(
        [AllowNull] IEnumerable<string?> source, Func<string?, Result<T, Failure<Unit>>> itemParser)
    {
        if (source is null || source.Any() is false)
        {
            return Result.Success<IReadOnlyCollection<T>>(Array.Empty<T>());
        }

        var items = new List<T>();

        foreach (var sourceItem in source)
        {
            var itemResult = itemParser.Invoke(sourceItem);
            if (itemResult.IsFailure)
            {
                return itemResult.FailureOrThrow();
            }

            items.Add(itemResult.SuccessOrThrow());
        }

        return Result.Success<IReadOnlyCollection<T>>(items);
    }
}