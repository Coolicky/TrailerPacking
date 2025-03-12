using Coolicky.TrailerPacking.Algorithms;
using Coolicky.TrailerPacking.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Coolicky.TrailerPacking;

public static class GeneticPackingSolver
{
    public static AlgorithmPackingResult Hello(Container container, List<Item> itemsToPack)
    {
        var originalItems = itemsToPack.Select(r => r with{}).ToList();
        itemsToPack = GroupByStacking(itemsToPack, container);

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
        
        results.ForEach(r => UngroupStacking(r, originalItems));
        
        var bestResult = results
            .OrderByDescending(r => r.PercentContainerVolumePacked)
            .ThenByDescending(r => r.PercentageWeightPacked)
            .ThenByDescending(r => r.PackedItems.Count)
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
        return bestResult;
    }

    private static void UngroupStacking(AlgorithmPackingResult result, List<Item> originalItems)
    {
        var packed = result.PackedItems;
        var unpacked = result.UnpackedItems;

        var packedGrouped = packed.Where(r => r.IsCombined == true).ToList();
        var unpackedGrouped = unpacked.Where(r => r.IsCombined == true).ToList();

        foreach (var item in packedGrouped)
        {
            var originalItem = originalItems
                .First(r => r.ID == item.CombinedItemId);

            var quantity = item.Height / originalItem.Height;
            originalItem.Quantity += (int)quantity;

            var index = packed.IndexOf(item);
            
            for (var i = 0; i < quantity; i++)
            {
                var newItem = originalItem with
                {
                    ID = originalItem.ID,
                    Quantity = 1,
                    Height = originalItem.Height,
                    CoordX = item.CoordX,
                    CoordZ = item.CoordZ,
                    CoordY = item.CoordY + (i * originalItem.Height),
                    PackDimX = item.PackDimX,
                    PackDimZ = item.PackDimZ,
                    PackDimY = originalItem.Height,
                    Weight = originalItem.Weight,
                    IsCombined = true,
                };
                packed.Insert(index + i, newItem);
            }
            packed.Remove(item);
        }

        foreach (var item in unpackedGrouped)
        {
            var originalItem = originalItems
                .First(r => r.ID == item.CombinedItemId);

            var quantity = item.Height / originalItem.Height;
            originalItem.Quantity += (int)quantity;

            var index = unpacked.IndexOf(item);
            
            for (var i = 0; i < quantity; i++)
            {
                var newItem = originalItem with
                {
                    ID = originalItem.ID,
                    Quantity = 1,
                    Height = originalItem.Height,
                    CoordX = item.CoordX,
                    CoordZ = item.CoordZ,
                    CoordY = item.CoordY + (i * originalItem.Height),
                    PackDimX = item.PackDimX,
                    PackDimZ = item.PackDimZ,
                    PackDimY = originalItem.Height,
                    IsCombined = true,
                };
                unpacked.Insert(index + i, newItem);
            }
            unpacked.Remove(item);
        }
    }

    private static List<Item> GroupByStacking(List<Item> itemsToPack, Container container)
    {
        var items = new List<Item>(itemsToPack);
        foreach (var item in itemsToPack)
        {
            if (item.MaxStack == 1)
                item.IsStackable = false;
            if (!item.IsStackable) continue;

            if (item.MaxStack == null) continue;
            if (item.MaxStack < 2) continue;
            if (item.Quantity < 2) continue;
            if (item.Height * 2 > container.Height) continue;

            var maxStack = item.MaxStack.Value;
            if (maxStack * item.Height > container.Height)
                maxStack = (int)(container.Height / item.Height);

            var newId = Guid.NewGuid();
            while (item.Quantity > 0)
            {
                if (item.Quantity == 1)
                {
                    break;
                }
                if (item.Quantity < maxStack)
                {
                    var smallId = Guid.NewGuid();
                    items.Add(item with
                    {
                        ID = smallId,
                        IsCombined = true,
                        Height = item.Height * item.Quantity,
                        Weight = item.Weight * item.Quantity,
                        CombinedItemId = item.ID,
                        IsStackable = false,
                        Quantity = 1,
                    });
                    break;
                }
                else
                {
                    items.Add(item with
                    {
                        ID = newId,
                        IsCombined = true,
                        Height = item.Height * maxStack,
                        Weight = item.Weight * maxStack,
                        CombinedItemId = item.ID,
                        IsStackable = false,
                        Quantity = 1,
                    });
                    item.Quantity -= maxStack;
                }
            }
        }
        return items
            .GroupBy(r => r.ID)
            .Select(r => r.First() with
            {
                Quantity = r.Sum(x => x.Quantity)
            })
            .Where(r => r.Quantity > 0).ToList();
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
            Container = container,
            PackedItems = [],
            UnpackedItems = new List<Item>(items),
        };

        if (dim > container.Length) return result;
        var newContainer = new Container(0, dim, container.Width, container.Height, container.MaxWeight);
        var firstPack = new EB_AFIT().Run(newContainer, items);
        OptimizeResult(firstPack);

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

    private static void RecursivelyPackItems(
        Container trailer,
        decimal[] dims,
        decimal currentX,
        AlgorithmPackingResult result,
        AlgorithmPackingResult previousPack)
    {
        if (result.TotalWeightPacked >= trailer.MaxWeight)
        {
            while (result.TotalWeightPacked > trailer.MaxWeight)
            {
                var last = result.PackedItems.Last();
                result.PackedItems.Remove(last);
                result.UnpackedItems.Add(last);
            }
            return;
        }

        if (result.UnpackedItems.Count == 0) return;

        var remainingLength = trailer.Length - currentX;
        if (remainingLength <= 0) return;

        var matchingNextPack = PreviousPackLeft(previousPack);
        AlgorithmPackingResult bestResult;
        
        if (matchingNextPack != null &&
            previousPack.PackedItems.Select(r => r.PackDimX).Max() <= remainingLength)
        {
            bestResult = matchingNextPack;
        }
        else
        {
            bestResult = CalculateBestResult(dims, remainingLength, trailer, result.UnpackedItems);
        }
        if (bestResult is null) return;
        if (bestResult.PackedItems.Count == 0) return;

        OptimizeResult(bestResult);

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

    private static void OptimizeResult(AlgorithmPackingResult result)
    {
        RearrangeLayers(result);
        DropFloatingItems(result);
    }

    private static void RearrangeLayers(AlgorithmPackingResult result)
    {
        if (result.PackedItems.Select(r => r.Height).DistinctWithTolerance(0.1M).Count() == 1) return;
        var layers = result.PackedItems
            .GroupBy(r => r.CoordY)
            .OrderBy(r => r.Select(i => i.PackDimY).DistinctWithTolerance(0.1M).Count())
            .ThenByDescending(r => r.Select(i => i.Volume).Sum())
            .ToList();

        var items = new List<Item>();
        var height = 0M;
        foreach (var layer in layers)
        {
            var layerHeight = layer.Select(r => r.PackDimY).Max();
            foreach (var item in layer)
            {
                items.Add(item with
                {
                    CoordY = height,
                });
            }
            height += layerHeight;
        }
        result.PackedItems = items;
    }

    private static void DropFloatingItems(AlgorithmPackingResult result)
    {
        foreach (var item in result.PackedItems)
        {
            if (item.CoordY == 0) continue;
            var coords = new BoxCoords(item);
            var itemsBelow = result
                .PackedItems
                .Where(r => r.CoordY < item.CoordY)
                .Where(r => coords.OverlapsXZ(r))
                .ToList();
            if (!itemsBelow.Any())
            {
                item.CoordY = 0;
                return;
            }
            var maxBelow = itemsBelow.Select(r => r.CoordY + r.PackDimY)
                .Max();
            if (maxBelow.EqualsWithTolerance(item.CoordY, 0.1M))
                item.CoordY = maxBelow;
        }
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
            var container = new Container(0, proposedDim, trailer.Width, trailer.Height, trailer.MaxWeight);
            if (remainingLength - proposedDim < proposedDim)
                container.Length = remainingLength;
            var packingResult = new EB_AFIT().Run(container, itemsToPack);

            results.Add(packingResult);
        }
        if (results.Count == 0) return null;
        return results
            .OrderByDescending(r => r.PercentContainerVolumePacked)
            .ThenByDescending(r => r.PercentageWeightPacked)
            .ThenByDescending(r => r.PackedItems.Count)
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
            var item = previousPack.UnpackedItems.FirstOrDefault(r => r.ID == packedItem.ID);
            if (item == null)
            {
                item = previousPack.UnpackedItems
                    .FirstOrDefault(r =>
                        Math.Abs(r.Height - packedItem.Height) < 0.01M &&
                        r.Depth == packedItem.Depth &&
                        r.Length == packedItem.Length);
            }
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