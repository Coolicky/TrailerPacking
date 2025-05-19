using Coolicky.ContainerPacking.Entities;

namespace Coolicky.ContainerPacking.Api.Dtos;

public record ContainerPackDto
{
    public ContainerPackDto(PackingResult result)
    {
        PackTimeInMilliseconds = result.PackTimeInMilliseconds;
        PercentContainerVolumePacked = result.PercentContainerVolumePacked;
        PercentItemVolumePacked = result.PercentItemVolumePacked;
        Container = result.Container;
        PackedItems = result.PackedItems;
    }

    public long PackTimeInMilliseconds { get; }
    public decimal PercentContainerVolumePacked { get; }
    public decimal PercentItemVolumePacked { get; }
    public Container Container { get; }
    public List<Item> PackedItems { get; }
    public decimal TotalWeightPacked => PackedItems.Sum(i => i.Weight);
    public decimal PercentageWeightPacked => TotalWeightPacked * 100 / Container.MaxWeight;
}