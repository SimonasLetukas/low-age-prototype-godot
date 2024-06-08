﻿using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class EntityRenderer : Node2D
{
    [Export]
    public bool DebugEnabled { get; set; } = false;
    
    public enum SortTypes
    {
        Point,
        Line
    }
    
    public Guid InstanceId { get; set; } = Guid.NewGuid();
    public bool Registered { get; set; } = false;
    public bool IsDynamic { get; private set; } = false;
    public Rect2 SpriteBounds { get; private set; }
    public SortTypes SortType { get; private set; } = SortTypes.Point;
    public Vector2 SpriteSize => _sprite.Texture.GetSize();
    public int YHighGroundOffset { get; private set; } = 0;

    public readonly List<EntityRenderer> StaticDependencies = new List<EntityRenderer>();
    public readonly List<EntityRenderer> DynamicDependencies = new List<EntityRenderer>();
    
    private string _entityName;
    private Rect2 _entityRelativeSize;
    
    private Vector2 _topOrigin;
    private Vector2 _bottomOrigin;

    private Node2D _debugVisuals;
    private Sprite _topOriginSprite;
    private Sprite _bottomOriginSprite;
    private RichTextLabel _zIndexText;

    private Node2D _spriteContainer;
    private Sprite _sprite;
    private TextureRect _icon;
    private readonly Vector2 _iconOffset = new Vector2(-4, -5);

    private Area2D _area;
    private Rect2 _previousSpriteBounds = new Rect2();

    private EntityNode _entityBelow = null;
    private bool _isOnHighGround = false;

    public Vector2 AsPoint => SortType is SortTypes.Point 
        ? _topOrigin 
        : (_topOrigin + _bottomOrigin) / 2;

    private float SortingLineCenterHeight => SortType is SortTypes.Point
        ? _topOrigin.y
        : (_topOrigin.y + _bottomOrigin.y) / 2;

    public override void _Ready()
    {
        _debugVisuals = GetNode<Node2D>($"Debug");
        _topOriginSprite = GetNode<Sprite>("Debug/OriginTop");
        _bottomOriginSprite = GetNode<Sprite>("Debug/OriginBottom");
        _zIndexText = GetNode<RichTextLabel>($"Debug/{nameof(RichTextLabel)}");
        _spriteContainer = GetNode<Node2D>($"SpriteContainer");
        _sprite = GetNode<Sprite>($"SpriteContainer/{nameof(Sprite)}");
        _icon = GetNode<TextureRect>($"Icon");
        _area = GetNode<Area2D>(nameof(Area2D));

        _icon.Texture = null;
        _debugVisuals.Visible = DebugEnabled;
    }
    
    public void Initialize(Guid instanceId, string name, bool isDynamic, Rect2 entityRelativeSize) 
    {
        InstanceId = instanceId;
        _entityName = name;
        ZIndex = 0;
        IsDynamic = isDynamic;
        SortType = (int)entityRelativeSize.Size.x == (int)entityRelativeSize.Size.y ? SortTypes.Point : SortTypes.Line;
        
        AdjustToRelativeSize(entityRelativeSize);
        SetSpriteVisibility(true);
    }

    public void SetSpriteVisibility(bool to) => _spriteContainer.Visible = to;

    public void SetIconVisibility(bool to) => _icon.Visible = to;

    public void MakeDynamic() => IsDynamic = true;
    public void MakeStatic() => IsDynamic = false;

    public void SetOutline(bool to)
    {
        if (_sprite.Material is ShaderMaterial spriteShaderMaterial) 
            spriteShaderMaterial.SetShaderParam("draw_outline", to);
    }

    public void SetTintColor(Color color)
    {
        if (_sprite.Material is ShaderMaterial shaderMaterial)
            shaderMaterial.SetShaderParam("tint_color", color);
    }

    public void SetTintAmount(float amount)
    {
        if (_sprite.Material is ShaderMaterial shaderMaterial)
            shaderMaterial.SetShaderParam("tint_effect_factor", amount);
    }

    public void SetAlphaAmount(float amount) => _sprite.Modulate = new Color(Colors.White, amount);

    public void SetSpriteTexture(string location)
    {
        _sprite.Texture = GD.Load<Texture>(location);
        _icon.Texture = GD.Load<Texture>(location);
        
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
        _icon.FlipH = to;
        
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
        _entityRelativeSize = entityRelativeSize;
        UpdateOrigins();
    }

    public void UpdateOrigins()
    {
        var position = _entityRelativeSize.Position;
        var entitySize = _entityRelativeSize.Size;
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
        
        const int widthHalfStep = Constants.TileWidth / 4;
        const int heightHalfStep = Constants.TileHeight / 4;
        
        var offsetFromX = (int)px * new Vector2((int)(widthStep), (int)(heightHalfStep)) + 
                          (int)(sx - 1) * new Vector2((int)(widthHalfStep), (int)(heightHalfStep));
        var offsetFromY = (int)py * new Vector2((int)(widthStep) * -1, (int)(heightHalfStep)) + 
                          (int)(sy - 1) * new Vector2((int)(widthHalfStep) * -1, (int)(heightHalfStep));
        _icon.RectPosition = offsetFromX + offsetFromY + _iconOffset + (SortType is SortTypes.Point 
            ? Vector2.Zero 
            : (xBiggerThanY ? px * Vector2.Down : py * Vector2.Down) * heightHalfStep);
    }

    public void UpdateSpriteBounds()
    {
        var top = _spriteContainer.Scale.x > 0 
            ? _sprite.GlobalPosition + _sprite.Offset 
            : _sprite.GlobalPosition + new Vector2(-_sprite.Offset.x - SpriteSize.x, _sprite.Offset.y);
        var bottom = top + SpriteSize;
        SpriteBounds = new Rect2(top, SpriteSize);
        
        if (DebugEnabled)
            GD.Print($"New {_entityName} sprite bounds: {SpriteBounds}");
        
        UpdateCollisionShape();
        
        //_topOriginSprite.GlobalPosition = SpriteBounds.Position;
        //_bottomOriginSprite.GlobalPosition = SpriteBounds.End;
    }

    public void UpdateElevation(bool isOnHighGround, int yHighGroundOffset, EntityNode entityBelow)
    {
        _isOnHighGround = isOnHighGround;
        YHighGroundOffset = yHighGroundOffset;
        _entityBelow = entityBelow;
    }
    
    public void ResetElevationOffset()
    {
        _sprite.Position = Vector2.Up * 0;
    }

    public void AdjustElevationOffset()
    {
        _sprite.Position = Vector2.Up * YHighGroundOffset;
        _previousSpriteBounds = new Rect2();
        UpdateSpriteBounds();
    }

    public void UpdateZIndex(int to)
    {
        if (_isOnHighGround && _entityBelow != null)
            to = _entityBelow.Renderer.ZIndex + 1;
        
        ZIndex = to;
        _zIndexText.Text = to.ToString();
    }

    public bool ContainsSpriteAt(Vector2 globalPosition)
    {
        var localPosition = _sprite.ToLocal(globalPosition);
        var result = _sprite.IsPixelOpaque(localPosition);
        if (DebugEnabled)
            GD.Print($"{nameof(EntityRenderer)}.{nameof(ContainsSpriteAt)}: {nameof(result)} '{result}' " +
                     $"at {nameof(localPosition)} '{localPosition}', {nameof(globalPosition)} '{globalPosition}'");
        return result;
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
                    GD.Print($"'{renderer1._entityName}' topOrigin: '{renderer1._topOrigin}', " +
                             $"'{renderer2._entityName}' topOrigin: '{renderer2._topOrigin}'.");
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
                ? $"'{renderer2._entityName}' is on top." : $"'{renderer1._entityName}' is on top.";
            GD.Print($"Renderer '{renderer1._entityName}' of type '{renderer1.SortType}' compared to " +
                  $"'{renderer2._entityName}' of type {renderer2.SortType} with the result of {result}. " + resultText);
        }

        if (result == 0 && ((renderer1.IsDynamic && renderer2.IsDynamic is false)
                            || (renderer2.IsDynamic && renderer1.IsDynamic is false)))
        {
            result = renderer1.IsDynamic ? -1 : 1;
            if (DebugEnabled)
                GD.Print(result > 0 ? $"'{renderer2._entityName}' is on top because it's dynamic." 
                    : $"'{renderer1._entityName}' is on top because it's dynamic.");
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

    private void UpdateCollisionShape()
    {
        var previousBounds = _previousSpriteBounds;
        _previousSpriteBounds = SpriteBounds;
        if (previousBounds.Equals(SpriteBounds))
            return;
        
        foreach (var node in _area.GetChildren().OfType<Node>()) 
            node.QueueFree();
        
        var shape = new RectangleShape2D();
        shape.Extents = SpriteBounds.Size / 2;

        var collision = new CollisionShape2D();
        collision.Shape = shape;
        
        _area.AddChild(collision);
        _area.GlobalPosition = SpriteBounds.Position + shape.Extents;
    }
}
