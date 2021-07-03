using Microsoft.AspNetCore.Http;

namespace Cympatic.Stub.Server.UnitTests
{
    public class RouteTransformerTests
    {
        private readonly RouteTransformer _sut;
        private readonly HttpContext _context;

        public RouteTransformerTests()
        {
            _sut = new RouteTransformer();
            _context = new DefaultHttpContext();
        }
    }
}
