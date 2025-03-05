using System.Collections.Generic;

namespace CromulentBisgetti.ContainerPacking.Entities;

public record AlgorithmPackingResult
{
	public int AlgorithmID { get; set; }
	public string AlgorithmName { get; set; }
	public bool IsCompletePack { get; set; }
	public long PackTimeInMilliseconds { get; set; }
	public decimal PercentContainerVolumePacked { get; set; }
	public decimal PercentItemVolumePacked { get; set; }
	public List<Item> PackedItems { get; set; } = [];
	public List<Item> UnpackedItems { get; set; } = [];
	public Container Container { get; set; }
}