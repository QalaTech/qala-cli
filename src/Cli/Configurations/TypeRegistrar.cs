using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Qala.Cli.Configurations;

public sealed class TypeRegistrar(IServiceCollection builder) : ITypeRegistrar
{
    public ITypeResolver Build()
    {
        return new TypeResolver(builder.BuildServiceProvider());
    }

    public void Register(Type service, Type implementation)
    {
        builder.AddSingleton(service, implementation);
    }

    public void RegisterInstance(Type service, object implementation)
    {
        builder.AddSingleton(service, implementation);
    }

    public void RegisterLazy(Type service, Func<object> func)
    {
        if (func is not null)
        {
            builder.AddSingleton(service, (provider) => func());
        }
        else
        {
            throw new ArgumentNullException(nameof(func));
        }
    }
}