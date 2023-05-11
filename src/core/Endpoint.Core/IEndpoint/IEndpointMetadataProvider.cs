#if NET6_0
using System;
using System.Linq;
using System.Reflection;
#endif

namespace GarageGroup.Infra.Endpoint;

public interface IEndpointMetadataProvider
{
#if NET7_0_OR_GREATER
    static abstract EndpointMetadata GetEndpointMetadata();
#else
    public static EndpointMetadata GetEndpointMetadata<TEndpoint>()
        where TEndpoint : IEndpointMetadataProvider
    {
        const string methodName = "GetEndpointMetadata";
        var methodInfo = typeof(TEndpoint).GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);

        if (methodInfo is null)
        {
            throw CreateMethodNotFoundException();
        }

        if (methodInfo.GetParameters().Any())
        {
            throw CreateInvalidParameterMethodException();
        }

        if (methodInfo.ReturnType.IsAssignableFrom(typeof(EndpointMetadata)) is false)
        {
            throw CreateUnexpectedReturnTypeException(methodInfo.ReturnType);
        }

        var metadata = methodInfo.Invoke(null, null);

        if (metadata is null)
        {
            throw CreateResultMustBeNotNullException();
        }

        return (EndpointMetadata)metadata;

        static InvalidOperationException CreateMethodNotFoundException()
            =>
            new($"A public static method '{methodName}' must be specified in the type '{typeof(TEndpoint)}'");

        static InvalidOperationException CreateInvalidParameterMethodException()
            =>
            new($"The method '{methodName}' of the type '{typeof(TEndpoint)}' must not contain any parameters");

        static InvalidOperationException CreateUnexpectedReturnTypeException(Type returnType)
            =>
            new($"The method '{methodName}' of the type '{typeof(TEndpoint)}' must return '{typeof(EndpointMetadata)}' instead '{returnType}'");

        static InvalidOperationException CreateResultMustBeNotNullException()
            =>
            new($"The method '{methodName}' of the type '{typeof(TEndpoint)}' result must not be null");
    }
#endif
}