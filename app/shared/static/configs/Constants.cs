using Godot;

public static class Constants
{
    public static class Input
    {
        public const string MouseLeft = "mouse_left";
        public const string MouseRight = "mouse_right";
        public const string Rotate = "rotate";
        public const string Flatten = "flatten";
        public const string MovementAttackToggle = "movement_attack_toggle";
        public const string FocusSelection = "focus_selection";
        public const string RepeatPlacement = "repeat_placement";
    }
    
    public static class ENet
    {
        public const string ConnectedToServerEvent = "connected_to_server";
        public const string NetworkPeerConnectedEvent = "network_peer_connected";
        public const string NetworkPeerDisconnectedEvent = "network_peer_disconnected";
        
        public static string ServerIp { get; private set; }
        public const int ServerPort = 3000;
        public const int ServerId = 1;
        public const int MaxPlayers = 2;
        public const int TimeoutLimitMs = 20_000;
        public const int TimeoutMinimumMs = 30_000;
        public const int TimeoutMaximumMs = 60_000;

        public static void SetLocalServer()
        {
            ServerIp = "127.0.0.1";
        }

        public static void SetRemoteServer()
        {
            ServerIp = "35.228.22.84"; // TODO move to secrets or configs
        }
    }

    public static class Pathfinding
    {
        public const int MaxSizeForPathfinding = 3;
        public const float DiagonalCost = Mathf.Sqrt2;
        public const int ImpassableIndex = -1;
        public const int HighGroundIndex = -2;
        
        /// <summary>
        /// TODO: 0.5 is added for smoother corners to align with circles from IShape, decide if this is needed.
        /// Argument against: not moving straight diagonally is more cost-effective, which is not intuitive for the
        /// player. This bonus could be added for units with 1 movement only.
        /// </summary>
        public const float SearchIncrement = 0.5f;
    }

    public static class Os
    {
        public const string Username = "USERNAME";
    }

    public const int TileWidth = 16;
    public const int TileHeight = 8;

    public const int FlattenedHighGroundHeight = 2;

    public const int MaxTooltipCharCount = 40;
}
