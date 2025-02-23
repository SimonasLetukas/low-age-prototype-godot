using System;
using System.Collections.Generic;
using Godot;
using LowAgeCommon;
using Area = LowAgeCommon.Area;

public partial class EntityRenderer : Node2D
{
    [Export]
    public bool DebugEnabled { get; set; } = false;
    
    public enum SortTypes
    {
        Point,
        Line
    }

    public Guid InstanceId => _parentEntity?.InstanceId ?? Guid.NewGuid();
    public bool Registered { get; set; } = false;
    public bool IsDynamic { get; private set; } = false;
    public Rect2 SpriteBounds { get; private set; }
    public SortTypes SortType { get; private set; } = SortTypes.Point;
    public Vector2 SpriteSize => _sprite.Texture.GetSize();
    public int YHighGroundOffset { get; private set; } = 0;

    public readonly List<EntityRenderer> StaticDependencies = [];
    public readonly List<EntityRenderer> DynamicDependencies = [];

    private Color _outlineColorNormal = Colors.Black;
    private Color _outlineColorEnemy = Colors.Red;
    
    private EntityNode? _parentEntity;
    private Area _entityRelativeSize;
    
    private Vector2 _topOrigin;
    private Vector2 _bottomOrigin;

    private Node2D _debugVisuals = null!;
    private Sprite2D _topOriginSprite = null!;
    private Sprite2D _bottomOriginSprite = null!;
    private RichTextLabel _zIndexText = null!;

    private Node2D _spriteContainer = null!;
    private Sprite2D _sprite = null!;
    private TextureRect _icon = null!;
    private readonly Vector2 _iconOffset = new(-4, -5);

    private Area2D _area = null!;
    private Rect2 _previousSpriteBounds = new();

    private EntityNode? _entityBelow = null;
    private bool _isOnHighGround = false;

    public Vector2 AsPoint => SortType is SortTypes.Point 
        ? _topOrigin 
        : (_topOrigin + _bottomOrigin) / 2;

    private float SortingLineCenterHeight => SortType is SortTypes.Point
        ? _topOrigin.Y
        : (_topOrigin.Y + _bottomOrigin.Y) / 2;

    public override void _Ready()
    {
        _debugVisuals = GetNode<Node2D>($"Debug");
        _topOriginSprite = GetNode<Sprite2D>("Debug/OriginTop");
        _bottomOriginSprite = GetNode<Sprite2D>("Debug/OriginBottom");
        _zIndexText = GetNode<RichTextLabel>($"Debug/{nameof(RichTextLabel)}");
        _spriteContainer = GetNode<Node2D>($"SpriteContainer");
        _sprite = GetNode<Sprite2D>($"SpriteContainer/{nameof(Sprite2D)}");
        _icon = GetNode<TextureRect>($"Icon");
        _area = GetNode<Area2D>(nameof(Area2D));

        _icon.Texture = null;
        _debugVisuals.Visible = DebugEnabled;

        EventBus.Instance.WhenFlattenedChanged += OnWhenFlattenedChanged;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        
        EventBus.Instance.WhenFlattenedChanged -= OnWhenFlattenedChanged;
    }

    public void Initialize(EntityNode parentEntity, bool isDynamic)
    {
        _parentEntity = parentEntity;
        ZIndex = 0;
        IsDynamic = isDynamic;

        var entityRelativeSize = parentEntity.RelativeSize;
        SortType = entityRelativeSize.Size.X == entityRelativeSize.Size.Y ? SortTypes.Point : SortTypes.Line;
        
        AdjustToRelativeSize(entityRelativeSize);
        SetSpriteVisibility(true);
    }

    public void SetSpriteVisibility(bool to) => _spriteContainer.Visible = to;

    public void SetIconVisibility(bool to) => _icon.Visible = to;

    public void MakeDynamic() => IsDynamic = true;
    public void MakeStatic() => IsDynamic = false;

    public void SetOutline(bool to, bool isEnemy)
    {
        if (_sprite.Material is not ShaderMaterial spriteShaderMaterial) 
            return;
        
        spriteShaderMaterial.SetShaderParameter("draw_outline", to);
        spriteShaderMaterial.SetShaderParameter("outline_color", isEnemy 
            ? _outlineColorEnemy 
            : _outlineColorNormal);
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
        _sprite.Texture = GD.Load<Texture2D>(location);
        _icon.Texture = GD.Load<Texture2D>(location);
        
        UpdateSpriteBounds();
    }

    public void UpdateSpriteOffset(Vector2Int entitySize, Vector2 centerOffset)
    {
        const int quarterWidth = Constants.TileWidth / 4;
        const int quarterHeight = Constants.TileHeight / 4;
        var offsetFromX = (entitySize.X - 1) * new Vector2(quarterWidth, quarterHeight);
        var offsetFromY = (entitySize.Y - 1) * new Vector2(quarterWidth * -1, quarterHeight);
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
        _spriteContainer.Scale = GlobalPosition.X > target.X
            ? new Vector2(-1, _spriteContainer.Scale.Y)
            : new Vector2(1, _spriteContainer.Scale.Y);
        
        UpdateSpriteBounds();
    }

    public void AdjustToRelativeSize(Area entityRelativeSize)
    {
        _entityRelativeSize = entityRelativeSize;
        UpdateOrigins();
    }

    public void UpdateOrigins()
    {
        var position = _entityRelativeSize.Start;
        var entitySize = _entityRelativeSize.Size;
        var xBiggerThanY = entitySize.X > entitySize.Y;

        var px = position.X;
        var py = position.Y;
        var sx = entitySize.X;
        var sy = entitySize.Y;
        
        const int widthStep = Constants.TileWidth / 2;
        const int heightStep = Constants.TileHeight / 2;
        
        _topOrigin = GlobalPosition + (SortType is SortTypes.Point
            ? entitySize.X > 1 ? new Vector2(0, (entitySize.X - 1) * heightStep) : Vector2.Zero
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
        
        var offsetFromX = px * new Vector2(widthStep, heightHalfStep) + 
                          (sx - 1) * new Vector2(widthHalfStep, heightHalfStep);
        var offsetFromY = py * new Vector2(widthStep * -1, heightHalfStep) + 
                          (sy - 1) * new Vector2(widthHalfStep * -1, heightHalfStep);
        _icon.Position = offsetFromX + offsetFromY + _iconOffset + (SortType is SortTypes.Point 
            ? Vector2.Zero 
            : (xBiggerThanY ? px * Vector2.Down : py * Vector2.Down) * heightHalfStep);
    }

    public void UpdateSpriteBounds()
    {
        var top = _spriteContainer.Scale.X > 0 
            ? _sprite.GlobalPosition + _sprite.Offset 
            : _sprite.GlobalPosition + new Vector2(-_sprite.Offset.X - SpriteSize.X, _sprite.Offset.Y);
        var bottom = top + SpriteSize;
        SpriteBounds = new Rect2(top, SpriteSize);
        
        if (DebugEnabled)
            GD.Print($"New {_parentEntity?.DisplayName} sprite bounds: {SpriteBounds}");
        
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
        _sprite.Position = Vector2.Up * (ClientState.Instance.Flattened && _isOnHighGround
            ? Constants.FlattenedHighGroundHeight 
            : YHighGroundOffset);
        _previousSpriteBounds = new Rect2();
        UpdateSpriteBounds();
    }

    public void UpdateZIndex(int to)
    {
        if (_isOnHighGround && _entityBelow != null)
            to = _entityBelow.Renderer.ZIndex + 1;
        
        ZIndex = to;
        _zIndexText.Text = to.ToString();

        EventBus.Instance.RaiseEntityZIndexUpdated(_parentEntity!, ZIndex);
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
                    GD.Print($"'{renderer1._parentEntity!.DisplayName}' topOrigin: '{renderer1._topOrigin}', " +
                             $"'{renderer2._parentEntity!.DisplayName}' topOrigin: '{renderer2._topOrigin}'.");
                result = renderer2._topOrigin.Y.CompareTo(renderer1._topOrigin.Y);
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
                ? $"'{renderer2._parentEntity!.DisplayName}' at '{renderer2._parentEntity.EntityPrimaryPosition}' is on top." 
                : $"'{renderer1._parentEntity!.DisplayName}' at '{renderer1._parentEntity.EntityPrimaryPosition}' is on top.";
            GD.Print($"Renderer '{renderer1._parentEntity!.DisplayName}' at " +
                     $"'{renderer1._parentEntity!.EntityPrimaryPosition}' of type '{renderer1.SortType}' compared to " +
                     $"'{renderer2._parentEntity!.DisplayName}' at '{renderer2._parentEntity.EntityPrimaryPosition}' " +
                     $"of type '{renderer2.SortType}' with the result of {result}. " + resultText);
        }

        if (result == 0 && ((renderer1.IsDynamic && renderer2.IsDynamic is false)
                            || (renderer2.IsDynamic && renderer1.IsDynamic is false)))
        {
            result = renderer1.IsDynamic ? -1 : 1;
            if (DebugEnabled)
                GD.Print(result > 0 ? $"'{renderer2._parentEntity!.DisplayName}' is on top because it's dynamic." 
                    : $"'{renderer1._parentEntity!.DisplayName}' is on top because it's dynamic.");
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
        return line1.SortingLineCenterHeight.CompareTo(line2.SortingLineCenterHeight);
    }

    private static int ComparePointAndLine(Vector2 point, EntityRenderer line)
    {
        var pointY = point.Y;
        if (pointY > line._topOrigin.Y && pointY > line._bottomOrigin.Y)
        {
            return -1;
        }

        if (pointY < line._topOrigin.Y && pointY < line._bottomOrigin.Y)
        {
            return 1;
        }

        var slope = (line._bottomOrigin.Y - line._topOrigin.Y) / (line._bottomOrigin.X - line._topOrigin.X);
        var intercept = line._topOrigin.Y - (slope * line._topOrigin.X);
        var yOnLineForPoint = (slope * point.X) + intercept;
        return yOnLineForPoint > point.Y ? 1 : -1;
    }

    private void UpdateCollisionShape()
    {
        var previousBounds = _previousSpriteBounds;
        _previousSpriteBounds = SpriteBounds;
        if (previousBounds.Equals(SpriteBounds))
            return;
        
        foreach (var node in _area.GetChildren()) 
            node.QueueFree();
        
        var shape = new RectangleShape2D();
        shape.Size = SpriteBounds.Size;

        var collision = new CollisionShape2D();
        collision.Shape = shape;
        
        _area.AddChild(collision);
        _area.GlobalPosition = SpriteBounds.Position + (shape.Size / 2);
    }

    private void OnWhenFlattenedChanged(bool to) => AdjustElevationOffset();
}
