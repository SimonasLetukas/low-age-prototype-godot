using Godot;

public partial class PlayerAttributes : VBoxContainer
{
	private const string Separator = " - ";
	
	private readonly Color _separatorLineColorEnemy = new("e1001887");	
	private readonly Color _separatorLineColorNormal = new("e0d1bf87");

	private Player? Player { get; set; }

	private HSeparator _hSeparator = null!;
	private Label _label = null!;
	
	public override void _Ready()
	{
		_hSeparator = GetNode<HSeparator>(nameof(HSeparator));
		_label = GetNode<Label>($"{nameof(MarginContainer)}/{nameof(Label)}");
	}

	public void Set(Player player)
	{
		if (Player is not null && Player.Id.Equals(player.Id))
			return;
		
		Player = player;

		if (Player is null)
			return;

		var isEnemy = Players.Instance.IsCurrentPlayerEnemyTo(player);

		UpdateLine(isEnemy);
		UpdateLabel(Player.Name, isEnemy);
	}

	private void UpdateLine(bool isEnemy) => _hSeparator.SelfModulate = isEnemy 
		? _separatorLineColorEnemy 
		: _separatorLineColorNormal;

	private void UpdateLabel(string playerName, bool isEnemy)
	{
		var teamText = isEnemy ? "Enemy" : "Ally";
		_label.Text = $"{playerName}{Separator}{teamText}";
	}
}
