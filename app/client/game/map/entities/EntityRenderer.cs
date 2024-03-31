using System;
using System.Collections.Generic;
using Godot;

public class EntityRenderer : Node2D
{
    public enum SortTypes
    {
        Point,
        Line
    }
    
    public Guid InstanceId { get; private set; } = Guid.NewGuid();
    public bool Registered { get; set; } = false;
    public bool IsDynamic { get; private set; } = false;
    public SpriteBounds SpriteBounds { get; private set; }
    public SortTypes SortType { get; private set; } = SortTypes.Point;

    public readonly List<EntityRenderer> StaticDependencies = new List<EntityRenderer>();
    public readonly List<EntityRenderer> DynamicDependencies = new List<EntityRenderer>();
    
    private Vector2 _topOrigin;
    private Vector2 _bottomOrigin;

    private Sprite _topOriginSprite;
    private Sprite _bottomOriginSprite;

    public Vector2 AsPoint => SortType is SortTypes.Point 
        ? _topOrigin 
        : (_topOrigin + _bottomOrigin) / 2;

    private float SortingLineCenterHeight => SortType is SortTypes.Point
        ? _topOrigin.y
        : (_topOrigin.y + _bottomOrigin.y) / 2;

    public override void _Ready()
    {
        _topOriginSprite = GetNode<Sprite>("OriginTop");
        _bottomOriginSprite = GetNode<Sprite>("OriginBottom");
    }
    
    public void Initialize(Guid instanceId, bool isDynamic, Rect2 entityRelativeSize, Sprite sprite) 
        // TODO move sprite inside of the renderer
        // TODO also call RegisterRenderer
    {
        InstanceId = instanceId;
        ZIndex = 0;
        IsDynamic = isDynamic;
        SortType = (int)entityRelativeSize.Size.x == (int)entityRelativeSize.Size.y ? SortTypes.Point : SortTypes.Line;
        
        SpriteBounds = new SpriteBounds(sprite.GetRect().Position + GlobalPosition, sprite.GetRect().Size + GlobalPosition); // TODO this doesn't work
        
        UpdateOrigins(entityRelativeSize);
    }

    public void UpdateOrigins(Rect2 entityRelativeSize)
    {
        var position = entityRelativeSize.Position;
        var entitySize = entityRelativeSize.Size;
        var xBiggerThanY = entitySize.x > entitySize.y;

        var px = position.x;
        var py = position.y;
        var sx = entitySize.x;
        var sy = entitySize.y;
        
        const int widthStep = Constants.TileWidth / 2;
        const int heightStep = Constants.TileHeight / 2;
        
        _topOrigin = GlobalPosition + (SortType is SortTypes.Point
            ? entitySize.x > 1 ? new Vector2(0, (entitySize.x - 1) * heightStep) : Vector2.Zero
            : new Vector2(
                (xBiggerThanY ? (sy - px) * -1 : sx - py) * widthStep, 
                ((xBiggerThanY ? sy + px : sx + py) - 1) * heightStep));
        _bottomOrigin = GlobalPosition + (SortType is SortTypes.Point 
            ? _topOrigin
            : new Vector2(
                (xBiggerThanY ? sx + px : (sy + py) * -1) * widthStep, 
                ((xBiggerThanY ? sx + px : sy + py) - 1) * heightStep));

        _topOriginSprite.GlobalPosition = _topOrigin;
        _bottomOriginSprite.GlobalPosition = _bottomOrigin;
    }

    // A result of -1 means renderer1 is above renderer2 in physical space
    public int CompareRenderers(EntityRenderer renderer2)
    {
        var renderer1 = this;
        switch (renderer1.SortType)
        {
            case SortTypes.Point when renderer2.SortType == SortTypes.Point:
                return renderer2._topOrigin.y.CompareTo(renderer1._topOrigin.y);
            case SortTypes.Line when renderer2.SortType == SortTypes.Line:
                return CompareLineAndLine(renderer1, renderer2);
            case SortTypes.Point when renderer2.SortType == SortTypes.Line:
                return ComparePointAndLine(renderer1._topOrigin, renderer2);
            case SortTypes.Line when renderer2.SortType == SortTypes.Point:
                return -ComparePointAndLine(renderer2._topOrigin, renderer1);
            default:
                return 0;
        }
    }

    private static int CompareLineAndLine(EntityRenderer line1, EntityRenderer line2)
    {
        var line1TopOrigin = line1._topOrigin;
        var line1BottomOrigin = line1._bottomOrigin;
        var line2TopOrigin = line2._topOrigin;
        var line2BottomOrigin = line2._bottomOrigin;

        var comp1 = ComparePointAndLine(line1TopOrigin, line2);
        var comp2 = ComparePointAndLine(line1BottomOrigin, line2);
        var oneVsTwo = int.MinValue;
        if (comp1 == comp2) // Both points in line 1 are above or below line2
        {
            oneVsTwo = comp1;
        }

        var comp3 = ComparePointAndLine(line2TopOrigin, line1);
        var comp4 = ComparePointAndLine(line2BottomOrigin, line1);
        var twoVsOne = int.MinValue;
        if (comp3 == comp4) // Both points in line 2 are above or below line1
        {
            twoVsOne = -comp3;
        }

        if (oneVsTwo != int.MinValue && twoVsOne != int.MinValue)
        {
            if (oneVsTwo == twoVsOne) // The two comparisons agree about the ordering
            {
                return oneVsTwo;
            }
            return CompareLineCenters(line1, line2);
        }

        if (oneVsTwo != int.MinValue)
        {
            return oneVsTwo;
        }
        
        if (twoVsOne != int.MinValue)
        {
            return twoVsOne;
        }
        
        return CompareLineCenters(line1, line2);
    }

    private static int CompareLineCenters(EntityRenderer line1, EntityRenderer line2)
    {
        return -line1.SortingLineCenterHeight.CompareTo(line2.SortingLineCenterHeight);
    }

    private static int ComparePointAndLine(Vector2 point, EntityRenderer line)
    {
        var pointY = point.y;
        if (pointY > line._topOrigin.y && pointY > line._bottomOrigin.y)
        {
            return -1;
        }

        if (pointY < line._topOrigin.y && pointY < line._bottomOrigin.y)
        {
            return 1;
        }

        var slope = (line._bottomOrigin.y - line._topOrigin.y) / (line._bottomOrigin.x - line._topOrigin.x);
        var intercept = line._topOrigin.y - (slope * line._topOrigin.x);
        var yOnLineForPoint = (slope * point.x) + intercept;
        return yOnLineForPoint > point.y ? 1 : -1;
    }
}
