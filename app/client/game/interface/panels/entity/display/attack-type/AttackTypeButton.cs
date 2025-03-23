using Godot;

public partial class AttackTypeButton : BaseButton
{
	public override void _Ready()
	{
		base._Ready();

		TintColor = new Color("c1d7df");
		SetTint(true);
	}
	
	protected override void SetClicked(bool to)
	{
		switch (to)
		{
			case true:
				Texture = TextureClicked;
				TextureRect.Modulate = new Color(TextureRect.Modulate, 0.7f);
				break;
			case false:
				Texture = TextureNormal;
				TextureRect.Modulate = new Color(TextureRect.Modulate, 1f);
				break;
		}
	}
}
