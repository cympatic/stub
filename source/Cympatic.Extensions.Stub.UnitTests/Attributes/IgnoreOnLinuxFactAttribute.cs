namespace Cympatic.Extensions.Stub.UnitTests.Attributes;

public class IgnoreOnLinuxFactAttribute : FactAttribute
{
    public IgnoreOnLinuxFactAttribute()
    {
        if (OperatingSystem.IsLinux())
        {
            Skip = "Ignore on Linux";
        }
    }
}
