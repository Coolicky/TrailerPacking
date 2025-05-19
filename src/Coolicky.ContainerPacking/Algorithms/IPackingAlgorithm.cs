using Coolicky.ContainerPacking.Entities;
using System.Collections.Generic;

namespace Coolicky.ContainerPacking.Algorithms;

/// <summary>
/// Represents an interface for implementing packing algorithms that determine how to efficiently pack items into a container.
/// </summary>
public interface IPackingAlgorithm
{
	/// <summary>
	/// Executes the packing algorithm to efficiently pack items into the specified container.
	/// </summary>
	/// <param name="trailer">The container into which the items are to be packed. This includes dimensions and weight capacity.</param>
	/// <param name="items">The collection of items to be packed into the container.</param>
	/// <returns>A <see cref="PackingResult"/> object containing details such as packed items, unpacked items, packing efficiency, and metrics.</returns>
	PackingResult Run(Container trailer, List<Item> items);
}