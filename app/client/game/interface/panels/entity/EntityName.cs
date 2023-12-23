using Godot;

public class EntityName : MarginContainer
{
    public void SetValue(string value)
    {
        GetNode<Label>("NameLabel").Text = value;
        GetNode<Label>("NameLabel/Shadow").Text = value;
    }
}
