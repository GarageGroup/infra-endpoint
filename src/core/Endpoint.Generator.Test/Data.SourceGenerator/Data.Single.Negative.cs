namespace GarageGroup.Infra.Endpoint.Generator.Test;

partial class EndpointSourceGeneratorData
{
    internal const string GenericEndpointTypeSourceCode
        =
        """
        using System.Threading;
        using System.Threading.Tasks;
        using GarageGroup.Infra;

        namespace Demo
        {
            [Endpoint("generic", EndpointMethod.Get, "/generic")]
            public interface IGenericFunc<TInput>
            {
                ValueTask<GenericOut> InvokeAsync(GenericIn input, CancellationToken cancellationToken);
            }

            public sealed record class GenericIn;

            public sealed record class GenericOut;
        }
        """;

    internal const string EndpointWithoutMethodSourceCode
        =
        """
        using System.Threading.Tasks;
        using GarageGroup.Infra;

        namespace Demo
        {
            [Endpoint("invalid", EndpointMethod.Get, "/invalid")]
            public interface IInvalidFunc
            {
                ValueTask<InvalidOut> InvokeAsync(InvalidIn input);
            }

            public sealed record class InvalidIn;

            public sealed record class InvalidOut;
        }
        """;

    internal const string EndpointWithWhiteSpaceNameSourceCode
        =
        """
        using System.Threading;
        using System.Threading.Tasks;
        using GarageGroup.Infra;

        namespace Demo
        {
            [Endpoint("   ", EndpointMethod.Get, "/invalid-name")]
            public interface IInvalidNameFunc
            {
                ValueTask<InvalidNameOut> InvokeAsync(InvalidNameIn input, CancellationToken cancellationToken);
            }

            public sealed record class InvalidNameIn;

            public sealed record class InvalidNameOut;
        }
        """;
}