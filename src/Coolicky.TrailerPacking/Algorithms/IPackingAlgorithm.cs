using Coolicky.TrailerPacking.Entities;
using System.Collections.Generic;

namespace Coolicky.TrailerPacking.Algorithms;

public interface IPackingAlgorithm
{
	AlgorithmPackingResult Run(Container trailer, List<Item> items);
}