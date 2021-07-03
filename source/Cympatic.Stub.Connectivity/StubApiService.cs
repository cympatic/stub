﻿using Cympatic.Stub.Connectivity.Extensions;
using Cympatic.Stub.Connectivity.Interfaces;
using Cympatic.Stub.Connectivity.Internal;
using System;
using System.Net.Http;

namespace Cympatic.Stub.Connectivity
{
    public abstract class StubApiService
    {
        protected HttpClient InternalHttpClient { get; }

        protected IClientStub DefaultClientStub { get; }

        protected StubApiService(HttpClient httpClient)
        {
            InternalHttpClient = httpClient;

            DefaultClientStub = new DefaultClientStub();
        }

        protected static void EnsureClientStubValid(IClientStub clientStub)
        {
            if (clientStub is null)
            {
                throw new ArgumentNullException(nameof(clientStub));
            }
        }

        protected void EnsureHeadersValid(IClientStub clientStub)
        {
            if (!InternalHttpClient.DefaultRequestHeaders.HasValidHeaders(clientStub.IdentifierHeaderName))
            {
                throw new ArgumentException("HttpClient doesn't contain the correct headers");
            }
        }
    }
}