using Cocona.Application;

namespace Cocona.Lite;

public class CoconaLiteInstanceActivator : ICoconaInstanceActivator
{
    public object? GetServiceOrCreateInstance(IServiceProvider serviceProvider, Type instanceType)
    {
        return serviceProvider.GetService(instanceType) ?? CreateInstance(serviceProvider, instanceType, []);
    }

    public object? CreateInstance(IServiceProvider serviceProvider, Type instanceType, object[]? parameters)
    {
        if (parameters is { Length: > 0 }) throw new NotSupportedException("SimpleCoconaInstanceActivator doesn't support extra arguments.");
        return SimpleActivator.CreateInstance(serviceProvider, instanceType);
    }
}
