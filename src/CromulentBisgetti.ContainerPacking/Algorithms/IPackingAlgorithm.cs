using CromulentBisgetti.ContainerPacking.Entities;
using System.Collections.Generic;

namespace CromulentBisgetti.ContainerPacking.Algorithms;

public interface IPackingAlgorithm
{
	AlgorithmPackingResult Run(Container trailer, List<Item> items);
}