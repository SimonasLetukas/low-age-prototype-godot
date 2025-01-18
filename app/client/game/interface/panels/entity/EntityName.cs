using Godot;

public partial class EntityName : MarginContainer
{
    public void SetValue(string value)
    {
        GetNode<Label>("NameLabel").Text = value;
    }
}
