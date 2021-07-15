using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Xunit;

namespace Cympatic.Extensions.Http.UnitTests
{
    public class HeaderDictionaryExtensionsTests
    {
        private readonly HeaderDictionary _sut;

        public HeaderDictionaryExtensionsTests()
        {
            _sut = new HeaderDictionary
            {
                { "test1", "value1_1" },
                { "test2", "value2_1" }
            };
        }

        [Fact]
        public void Given_a_HeaderDictionary_When_AddRange_is_called_with_Dictionary_and_additional_values_Then_the_HeaderDictionary_should_be_combined_into_the_existing_HeaderDictionary()
        {
            // Arrange & Act
            _sut.AddRange(new Dictionary<string, IEnumerable<string>>
            {
                { "test3", new [] { "value3_1" } },
                { "test4", new [] { "value4_1" } }
            });

            // Assert
            _sut.Should()
                .BeEquivalentTo(new Dictionary<string, IEnumerable<string>>
                {
                    { "test1", new [] { "value1_1" } },
                    { "test2", new [] { "value2_1" } },
                    { "test3", new [] { "value3_1" } },
                    { "test4", new [] { "value4_1" } }
                });
        }

        [Fact]
        public void Given_a_HeaderDictionary_When_AddRange_is_called_with_an_empty_HeaderDictionary_Then_the_HeaderDictionary_should_be_as_is()
        {
            // Arrange & Act
            _sut.AddRange(new Dictionary<string, IEnumerable<string>>());

            // Assert
            _sut.Should()
                .BeEquivalentTo(new Dictionary<string, IEnumerable<string>>
                {
                    { "test1", new [] { "value1_1" } },
                    { "test2", new [] { "value2_1" } }
                });
        }

        [Fact]
        public void Given_a_HeaderDictionary_When_AddRange_is_called_with_null_value_Then_the_HeaderDictionary_should_be_as_is()
        {
            // Arrange & Act
            _sut.AddRange(null);

            // Assert
            _sut.Should()
                .BeEquivalentTo(new Dictionary<string, IEnumerable<string>>
                {
                    { "test1", new [] { "value1_1" } },
                    { "test2", new [] { "value2_1" } }
                });
        }

        [Fact]
        public void Given_a_HeaderDictionary_When_AddRange_is_called_with_similar_keys_and_different_values_Then_the_similar_keys_in_the_HeaderDictionary_should_be_combined_in_the_existing_HeaderDictionary()
        {
            // Arrange & Act
            _sut.AddRange(new Dictionary<string, IEnumerable<string>>
            {
                { "test2", new [] { "value2_2" } },
                { "test3", new [] { "value3_1" } }
            });

            // Assert
            _sut.Should()
                .BeEquivalentTo(new Dictionary<string, IEnumerable<string>>
                {
                    { "test1", new [] { "value1_1" } },
                    { "test2", new [] { "value2_1", "value2_2" } },
                    { "test3", new [] { "value3_1" } }
                });
        }

        [Fact]
        public void Given_a_HeaderDictionary_When_AddRange_is_called_with_similar_keys_and_similar_values_Then_the_headers_should_be_combined_in_the_existing_HeaderDictionary()
        {
            // Arrange & Act
            _sut.AddRange(new Dictionary<string, IEnumerable<string>>
            {
                { "test2", new [] { "value2_1" } },
                { "test3", new [] { "value3_1" } }
            });

            // Assert
            _sut.Should()
                .BeEquivalentTo(new Dictionary<string, IEnumerable<string>>
                {
                    { "test1", new [] { "value1_1" } },
                    { "test2", new [] { "value2_1" } },
                    { "test3", new [] { "value3_1" } }
                });
        }

        [Fact]
        public void Given_a_HeaderDictionary_When_ToDictionary_is_called_Then_the_returned_dictionary_should_be_equal_to_the_existing_HeaderDictionary()
        {
            // Arrange & Act
            _sut.AddRange(new Dictionary<string, IEnumerable<string>>
            {
                { "test2", new [] { "value2_2" } },
                { "test3", new [] { "value3_1" } }
            });

            var actual = _sut.ToDictionary();

            // Assert
            actual.Should()
                .BeEquivalentTo(new Dictionary<string, IEnumerable<string>>
                {
                    { "test1", new [] { "value1_1" } },
                    { "test2", new [] { "value2_1", "value2_2" } },
                    { "test3", new [] { "value3_1" } }
                });
        }
    }
}