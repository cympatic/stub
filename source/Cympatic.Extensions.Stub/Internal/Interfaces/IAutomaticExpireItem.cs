namespace Cympatic.Extensions.Stub.Internal.Interfaces;

internal interface IAutomaticExpireItem
{
    Guid Id { get; set; }

    DateTimeOffset CreatedDateTime { get; set;  }
}
