using System;
using Godot;

public static class Log
{
    public const bool VerboseDebugEnabled = false; // Should be used for spammy logs (e.g. frame-based).
    public const bool DebugEnabled = false; 
    
    private static string CurrentPlayerStableId => Network.Instance.TryGetMultiplayer()?.IsServer() ?? true
        ? "S"
        : Players.Instance.Current.StableId.ToString();

    private static string Timestamp => DateTimeOffset.UtcNow.ToString("s");
    
    public static void Info(string @class, string method, string message) 
        => GD.Print($"P{CurrentPlayerStableId}.{Timestamp}.{@class}.{method}: {message}");
    
    public static void Error(string @class, string method, string message) 
        => GD.PrintErr($"P{CurrentPlayerStableId}.{Timestamp}.{@class}.{method}: {message}");
}