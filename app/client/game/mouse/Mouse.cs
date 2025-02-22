using Godot;

/// <summary>
/// Mouse actions by priority:
/// 1. SELECT (UI, unit, ability): LEFT-UP
/// 2. CANCEL / ESCAPE (UI, unit, ability): RIGHT-UP UI; LEFT-UP (when nothing to select)
/// 3. EXECUTE (movement, attack, ability): RIGHT-UP
/// 4. DRAG CAMERA: LEFT-DOWN (initiates) -> LEFT-HOLD (disables SELECT and CANCEL)
/// 5. EXAMINE / ALT-SELECT (statistics, area) - RIGHT-DOWN (initiates) -> 
///    RIGHT-HOLD (disables EXECUTE) or CTRL-HOLD
/// </summary>
public partial class Mouse : Node2D
{
    [Export(PropertyHint.File, "*.png")] public string ArrowCursorFileLocation { get; set; }
    [Export(PropertyHint.File, "*.png")] public string ArrowCursorSmallFileLocation { get; set; }
    [Export(PropertyHint.File, "*.png")] public string GrabCursorFileLocation { get; set; }
    [Export(PropertyHint.File, "*.png")] public string GrabCursorSmallFileLocation { get; set; }
    [Export(PropertyHint.Range, "0.01,15")] public float MinimumMouseDistanceToStartDrag { get; set; } = 8f;
    
    [Signal] public delegate void MouseDraggedEventHandler(Vector2 by);
    [Signal] public delegate void TakingControlEventHandler(bool flag);
    [Signal] public delegate void LeftReleasedWithoutDragEventHandler();
    [Signal] public delegate void RightReleasedWithoutExamineEventHandler();

    private Resource _arrowCursor;
    private Resource _arrowSmallCursor;
    private Resource _grabCursor;
    private Resource _grabSmallCursor;
    private bool _cameraIsMoving = false;
    private bool _mouseIsOnUi = false;
    private Vector2 _previousPosition = Vector2.Zero;
    private Vector2 _startPosition = Vector2.Zero;

    public override void _Ready()
    {
        _arrowCursor = GD.Load(ArrowCursorFileLocation);
        _arrowSmallCursor = GD.Load(ArrowCursorSmallFileLocation);
        _grabCursor = GD.Load(GrabCursorFileLocation);
        _grabSmallCursor = GD.Load(GrabCursorSmallFileLocation);
        
        SetCursorToArrow();
    }

    public override void _Input(InputEvent inputEvent)
    {
	    base._Input(inputEvent);
	    
	    var mousePos = GetViewport().GetMousePosition();

	    if (Input.IsActionJustPressed(Constants.Input.MouseLeft))
	    {
		    _startPosition = mousePos;
	    }
        
	    if (Input.IsActionPressed(Constants.Input.MouseLeft))
	    {
		    if (_cameraIsMoving)
		    {
			    var changeVector = _previousPosition - mousePos;
			    EmitSignal(nameof(MouseDragged), changeVector);
			    SetCursorToGrab();
			    _previousPosition = mousePos;
		    }
		    else
		    {
			    var initiationVector = mousePos - _startPosition;
			    if (_mouseIsOnUi is false && initiationVector.Length() > MinimumMouseDistanceToStartDrag)
			    {
				    _previousPosition = mousePos;
				    _cameraIsMoving = true;
				    EmitSignal(nameof(TakingControl), true);
			    }
		    }
	    }

	    if (Input.IsActionJustReleased(Constants.Input.MouseLeft))
	    {
		    _cameraIsMoving = false;
		    SetCursorToArrow();
		    EmitSignal(nameof(TakingControl), false);
		    var travelVector = mousePos - _startPosition;
		    if (travelVector.Length() <= MinimumMouseDistanceToStartDrag && _mouseIsOnUi is false) 
			    EmitSignal(nameof(LeftReleasedWithoutDrag));
	    }

	    if (Input.IsActionJustPressed(Constants.Input.MouseRight))
	    {
		    // TODO examine functionality etc
		    // Track holding of key
	    }

	    if (Input.IsActionJustReleased(Constants.Input.MouseRight))
	    {
		    if (_mouseIsOnUi is false)
			    EmitSignal(nameof(RightReleasedWithoutExamine));
	    }
    }
    
    internal void OnInterfaceMouseEntered() => _mouseIsOnUi = true;

    internal void OnInterfaceMouseExited() => _mouseIsOnUi = false;

    private void SetCursorToArrow() => Input.SetCustomMouseCursor(GetArrowCursorCorrectSize());

    private void SetCursorToGrab() => Input.SetCustomMouseCursor(GetGrabCursorCorrectSize());

    private Resource GetArrowCursorCorrectSize() => Config.Instance.LargeCursor
	    ? _arrowCursor
	    : _arrowSmallCursor;

    private Resource GetGrabCursorCorrectSize() => Config.Instance.LargeCursor
	    ? _grabCursor
	    : _grabSmallCursor;
}
