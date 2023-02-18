public static class Constants
{
    public static string ServerIp { get; private set; }

    public static void SetLocalServer()
    {
        ServerIp = "127.0.0.1";
    }
}
