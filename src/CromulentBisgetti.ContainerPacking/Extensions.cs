﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace CromulentBisgetti.ContainerPacking;

public static class Extensions
{
    public static IEnumerable<decimal> DistinctWithTolerance(this IEnumerable<decimal> source, decimal tolerance)
    {
        var distinctItems = new List<decimal>();
        foreach (var item in source)
        {
            if (distinctItems.All(distinctItem => distinctItem.EqualsWithTolerance(item, tolerance)))
                distinctItems.Add(item);
        }

        return distinctItems;
    }

    public static bool EqualsWithTolerance(this decimal source, decimal other, decimal tolerance)
    {
        return Math.Abs(source - other) >= tolerance;
    }
}