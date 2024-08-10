namespace Cympatic.Extensions.Stub.Internal;

internal static class StreamExtensions
{
    public static async Task<string> ReadAsStringAsync(this Stream stream, bool leaveOpen = false)
    {
        using StreamReader reader = new(stream, leaveOpen: leaveOpen);
        var bodyAsString = await reader.ReadToEndAsync();
    
        return bodyAsString;
    }
}
