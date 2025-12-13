public readonly record struct ValidationResult(
    bool IsValid,
    string Message = "")
{
    public static ValidationResult Valid => new(true);
    public static ValidationResult Invalid(string why) => new(false, why);
}