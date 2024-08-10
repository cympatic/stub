using Cympatic.Extensions.Stub.Internal.Collections;
using Cympatic.Extensions.Stub.Internal.Interfaces;
using FluentAssertions;

namespace Cympatic.Extensions.Stub.UnitTests.Internal.Collections;

public class AutomaticExpireCollectionTests : IDisposable
{
    private class FakeAutomaticExpireItem : IAutomaticExpireItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTimeOffset CreatedDateTime { get; set; } = DateTimeOffset.UtcNow;
    }

    private class FakeModelContainer : AutomaticExpireCollection<FakeAutomaticExpireItem>
    {
        public FakeModelContainer() : base(new TimeSpan(1))
        { }
    }

    private const int NumberOfItems = 10;

    private readonly FakeModelContainer _sut = new();

    public void Dispose()
    {
        _sut?.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void When_the_Ttl_expires_Then_all_items_are_removed_that_meet_the_expiration_criteria()
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
        Thread.Sleep(100);

        // Assert
        actual = _sut.All();
        actual.Should().HaveCount(0);
    }

    [Fact]
    public void When_All_is_called_Then_all_items_are_returned()
    {
        // Arrange
        for (int i = 0; i < NumberOfItems; i++)
        {
            _sut.Add(new FakeAutomaticExpireItem());
        }

        // Act
        var actual = _sut.All();

        // Assert
        actual.Should().HaveCount(NumberOfItems);
    }

    [Fact]
    public void When_Add_is_called_with_a_valid_Item_Then_the_Item_is_added_to_the_Collection()
    {
        // Arrange
        var expected = new FakeAutomaticExpireItem();

        // Act
        _sut.Add(expected);
        var actual = _sut.All().SingleOrDefault();

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void When_Add_is_called_with_a_null_Item_Then_an_ArgumentNullException_is_thrown()
    {
        // Arrange & Act
        var act = () => _sut.Add(default!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("item");
    }

    [Fact]
    public void When_Remove_is_called_with_a_valid_Item_Then_the_Item_is_removed_to_the_Collection()
    {
        // Arrange
        var item = new FakeAutomaticExpireItem();
        _sut.Add(item);

        // Act
        var actual = _sut.Remove(item);

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void When_Remove_is_called_with_a_null_Item_Then_an_ArgumentNullException_is_thrown()
    {
        // Arrange & Act
        var act = () => _sut.Remove(default!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("item");
    }

    [Fact]
    public void When_Clear_is_called_Then_all_items_are_removed_and_is_the_Collection_empty()
    {
        // Arrange
        for (int i = 0; i < NumberOfItems; i++)
        {
            _sut.Add(new FakeAutomaticExpireItem());
        }

        // Act
        _sut.Clear();

        // Assert
        _sut.Count.Should().Be(0);
    }

    [Fact]
    public void When_Count_is_called_Then_the_number_of_items_is_returned()
    {
        // Arrange
        const int expected = NumberOfItems;
        for (int i = 0; i < NumberOfItems; i++)
        {
            _sut.Add(new FakeAutomaticExpireItem());
        }

        // Act
        var actual = _sut.Count;

        // Assert
        actual.Should().Be(expected);
    }
}
