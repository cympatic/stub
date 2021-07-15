using System;

namespace Cympatic.Extensions.SpecFlow.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SpecFlowItemAttribute : Attribute
    {
        public string Name { get; private set; }

        public SpecFlowItemAttribute(string name = default)
        {
            Name = name;
        }
    }
}
