using Cympatic.Extensions.Http;
using Cympatic.Stub.Connectivity.Models;
using Cympatic.Stub.Server.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Cympatic.Stub.Server.Containers
{
    public class ResponseModelContainer : ModelContainer<ResponseModel>
    {
        private readonly ILogger _logger;

        public ResponseModelContainer(ILogger<ResponseModelContainer> logger) : base()
        {
            _logger = logger;
        }

        public virtual IEnumerable<ResponseModel> Get(string identifierValue)
        {
            _logger.LogDebug("{type}.Get with identifier: '{identifier}'", GetType().Name, identifierValue);

            if (!_internalContainer.TryGetValue(identifierValue, out var items))
            {
                items = new HashSet<ResponseModel>();
            }

            return items;
        }

        public virtual void AddOrUpdate(string identifierValue, IEnumerable<ResponseModel> responseModels)
        {
            _logger.LogDebug("{type}.AddOrUpdate for identifier: '{identifier}' with ResponseModels:\r\n{@responseModels}", GetType().Name, identifierValue, responseModels);

            _internalContainer.AddOrUpdate(identifierValue,
                new HashSet<ResponseModel>(responseModels),
                (key, oldValue) =>
                {
                    responseModels
                        .ToList()
                        .ForEach(model => AddOrUpdateModel(oldValue, model));

                    return oldValue;
                });
        }

        public virtual ResponseModel FindResult(string identifierValue, string httpMethod, string path, IQueryCollection query)
        {
            _logger.LogDebug("{type}.FindResult for identifier: '{identifier}', httpMethod: '{httpMethod}', path: '{path}', and query:\r\n{@query}",
                GetType().Name, identifierValue, httpMethod, path, query);

            if (_internalContainer.TryGetValue(identifierValue, out var items))
            {
                var foundItems = items.Where(model => model.IsMatching(httpMethod, path, query.ToDictionary()));
                if (foundItems.Count() == 1)
                {
                    return foundItems.SingleOrDefault();
                }

                return foundItems.FirstOrDefault();
            }

            return null;
        }

        public virtual void Remove(string identifierValue)
        {
            _logger.LogDebug("{type}.Clear for identifier: '{identifier}'", GetType().Name, identifierValue);

            _internalContainer.TryRemove(identifierValue, out var _);
        }

        private static void AddOrUpdateModel(HashSet<ResponseModel> items, ResponseModel responseModel)
        {
            var foundItems = items.Where(model => model.AreRequestParamsEqual(responseModel.HttpMethods, responseModel.Path, responseModel.Query));
            foundItems.ToList().ForEach(item => items.Remove(item));

            items.Add(responseModel);
        }
    }
}
