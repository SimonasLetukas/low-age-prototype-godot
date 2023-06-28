public static class Constants
{
    public static class ENet
    {
        public const string ConnectedToServerEvent = "connected_to_server";
        public const string NetworkPeerConnectedEvent = "network_peer_connected";
        public const string NetworkPeerDisconnectedEvent = "network_peer_disconnected";
    }

    public static class Os
    {
        public const string Username = "USERNAME";
    }

    public static class Game
    {
        public enum Faction
        {
            Revelators,
            Uee
        }

        public enum Terrain
        {
            Grass,
            Mountains,
            Marsh,
            Scraps,
            Celestium
        }
    }

    public static string ServerIp { get; private set; }
    public const int ServerPort = 3000;
    public const int ServerId = 1;
    public const int MaxPlayers = 2;

    public static void SetLocalServer()
    {
        ServerIp = "127.0.0.1";
    }

    public static void SetRemoteServer()
    {
        ServerIp = "35.228.22.84"; // TODO move to secrets
    }
}
