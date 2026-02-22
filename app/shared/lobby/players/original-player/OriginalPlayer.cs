using Godot;
using System;
using System.Collections.Generic;

public partial class OriginalPlayer : OptionButton
{
	public event Action<int> OriginalPlayerSelected = delegate { };
	
	private readonly Dictionary<int, (int, SavePlayer)> _originalPlayersByOptionButtonIndex = new();
	private readonly Dictionary<int, int> _optionButtonIndexesByOriginalPlayerStableIds = new();
	
	public override void _Ready()
	{
		base._Ready();
		
		PopulateOriginalPlayers();
		
		Connect("item_selected", new Callable(this, nameof(OnItemSelected)));
		Data.Instance.SaveUpdated += OnSaveUpdated;
	}

	public override void _ExitTree()
	{
		Data.Instance.SaveUpdated -= OnSaveUpdated;
		
		base._ExitTree();
	}
	
	public SavePlayer? SetSelectedOriginalPlayer(int originalPlayerStableId)
	{
		if (Data.Instance.Save is null)
		{
			return null;
		}
		
		var index = _optionButtonIndexesByOriginalPlayerStableIds[originalPlayerStableId];
		Selected = index;
		
		var (_, originalPlayer) = _originalPlayersByOptionButtonIndex[index];
		return originalPlayer;
	}

	private void PopulateOriginalPlayers()
	{
		var save = Data.Instance.Save;
		if (save is null)
			return;
		
		PopulateOriginalPlayers(save);
	}
	
	private void PopulateOriginalPlayers(Save save)
	{
		var previouslySelected = Selected;
		Clear();
        
		var optionButtonIndex = 0;
		foreach (var (originalPlayerStableId, originalPlayer) in save.Players)
		{
			AddItem($"{originalPlayer.OriginalName}", optionButtonIndex);
			_originalPlayersByOptionButtonIndex[optionButtonIndex] = (originalPlayerStableId, originalPlayer);
			_optionButtonIndexesByOriginalPlayerStableIds[originalPlayerStableId] = optionButtonIndex;
			optionButtonIndex++;
		}
		
		Selected = previouslySelected;
	}
	
	private void OnItemSelected(int index)
	{
		var (originalPlayerStableId, _) = _originalPlayersByOptionButtonIndex[index];
		OriginalPlayerSelected(originalPlayerStableId);
	}

	private void OnSaveUpdated(Save save) => PopulateOriginalPlayers(save);
}
