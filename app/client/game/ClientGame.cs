using Godot;
using System;

public class ClientGame : Game
{
    private ClientMap _map;
    private Camera _camera;
    private Mouse _mouse;
    private Interface _interface;
    private Data _data;
    
    public override async void _Ready()
    {
        base._Ready();
        
        _map = GetNode<ClientMap>($"{nameof(Map)}");
        _camera = GetNode<Camera>($"{nameof(Camera)}");
        _mouse = GetNode<Mouse>($"{nameof(Mouse)}");
        _interface = GetNode<Interface>($"{nameof(Interface)}");
        _data = Data.Instance;
        
        GD.Print($"{nameof(ClientGame)}: entering.");
        GetTree().Paused = true;
        
        // Wait until the parent scene is fully loaded
        await ToSignal(GetTree().Root.GetChild(GetTree().Root.GetChildCount() - 1), "ready");

        ConnectSignals();
        //InitializeFakeMap();
        MarkAsLoaded();
    }

    private void ConnectSignals()
    {
        _mouse.Connect(nameof(Mouse.LeftReleasedWithoutDrag), _map, nameof(ClientMap.OnMouseLeftReleasedWithoutDrag));
        _mouse.Connect(nameof(Mouse.RightReleasedWithoutExamine), _map, nameof(ClientMap.OnMouseRightReleasedWithoutExamine));

        _mouse.Connect(nameof(Mouse.MouseDragged), _camera, nameof(Camera.OnMouseDragged));
        _mouse.Connect(nameof(Mouse.TakingControl), _camera, nameof(Camera.OnMouseTakingControl));

        _interface.Connect(nameof(Interface.MouseEntered), _mouse, nameof(Mouse.OnInterfaceMouseEntered));
        _interface.Connect(nameof(Interface.MouseExited), _mouse, nameof(Mouse.OnInterfaceMouseExited));

        _map.Connect(nameof(ClientMap.NewTileHovered), _interface, nameof(Interface.OnMapNewTileHovered));

        _map.Connect(nameof(ClientMap.UnitMovementIssued), this, nameof(SendNewUnitPosition));
        _map.Connect(nameof(Map.StartingPositionsDeclared), this, nameof(OnMapStartingPositionsDeclared));
        _data.Connect(nameof(Data.Synchronised), this, nameof(OnDataSynchronized));
    }

    private void InitializeFakeMap()
    {
        var mapSize = new Vector2(100, 100);
        _map.OnMapCreatorMapSizeDeclared(mapSize);
        _camera.OnCreatorMapSizeDeclared(mapSize);
        _interface.OnMapCreatorMapSizeDeclared(mapSize);
    }

    [RemoteSync]
    protected override void GameEnded()
    {
        GD.Print($"{nameof(ClientGame)}: game ended, returning to main menu.");
        Client.Instance.ResetNetwork();
        GetTree().ChangeScene(MainMenu.ScenePath);
    }

    [RemoteSync]
    protected override void OnUnitPositionUpdated(Vector2 entityPosition, Vector2[] globalPath, Vector2[] path)
    {
        _map.MoveUnit(entityPosition, globalPath, path);
    }

    private void OnDataSynchronized()
    {
        // TODO these could listen to data events themselves
        var mapSize = _data.MapSize;
        _camera.OnCreatorMapSizeDeclared(mapSize);
        _interface.OnMapCreatorMapSizeDeclared(mapSize);
    }

    private void OnMapStartingPositionsDeclared(Vector2[] startingPositions)
    {
        GetTree().Paused = false;
        // TODO
    }
}