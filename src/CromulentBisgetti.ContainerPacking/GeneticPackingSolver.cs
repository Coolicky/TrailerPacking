using CromulentBisgetti.ContainerPacking.Algorithms;
using CromulentBisgetti.ContainerPacking.Entities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CromulentBisgetti.ContainerPacking;

public static class GeneticPackingSolver
{
    public static AlgorithmPackingResult Hello(Container container, List<Item> itemsToPack)
    {
        var totalItems = itemsToPack.Sum(i => i.Quantity);
        var dims = new List<decimal>();
        foreach (var item in itemsToPack)
        {
            dims.Add(item.Length);
            dims.Add(item.Depth);
        }
        dims = dims.Distinct().ToList();
        var secondaryDims = new List<decimal>();
        foreach (var dim in dims)
        {
            foreach (var secondDim in dims.Where(r => r != dim))
            {
                secondaryDims.Add(dim+ secondDim);
            }
        }
        dims = dims
            .Concat(secondaryDims)
            .DistinctWithTolerance(1M)
            .OrderByDescending(r => r)
            .ToList();

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var results = new List<AlgorithmPackingResult>();
        Parallel.ForEach(dims, dim =>
        {
            results.Add(PackForContainer(container, itemsToPack, dim, dims.ToArray()));
        });
        
        var bestResult = results
            .OrderByDescending(r => r.PercentContainerVolumePacked)
            .First();

        stopwatch.Stop();
        
        bestResult.PackTimeInMilliseconds = stopwatch.ElapsedMilliseconds;

        var containerVolume = container.Length * container.Width * container.Height;
        var itemVolumePacked = bestResult.PackedItems.Sum(i => i.Volume);
        var itemVolumeUnpacked = bestResult.UnpackedItems.Sum(i => i.Volume);

        bestResult.PercentContainerVolumePacked = Math.Round(itemVolumePacked / containerVolume * 100, 2);
        bestResult.PercentItemVolumePacked = Math.Round(itemVolumePacked / (itemVolumePacked + itemVolumeUnpacked) * 100, 2);

        var unpacked = new List<Item>();
        foreach (var unpackedItem in bestResult.UnpackedItems)
        {
            for (var i = 0; i < unpackedItem.Quantity; i++)
            {
                unpacked.Add(unpackedItem with
                {
                    Quantity = 1,
                });
            }
        }
        bestResult.UnpackedItems = unpacked;

        var total = bestResult.PackedItems.Count + bestResult.UnpackedItems.Count;
        
        return bestResult;
    }

    private static AlgorithmPackingResult PackForContainer(
        Container container,
        IReadOnlyCollection<Item> itemsToPack,
        decimal dim,
        decimal[] dims)
    {
        var items = itemsToPack.Select(r => r with {}).ToList();
        var currentX = 0M;
        var result = new AlgorithmPackingResult
        {
            PackedItems = [],
            UnpackedItems = new List<Item>(items),
        };

        if (dim > container.Length) return result;
        var newContainer = new Container(0, dim, container.Width, container.Height);
        var firstPack = new EB_AFIT().Run(newContainer, items);

        if (firstPack.PackedItems.Count == 0) return result;
        
        var maxX = firstPack.PackedItems.Max(r => r.CoordX + r.PackDimX);
        foreach (var packedItem in firstPack.PackedItems)
        {
            packedItem.CoordX += currentX;
            var item = result.UnpackedItems.FirstOrDefault(r => r.ID == packedItem.ID);
            if (item == null) continue;
            item.Quantity -= 1;
            if (item.Quantity <= 0)
                result.UnpackedItems.Remove(item);
        }
        result.PackedItems.AddRange(firstPack.PackedItems);
        
        currentX += maxX;

        RecursivelyPackItems(container, dims, currentX, result, firstPack);

        var itemVolumePacked = result.PackedItems.Sum(i => i.Volume);
        result.PercentContainerVolumePacked = Math.Round(itemVolumePacked / container.Volume * 100, 2);
        return result;
    }

    private static void RecursivelyPackItems(Container trailer,
        decimal[] dims,
        decimal currentX,
        AlgorithmPackingResult result,
        AlgorithmPackingResult previousPack)
    {
        if (result.UnpackedItems.Count == 0) return;

        var remainingLength = trailer.Length - currentX;
        if (remainingLength <= 0) return;

        var matchingNextPack = PreviousPackLeft(previousPack);
        AlgorithmPackingResult bestResult;
        
        if (matchingNextPack != null &&
            previousPack.PackedItems.Select(r => r.PackDimX).Max() <= remainingLength)
        {
            bestResult = matchingNextPack;
            Log.Logger.Warning("Picking best result!!!!");
        }
        else
        {
            Log.Logger.Warning("Calculating from remaining.....");
            bestResult = CalculateBestResult(dims, remainingLength, trailer, result.UnpackedItems);
        }
        if (bestResult is null) return;
        if (bestResult.PackedItems.Count == 0) return;

        var maxX = bestResult.PackedItems.Max(r => r.CoordX + r.PackDimX);
        foreach (var packedItem in bestResult.PackedItems)
        {
            var newItem = packedItem with {};
            newItem.CoordX += currentX;
            var item = result.UnpackedItems.FirstOrDefault(r => r.ID == newItem.ID);
            if (item == null) continue;
            item.Quantity -= 1;
            if (item.Quantity <= 0)
                result.UnpackedItems.Remove(item);
            result.PackedItems.Add(newItem);
        }

        currentX += maxX;
        RecursivelyPackItems(trailer, dims, currentX, result, bestResult);
    }

    private static AlgorithmPackingResult CalculateBestResult(
        decimal[] dims,
        decimal remainingLength,
        Container trailer,
        List<Item> itemsToPack)
    {
        var results = new List<AlgorithmPackingResult>();
        foreach (var proposedDim in dims)
        {
            if (proposedDim > remainingLength) continue;
            var container = new Container(0, proposedDim, trailer.Width, trailer.Height);
            if (remainingLength - proposedDim < proposedDim)
                container.Length = remainingLength;
            var packingResult = new EB_AFIT().Run(container, itemsToPack);

            results.Add(packingResult);
        }
        if (results.Count == 0) return null;
        return results
            .OrderByDescending(r => r.PercentContainerVolumePacked)
            .First();
    }
    
    private static AlgorithmPackingResult PreviousPackLeft(AlgorithmPackingResult previousPack)
    {
        var nextResult = previousPack with
        {
            PackedItems = [],
        };
        foreach (var packedItem in previousPack.PackedItems)
        {
            var item = previousPack.UnpackedItems
                .FirstOrDefault(r =>
                    Math.Abs(r.Height - packedItem.Height) < 0.01M &&
                    r.Depth == packedItem.Depth &&
                    r.Length == packedItem.Length);
            if (item == null) return null;

            var newItem = packedItem with
            {
                ID = item.ID,
            };
            nextResult.PackedItems.Add(newItem);
            nextResult.UnpackedItems.Remove(item);
        }
        return nextResult;
    }
}