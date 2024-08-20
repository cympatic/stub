using System.Security.Cryptography.X509Certificates;

namespace Cympatic.Extensions.Stub;

public sealed class StubServerOptions
{
    private bool _useSsl;
    public bool UseSsl {
        get
        { 
            return _useSsl;
        }

        set
        {
            if (value && !OperatingSystem.IsWindows())
            { 
                throw new PlatformNotSupportedException("Default Ssl is only supported on Windows! Use ServerCertificateSelector to select the certificate");
            }

            _useSsl = value;
        } 
    }

    public Func<X509Certificate2?>? ServerCertificateSelector {  get; set; }

    public Func<HttpMessageHandler?>? ConfigureHttpClientHandler { get; set; }
}
