using System.Collections.Generic;
using System.Linq;

namespace Coolicky.ContainerPacking.Entities;

/// <summary>
/// Represents the result of a packing operation, including details about packed and unpacked items,
/// efficiency metrics, and the associated container.
/// </summary>
public record PackingResult
{
	public bool IsCompletePack { get; set; }
	public long PackTimeInMilliseconds { get; set; }
	public decimal PercentContainerVolumePacked { get; set; }
	public decimal PercentItemVolumePacked { get; set; }
	public List<Item> PackedItems { get; set; } = [];
	public List<Item> UnpackedItems { get; set; } = [];
	public Container Container { get; set; }

	public decimal TotalWeightPacked => PackedItems.Sum(i => i.Weight);
	public decimal PercentageWeightPacked => TotalWeightPacked * 100 / Container.MaxWeight;
}