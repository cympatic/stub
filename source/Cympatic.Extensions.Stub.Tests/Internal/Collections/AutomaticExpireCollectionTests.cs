using Cympatic.Extensions.Stub.Internal.Collections;
using Cympatic.Extensions.Stub.Internal.Interfaces;
using FluentAssertions;

namespace Cympatic.Extensions.Stub.Tests.Internal.Collections;

public class AutomaticExpireCollectionTests
{
    internal class FakeAutomaticExpireItem : IAutomaticExpireItem
    {
        private readonly DateTimeOffset _createdDateTime = DateTimeOffset.UtcNow;
        
        public DateTimeOffset CreatedDateTime => _createdDateTime;
    }

    internal class FakeModelContainer : AutomaticExpireCollection<FakeAutomaticExpireItem>
    {
        public FakeModelContainer() : base(new TimeSpan(1))
        { }
    }

    private const int NumberOfItems = 10;
    private readonly FakeModelContainer _sut = new();

    [Fact]
    public void When_the_Ttl_expires_Then_the_items_are_removed()
    {
        // Arrange
        for (int i = 0; i < NumberOfItems; i++)
        {
            _sut.Add(new FakeAutomaticExpireItem());
        }

        var actual = _sut.All();
        actual.Should().HaveCount(NumberOfItems);

        // Act
        _sut.SetTimeToLive(new TimeSpan(1));
        Thread.Sleep(50);

        // Assert
        actual = _sut.All();
        actual.Should().BeEmpty();
    }
}
