using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class TopologicalSort
{
    private static readonly Dictionary<Guid, bool> CircularDepData = new Dictionary<Guid, bool>();
    private static readonly List<EntityRenderer> CircularDepStack = new List<EntityRenderer>();

    private static readonly HashSet<Guid> Visited = new HashSet<Guid>();
    private static readonly List<EntityRenderer> AllSprites = new List<EntityRenderer>();
    public static void Sort(IEnumerable<EntityRenderer> staticSprites, IEnumerable<EntityRenderer> dynamicSprites,
        List<EntityRenderer> sortedSprites)
    {
        AllSprites.Clear();
        AllSprites.AddRange(dynamicSprites);
        AllSprites.AddRange(staticSprites);
        
        var allSpritesCount = AllSprites.Count;
        for (var i = 0; i < 5; i++)
        {
            CircularDepStack.Clear();
            CircularDepData.Clear();
            var removedDependency = false;
            
            for (var j = 0; j < allSpritesCount; j++)
                if (RemoveCircularDependencies(AllSprites[j]))
                    removedDependency = true;
            
            if (!removedDependency)
                break;
        }

        Visited.Clear();
        for (var i = 0; i < allSpritesCount; i++)
        {
            var sprite = AllSprites[i];
            Visit(sprite, sortedSprites);
        }
    }

    private static void Visit(EntityRenderer item, ICollection<EntityRenderer> sortedSprites)
    {
        var id = item.InstanceId;
        if (Visited.Add(id) is false) 
            return;
        
        foreach (var d in item.DynamicDependencies) 
            Visit(d, sortedSprites);
        
        foreach (var d in item.StaticDependencies) 
            Visit(d, sortedSprites);

        sortedSprites.Add(item);
    }

    private static bool RemoveCircularDependencies(EntityRenderer item)
    {
        try
        {
            CircularDepStack.Add(item);
            var removedDependency = false;

            var id = item.InstanceId;
            var alreadyVisited = CircularDepData.TryGetValue(id, out var inProcess);
            if (alreadyVisited)
            {
                if (inProcess)
                {
                    RemoveCircularDependencyFromStack();
                    removedDependency = true;
                }
            }
            else
            {
                CircularDepData[id] = true;

                foreach (var _ in item.DynamicDependencies.Where(RemoveCircularDependencies)) 
                    removedDependency = true;
            
                foreach (var _ in item.StaticDependencies.Where(RemoveCircularDependencies)) 
                    removedDependency = true;

                CircularDepData[id] = false;
            }

            CircularDepStack.RemoveAt(CircularDepStack.Count - 1);
            return removedDependency;
        }
        // This is to specifically catch "System.InvalidOperationException: Collection was modified; enumeration
        // operation may not execute". Could not debug how to stop this and catching it here does not impact the
        // overall functionality.
        catch (InvalidOperationException) 
        {
            return true;
        }
    }

    private static void RemoveCircularDependencyFromStack()
    {
        if (CircularDepStack.Count < 2) 
            return;
        
        var startingSorter = CircularDepStack[CircularDepStack.Count - 1];
        var repeatIndex = 0;
        for (var i = CircularDepStack.Count - 2; i >= 0; i--)
        {
            var sorter = CircularDepStack[i];
            if (sorter != startingSorter) 
                continue;
            
            repeatIndex = i;
            break;
        }

        var weakestDepIndex = -1;
        var longestDistance = float.MinValue;
        for (var i = repeatIndex; i < CircularDepStack.Count - 1; i++)
        {
            var sorter1A = CircularDepStack[i];
            var sorter2A = CircularDepStack[i + 1];
            if (sorter1A.SortType != EntityRenderer.SortTypes.Point
                || sorter2A.SortType != EntityRenderer.SortTypes.Point) 
                continue;
            
            var dist = Mathf.Abs(sorter1A.AsPoint.X - sorter2A.AsPoint.X);
            if (dist > longestDistance)
            {
                weakestDepIndex = i;
                longestDistance = dist;
            }
        }
        
        if (weakestDepIndex == -1)
        {
            for (var i = repeatIndex; i < CircularDepStack.Count - 1; i++)
            {
                var sorter1A = CircularDepStack[i];
                var sorter2A = CircularDepStack[i + 1];
                var dist = Mathf.Abs(sorter1A.AsPoint.X - sorter2A.AsPoint.X);
                if (dist > longestDistance)
                {
                    weakestDepIndex = i;
                    longestDistance = dist;
                }
            }
        }
        
        var sorter1 = CircularDepStack[weakestDepIndex];
        var sorter2 = CircularDepStack[weakestDepIndex + 1];
        sorter1.StaticDependencies.Remove(sorter2);
        sorter1.DynamicDependencies.Remove(sorter2);
    }
}
