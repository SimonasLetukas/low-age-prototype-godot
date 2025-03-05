using System;
using System.Collections.Generic;
using Godot;
using MultipurposePathfinding;

public partial class TeamSelection : OptionButton
{
	public event Action<Team> TeamSelected = delegate { };

	private readonly Dictionary<int, Team> _teamsByOptionButtonIndex = new();
	private readonly Dictionary<Team, int> _optionButtonIndexesByTeam = new();
	
	public override void _Ready()
	{
		base._Ready();
		
		PopulateTeams();
		
		Connect("item_selected", new Callable(this, nameof(OnItemSelected)));
		Players.Instance.PlayerAdded += OnPlayerAdded;
		Players.Instance.PlayerRemoved += OnPlayerRemoved;
	}

	public override void _ExitTree()
	{
		Players.Instance.PlayerAdded -= OnPlayerAdded;
		Players.Instance.PlayerRemoved -= OnPlayerRemoved;
		
		base._ExitTree();
	}

	public void SetSelectedTeam(Team team)
	{
		var index = _optionButtonIndexesByTeam[team];
		Selected = index;
	}
	
	private void PopulateTeams()
	{
		var previouslySelected = Selected;
		Clear();
        
		var optionButtonIndex = 0;
		foreach (var team in Players.Instance.GetAvailableTeams())
		{
			AddItem($"Team {team}", optionButtonIndex);
			_teamsByOptionButtonIndex[optionButtonIndex] = team;
			_optionButtonIndexesByTeam[team] = optionButtonIndex;
			optionButtonIndex++;
		}
		
		Selected = previouslySelected;
	}
	
	private void OnItemSelected(int index)
	{
		var team = _teamsByOptionButtonIndex[index];
		TeamSelected(team);
	}

	private void OnPlayerAdded(int playerId) => PopulateTeams();

	private void OnPlayerRemoved(int playerId) => PopulateTeams();
}
