namespace Cocona;

/// <summary>
/// Specifies the parameter that should be set by dependency injection.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
public sealed class FromServiceAttribute : Attribute;
