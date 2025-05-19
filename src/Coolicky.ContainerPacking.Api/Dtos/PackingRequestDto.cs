using Coolicky.ContainerPacking.Entities;

namespace Coolicky.ContainerPacking.Api.Dtos;

public record PackingRequestDto
{
    public required Container Container { get; init; }
    public required List<Item> Items { get; init; }
}