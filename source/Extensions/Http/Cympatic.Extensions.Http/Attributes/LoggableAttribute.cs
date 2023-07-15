using System;

namespace Cympatic.Extensions.Http.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class LoggableAttribute : Attribute
{
}
