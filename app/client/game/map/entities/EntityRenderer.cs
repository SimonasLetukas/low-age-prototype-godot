using System;
using System.Collections.Generic;
using Godot;

public partial class EntityRenderer : Node2D
{
    [Export]
    public bool DebugEnabled { get; set; } = false;
    
    public enum SortTypes
    {
        Point,
        Line
    }
    
    public string EntityName { get; set; }
    public Guid InstanceId { get; set; } = Guid.NewGuid();
    public bool Registered { get; set; } = false;
    public bool IsDynamic { get; private set; } = false;
    public Rect2 SpriteBounds { get; private set; }
    public SortTypes SortType { get; private set; } = SortTypes.Point;
    public Vector2 SpriteSize => _sprite.Texture2D.GetSize();
    public Rect2 EntityRelativeSize { get; private set; }

    public readonly List<EntityRenderer> StaticDependencies = new List<EntityRenderer>();
    public readonly List<EntityRenderer> DynamicDependencies = new List<EntityRenderer>();
    
    private Vector2 _topOrigin;
    private Vector2 _bottomOrigin;

    private Node2D _debugVisuals;
    private Sprite2D _topOriginSprite;
    private Sprite2D _bottomOriginSprite;
    private RichTextLabel _zIndexText;

    private Node2D _spriteContainer;
    private Sprite2D _sprite;

    public Vector2 AsPoint => SortType is SortTypes.Point 
        ? _topOrigin 
        : (_topOrigin + _bottomOrigin) / 2;

    private float SortingLineCenterHeight => SortType is SortTypes.Point
        ? _topOrigin.y
        : (_topOrigin.y + _bottomOrigin.y) / 2;

    public override void _Ready()
    {
        _debugVisuals = GetNode<Node2D>($"Debug");
        _topOriginSprite = GetNode<Sprite2D>("Debug/OriginTop");
        _bottomOriginSprite = GetNode<Sprite2D>("Debug/OriginBottom");
        _zIndexText = GetNode<RichTextLabel>($"Debug/{nameof(RichTextLabel)}");
        _spriteContainer = GetNode<Node2D>($"SpriteContainer");
        _sprite = GetNode<Sprite2D>($"SpriteContainer/{nameof(Sprite2D)}");

        _debugVisuals.Visible = DebugEnabled;
    }
    
    public void Initialize(Guid instanceId, string name, bool isDynamic, Rect2 entityRelativeSize) 
    {
        InstanceId = instanceId;
        EntityName = name;
        ZIndex = 0;
        IsDynamic = isDynamic;
        SortType = (int)entityRelativeSize.Size.x == (int)entityRelativeSize.Size.y ? SortTypes.Point : SortTypes.Line;
        
        AdjustToRelativeSize(entityRelativeSize);
    }

    public void MakeDynamic() => IsDynamic = true;
    public void MakeStatic() => IsDynamic = false;

    public void SetOutline(bool to)
    {
        if (_sprite.Material is ShaderMaterial shaderMaterial) 
            shaderMaterial.SetShaderParameter("draw_outline", to);
    }

    public void SetTintColor(Color color)
    {
        if (_sprite.Material is ShaderMaterial shaderMaterial)
            shaderMaterial.SetShaderParameter("tint_color", color);
    }

    public void SetTintAmount(float amount)
    {
        if (_sprite.Material is ShaderMaterial shaderMaterial)
            shaderMaterial.SetShaderParameter("tint_effect_factor", amount);
    }

    public void SetAlphaAmount(float amount) => _sprite.Modulate = new Color(Colors.White, amount);

    public void SetSpriteTexture(string location)
    {
        _sprite.Texture2D = GD.Load<Texture2D>(location);
        
        UpdateSpriteBounds();
    }

    public void UpdateSpriteOffset(Vector2 entitySize, Vector2 centerOffset)
    {
        var offsetFromX = (int)(entitySize.x - 1) * 
                        new Vector2((int)(Constants.TileWidth / 4), (int)(Constants.TileHeight / 4));
        var offsetFromY = (int)(entitySize.y - 1) *
                          new Vector2((int)(Constants.TileWidth / 4) * -1, (int)(Constants.TileHeight / 4));
        _sprite.Offset = (centerOffset * -1) + offsetFromX + offsetFromY;
        
        UpdateSpriteBounds();
    }

    public void FlipSprite(bool to)
    {
        _sprite.FlipH = to;
        
        UpdateSpriteBounds();
    }

    public void AimSprite(Vector2 target)
    {
        _spriteContainer.Scale = GlobalPosition.x > target.x
            ? new Vector2(-1, _spriteContainer.Scale.y)
            : new Vector2(1, _spriteContainer.Scale.y);
        
        UpdateSpriteBounds();
    }

    public void AdjustToRelativeSize(Rect2 entityRelativeSize)
    {
        EntityRelativeSize = entityRelativeSize;
        UpdateOrigins();
    }

    public void UpdateOrigins()
    {
        var position = EntityRelativeSize.Position;
        var entitySize = EntityRelativeSize.Size;
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

    public void UpdateSpriteBounds()
    {
        var top = _spriteContainer.Scale.x > 0 
            ? _sprite.GlobalPosition + _sprite.Offset 
            : _sprite.GlobalPosition + new Vector2(-_sprite.Offset.x - SpriteSize.x, _sprite.Offset.y);
        var bottom = top + SpriteSize;
        SpriteBounds = new Rect2(top, SpriteSize);
        
        if (DebugEnabled)
            GD.Print($"New {EntityName} sprite bounds: {SpriteBounds}");
        
        //_topOriginSprite.GlobalPosition = SpriteBounds.Position;
        //_bottomOriginSprite.GlobalPosition = SpriteBounds.End;
    }

    public void UpdateZIndex(int to)
    {
        ZIndex = to;
        _zIndexText.Text = to.ToString();
    }

    // A result of -1 means renderer1 is above renderer2 in physical space
    public int CompareRenderers(EntityRenderer renderer2)
    {
        var renderer1 = this;
        int result;
        switch (renderer1.SortType)
        {
            case SortTypes.Point when renderer2.SortType == SortTypes.Point:
                if (DebugEnabled)
                    GD.Print($"'{renderer1.EntityName}' topOrigin: '{renderer1._topOrigin}', " +
                             $"'{renderer2.EntityName}' topOrigin: '{renderer2._topOrigin}'.");
                result = renderer2._topOrigin.y.CompareTo(renderer1._topOrigin.y);
                break;
            case SortTypes.Line when renderer2.SortType == SortTypes.Line:
                result = CompareLineAndLine(renderer1, renderer2);
                break;
            case SortTypes.Point when renderer2.SortType == SortTypes.Line:
                result = ComparePointAndLine(renderer1._topOrigin, renderer2);
                break;
            case SortTypes.Line when renderer2.SortType == SortTypes.Point:
                result = -ComparePointAndLine(renderer2._topOrigin, renderer1);
                break;
            default:
                result = 0;
                break;
        }
        
        if (DebugEnabled)
        {
            var resultText = result == 0 ? "Both the same." : result > 0 
                ? $"'{renderer2.EntityName}' is on top." : $"'{renderer1.EntityName}' is on top.";
            GD.Print($"Renderer '{renderer1.EntityName}' of type '{renderer1.SortType}' compared to " +
                  $"'{renderer2.EntityName}' of type {renderer2.SortType} with the result of {result}. " + resultText);
        }

        if (result == 0 && ((renderer1.IsDynamic && renderer2.IsDynamic is false)
                            || (renderer2.IsDynamic && renderer1.IsDynamic is false)))
        {
            result = renderer1.IsDynamic ? -1 : 1;
            if (DebugEnabled)
                GD.Print(result > 0 ? $"'{renderer2.EntityName}' is on top because it's dynamic." 
                    : $"'{renderer1.EntityName}' is on top because it's dynamic.");
        }
        
        return result;
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
