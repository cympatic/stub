using System;
using System.Runtime.CompilerServices;

namespace Cympatic.Extensions.Stub.SpecFlow.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreAttribute : Attribute
    {
        public string PropertyName { get; private set; }

        public IgnoreAttribute([CallerMemberName] string propertyName = default)
        {
            PropertyName = propertyName;
        }
    }
}
