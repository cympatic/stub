using System;
using System.Collections.Generic;
using System.Net;

namespace Cympatic.Stub.Connectivity.Models;

public class ResponseModel : StubModel
{
    public IList<string> HttpMethods { get; set; }

    public HttpStatusCode ReturnStatusCode { get; set; }

    public Uri Location { get; set; }

    public object Result { get; set; }

    public int DelayInMilliseconds { get; set; }
}