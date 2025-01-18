using System.Collections.Generic;
using Godot;

public partial class EntityRenderers : Node2D
{
    private static readonly List<EntityRenderer> StaticRenderers = new List<EntityRenderer>();
    private static readonly List<EntityRenderer> DynamicRenderers = new List<EntityRenderer>();
    
    private static readonly List<EntityRenderer> SortedRenderers = new List<EntityRenderer>();

    public void RegisterRenderer(EntityRenderer newRenderer)
    {
        if (newRenderer.Registered) 
            return;
        
        if (newRenderer.IsDynamic)
        {
            DynamicRenderers.Add(newRenderer);
        }
        else
        {
            StaticRenderers.Add(newRenderer);
            SetupStaticDependencies(newRenderer);
        }
        
        newRenderer.Registered = true;
    }

    private static void SetupStaticDependencies(EntityRenderer newRenderer)
    {
        foreach (var otherRenderer in StaticRenderers)
        {
            if (BoundsIntersect(newRenderer, otherRenderer) is false) 
                continue;
            
            var compareResult = newRenderer.CompareRenderers(otherRenderer);
            if (compareResult == -1)
            {
                otherRenderer.StaticDependencies.Add(newRenderer);
            }
            else if (compareResult == 1)
            {
                newRenderer.StaticDependencies.Add(otherRenderer);
            }
        }
    }

    // TODO: to be called before entity is removed / destroyed
    public void UnregisterRenderer(EntityRenderer rendererToRemove)
    {
        if (rendererToRemove.Registered is false) 
            return;
        
        if (rendererToRemove.IsDynamic)
        {
            DynamicRenderers.Remove(rendererToRemove);
        }
        else
        {
            StaticRenderers.Remove(rendererToRemove);
            RemoveStaticDependencies(rendererToRemove);
        }
        
        rendererToRemove.Registered = false;
    }

    private static void RemoveStaticDependencies(EntityRenderer rendererToRemove)
    {
        for (var i = 0; i < rendererToRemove.StaticDependencies.Count; i++)
        {
            var otherSprite = rendererToRemove.StaticDependencies[i];
            otherSprite.StaticDependencies.Remove(rendererToRemove);
        }
        rendererToRemove.StaticDependencies.Clear();
    }
    
    // Only needed to allow non-anonymous delegate unsubscription in Entities.cs:
    // https://stackoverflow.com/questions/8803064/event-unsubscription-via-anonymous-delegate
    public void UpdateSorting(EntityNode entity) => UpdateSorting();

    public void UpdateSorting()
    {
        ClearDynamicDependencies(StaticRenderers);
        ClearDynamicDependencies(DynamicRenderers);

        AddMovingDependencies(DynamicRenderers, StaticRenderers);

        SortedRenderers.Clear();
        TopologicalSort.Sort(StaticRenderers, DynamicRenderers, SortedRenderers);
        SetZIndexForRenderers(SortedRenderers);
    }

    private static void AddMovingDependencies(IReadOnlyList<EntityRenderer> dynamicRenderers, 
        IReadOnlyList<EntityRenderer> staticRenderers)
    {
        foreach (var dynamicRenderer in dynamicRenderers)
        {
            foreach (var staticRenderer in staticRenderers)
            {
                if (BoundsIntersect(dynamicRenderer, staticRenderer) is false) 
                    continue;
                
                var compareResult = dynamicRenderer.CompareRenderers(staticRenderer);
                if (compareResult == -1)
                {
                    staticRenderer.DynamicDependencies.Add(dynamicRenderer);
                }
                else if (compareResult == 1)
                {
                    dynamicRenderer.DynamicDependencies.Add(staticRenderer);
                }
            }

            foreach (var dynamicRendererInner in dynamicRenderers)
            {
                if (BoundsIntersect(dynamicRenderer, dynamicRendererInner) is false) 
                    continue;
                
                var compareResult = dynamicRenderer.CompareRenderers(dynamicRendererInner);
                if (compareResult == -1)
                {
                    dynamicRendererInner.DynamicDependencies.Add(dynamicRenderer);
                }
            }
        }
    }

    private static void ClearDynamicDependencies(List<EntityRenderer> renderers)
    {
        foreach (var renderer in renderers) renderer.DynamicDependencies.Clear();
    }

    private static bool BoundsIntersect(EntityRenderer renderer, EntityRenderer otherRenderer)
    {
        var result = renderer.SpriteBounds.Intersects(otherRenderer.SpriteBounds);
        return result;
    }

    private static void SetZIndexForRenderers(List<EntityRenderer> sortedRenderers)
    {
        var currentOrder = sortedRenderers.Count;
        foreach (var renderer in sortedRenderers)
        {
            renderer.ZIndex = currentOrder;
            renderer.UpdateZIndex(currentOrder);
            currentOrder -= 1;
        }
    }
}
