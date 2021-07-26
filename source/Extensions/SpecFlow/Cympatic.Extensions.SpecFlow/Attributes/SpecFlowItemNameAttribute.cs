using System;

namespace Cympatic.Extensions.SpecFlow.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SpecFlowItemNameAttribute : Attribute
    {
        public string Name { get; private set; }

        public SpecFlowItemNameAttribute(string name = default)
        {
            Name = name;
        }
    }
}
