using System;

namespace Cympatic.Extensions.Stub.SpecFlow.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class SpecFlowItemNameAttribute : Attribute
{
    public string Name { get; private set; }

    public SpecFlowItemNameAttribute(string name = default)
    {
        Name = name;
    }
}
