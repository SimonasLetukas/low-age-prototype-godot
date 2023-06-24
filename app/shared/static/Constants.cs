public static class Constants
{
    public static class ENet
    {
        public const string ConnectedToServerEvent = "connected_to_server";
    }

    public static class Os
    {
        public const string Username = "USERNAME";
    }

    public static string ServerIp { get; private set; }
    public const int ServerPort = 3000;

    public static void SetLocalServer()
    {
        ServerIp = "127.0.0.1";
    }

    public static void SetRemoteServer()
    {
        ServerIp = "35.228.22.84"; // TODO move to secrets
    }
}
