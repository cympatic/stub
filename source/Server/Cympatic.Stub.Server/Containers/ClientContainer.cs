using Cympatic.Stub.Connectivity.Models;
using Cympatic.Stub.Server.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Cympatic.Stub.Server.Containers;

public class ClientContainer : IDisposable, IClientContainer
{
    private const string DefaultIdentifierValue = "default";

    private readonly HashSet<ClientInfo> _internalContainer;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly Func<ResponseModelContainer> _responseModelContainerFactory;
    private readonly Func<RequestModelContainer> _requestModelContainerFactory;
    private readonly ILogger _logger;

    private bool disposedValue = false;
    private ReaderWriterLockSlim _cacheLock = new();

    public ClientContainer(
        IHttpContextAccessor httpContextAccessor,
        Func<ResponseModelContainer> responseModelContainerFactory,
        Func<RequestModelContainer> requestModelContainerFactory,
        ILogger<ClientContainer> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _responseModelContainerFactory = responseModelContainerFactory;
        _requestModelContainerFactory = requestModelContainerFactory;
        _logger = logger;

        _internalContainer = new HashSet<ClientInfo>();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public ClientModel Add(string identifierHeaderName, int responseTtlInMinutes, int requestTtlInMinutes)
    {
        if (_httpContextAccessor.HttpContext.Request.RouteValues.TryGetValue("client", out var clientName))
        {
            if (string.IsNullOrEmpty(identifierHeaderName))
            {
                _logger.LogError("{type}.Add for client: '{client}' without valid identifierHeader", GetType().Name, clientName);
                return default;
            }

            _logger.LogInformation("{type}.Add for client: '{client}' with identifierHeader: '{identifierHeader}'", GetType().Name, clientName, identifierHeaderName);

            var clientInfo = new ClientInfo(
                (string)clientName,
                identifierHeaderName,
                _responseModelContainerFactory(),
                TimeSpan.FromMinutes(responseTtlInMinutes),
                _requestModelContainerFactory(),
                TimeSpan.FromMinutes(requestTtlInMinutes));

            Add(clientInfo);

            return new ClientModel
            {
                Name = clientInfo.Name,
                IdentifierHeaderName = clientInfo.IdentifierHeaderName
            };
        }

        _logger.LogError("{type}.AddClient no client in RouteValues: {@routeValues}", GetType().Name, _httpContextAccessor.HttpContext.Request.RouteValues);
        return default;
    }

    public ClientModel GetClient()
    {
        if (_httpContextAccessor.HttpContext.Request.RouteValues.TryGetValue("client", out var clientName))
        {
            _logger.LogInformation("{type}.GetClient for client: '{client}'", GetType().Name, clientName);

            if (TryGet((string)clientName, out ClientInfo client))
            {
                return new ClientModel
                {
                    Name = client.Name,
                    IdentifierHeaderName = client.IdentifierHeaderName
                };
            }

            throw new ArgumentException("Unable to determine the ClientInfo");
        }

        _logger.LogError("{type}.AddClient no client in RouteValues: {@routeValues}", GetType().Name, _httpContextAccessor.HttpContext.Request.RouteValues);
        return default;
    }

    public IEnumerable<ClientModel> GetClients()
    {
        _logger.LogInformation("{type}.GetClients", GetType().Name);

        return InternalGetClients();
    }

    public void Remove()
    {
        if (!_httpContextAccessor.HttpContext.Request.RouteValues.TryGetValue("client", out var clientName))
        {
            throw new ArgumentException("Unable to determine the clientName");
        }

        _logger.LogDebug("{type}.Clear for client: '{clientName}'", GetType().Name, clientName);

        Remove((string)clientName);
    }

    public IEnumerable<ResponseModel> GetResponses()
    {
        return WithClient((client, identifierValue) =>
        {
            return client.ResponseContainer.Get(identifierValue);
        });
    }

    public void AddOrUpdateResponses(IEnumerable<ResponseModel> responses)
    {
        WithClient((client, identifierValue) =>
        {
            client.ResponseContainer.AddOrUpdate(identifierValue, responses);
        });
    }

    public void RemoveResponses()
    {
        WithClient((client, identifierValue) =>
        {
            client.ResponseContainer.Remove(identifierValue);
        });
    }

    public ResponseModel FindResult(string httpMethod, string path, IQueryCollection query)
    {
        return WithClient((client, identifierValue) =>
        {
            return client.ResponseContainer.FindResult(identifierValue, httpMethod, path, query);
        });
    }

    public IEnumerable<RequestModel> GetRequests()
    {
        return WithClient((client, identifierValue) =>
        {
            return client.RequestContainer.Get(identifierValue);
        });
    }

    public void RemoveRequests()
    {
        WithClient((client, identifierValue) =>
        {
            client.RequestContainer.Remove(identifierValue);
        });
    }

    public RequestModel AddRequest(string path, IDictionary<string, string> query, string httpMethod, IDictionary<string, IEnumerable<string>> headers, string body, bool responseFound)
    {
        return WithClient((client, identifierValue) =>
        {
            return client.RequestContainer.AddRequest(identifierValue, path, query, httpMethod, headers, body, responseFound);
        });
    }

    public IEnumerable<RequestModel> SearchRequests(RequestSearchModel searchModel)
    {
        return WithClient((client, identifierValue) =>
        {
            return client.RequestContainer.Find(identifierValue, searchModel.Path, searchModel.Query, searchModel.HttpMethods);
        });
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                foreach (var clientInfo in _internalContainer)
                {
                    clientInfo.Dispose();
                }
                _cacheLock.Dispose();
                _cacheLock = null;
            }

            disposedValue = true;
        }
    }

    private void WithClient(Action<ClientInfo, string> actionToExecute)
    {
        WithClient((client, identifier) =>
        {
            actionToExecute(client, identifier);
            return true;
        });
    }

    private T WithClient<T>(Func<ClientInfo, string, T> actionToExecute)
    {
        if (_httpContextAccessor.HttpContext.Request.RouteValues.TryGetValue("client", out var clientName) &&
            TryGet((string)clientName, out ClientInfo client))
        {
            return actionToExecute(client, GetIdentifierValue(client.IdentifierHeaderName));
        }

        throw new ArgumentException("Unable to determine the ClientInfo");
    }

    private bool TryGet(string clientName, out ClientInfo clientInfo)
    {
        _cacheLock.EnterReadLock();
        try
        {
            clientInfo = null;
            if (!string.IsNullOrEmpty(clientName))
            {
                clientInfo = _internalContainer.FirstOrDefault(client => client.Name.Equals(clientName, StringComparison.OrdinalIgnoreCase));
            }
            return clientInfo != null;
        }
        finally
        {
            _cacheLock.ExitReadLock();
        }
    }

    private string GetIdentifierValue(string identifierHeaderName)
    {
        if (_httpContextAccessor.HttpContext.Request.Headers.TryGetValue(identifierHeaderName, out var values))
        {
            if (values.Any(value => !string.IsNullOrEmpty(value)))
            {
                return values.FirstOrDefault(value => !string.IsNullOrEmpty(value));
            }
            throw new ArgumentException($"IdentifierValue for header: \"{identifierHeaderName}\" not found!");
        }

        return DefaultIdentifierValue;
    }

    private IEnumerable<ClientModel> InternalGetClients()
    {
        _cacheLock.EnterReadLock();
        try
        {
            var list = _internalContainer.Select(client => new ClientModel 
            { 
                Name = client.Name,  
                IdentifierHeaderName = client.IdentifierHeaderName
            });

            return list;
        }
        finally
        {
            _cacheLock.ExitReadLock();
        }
    }

    private void Add(ClientInfo newClient)
    {
        _cacheLock.EnterUpgradeableReadLock();
        try
        {
            var clientInfo = _internalContainer.FirstOrDefault(client => client.Name.Equals(newClient.Name, StringComparison.OrdinalIgnoreCase));
            if (clientInfo != null)
            {
                if (newClient.Equals(clientInfo))
                {
                    return;
                }
                throw new ArgumentOutOfRangeException(nameof(newClient), $"ClientName: {newClient.Name} already exists, but has differences");
            }

            _cacheLock.EnterWriteLock();
            try
            {
                _internalContainer.Add(newClient);
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }
        }
        finally
        {
            _cacheLock.ExitUpgradeableReadLock();
        }
    }

    private void Remove(string clientName)
    {
        _cacheLock.EnterUpgradeableReadLock();
        try
        {
            var clientInfo = _internalContainer.FirstOrDefault(client => client.Name.Equals(clientName, StringComparison.OrdinalIgnoreCase))
                ?? throw new ArgumentOutOfRangeException(nameof(clientName), $"ClientName: {clientName} is not a valid client name");

            _cacheLock.EnterWriteLock();
            try
            {
                _internalContainer.Remove(clientInfo);
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }
        }
        finally
        {
            _cacheLock.ExitUpgradeableReadLock();
        }
    }
}
