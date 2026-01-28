using Godot;

public static class FloatExtensions
{
    public static string ToDisplayFormat(this float value)
    {
        var snapped = Mathf.Snapped(value, 0.1f);
        return $"{snapped:0.#}";
    }
}