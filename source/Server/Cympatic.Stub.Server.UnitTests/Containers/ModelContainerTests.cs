using Cympatic.Stub.Connectivity.Models;
using Cympatic.Stub.Server.Containers;
using Cympatic.Stub.Server.UnitTests.TestData;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace Cympatic.Stub.Server.UnitTests.Containers
{
    public class ModelContainerTests
    {
        private class FakeModelContainer : ModelContainer<RequestModel>
        {
            public FakeModelContainer() : base(new TimeSpan(1))
            {
            }

            public void AddModel(string identifierValue, RequestModel requestModel)
            {
                _internalContainer.AddOrUpdate(identifierValue,
                    new HashSet<RequestModel> { requestModel },
                    (key, oldValue) =>
                    {
                        oldValue.Add(requestModel);
                        return oldValue;
                    });
            }

            public virtual IEnumerable<RequestModel> Get(string identifierValue)
            {
                if (_internalContainer.TryGetValue(identifierValue, out var models))
                {
                    return models;
                }

                return new List<RequestModel>();
            }
        }

        private readonly FakeModelContainer _sut;
        private readonly string _identifierValue;

        public ModelContainerTests()
        {
            _sut = new FakeModelContainer();
            _identifierValue = Guid.NewGuid().ToString("N");
        }

        [Fact]
        public void CleanUp()
        {
            PrepareContainer(_identifierValue);
            var actual = _sut.Get(_identifierValue);
            actual.Should().HaveCount(2);

            _sut.SetTimeToLive(new TimeSpan(1));
            Thread.Sleep(50);

            actual = _sut.Get(_identifierValue);
            actual.Should().HaveCount(0);
        }

        private IEnumerable<RequestModel> PrepareContainer(string identifierValue)
        {
            var models = RequestModelTestData.GetRequestModels();
            foreach (var model in models)
            {
                _sut.AddModel(identifierValue, model);
            }

            return models;
        }
    }
}
