using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeCommon.Extensions;
using LowAgeData.Domain.Common;
using Resource = LowAgeData.Domain.Resources.Resource;

public partial class ResourceItem : Control
{
	private const string ScenePath = @"res://app/client/game/interface/panels/resource/ResourceItem.tscn";
	private static ResourceItem Instance() => (ResourceItem) GD.Load<PackedScene>(ScenePath).Instantiate();
	public static ResourceItem InstantiateAsChild(Node parentNode, Resource resource, Player player)
	{
		var resourceItem = Instance();
		parentNode.AddChild(resourceItem);
		resourceItem.Initialize(resource, player);
		return resourceItem;
	}

	private Text _text = null!;
	private TextureRect _textureRect = null!;
	private Resource _resource = null!;
	private Player _player = null!;

	private void Initialize(Resource resource, Player player)
	{
		_resource = resource;
		_player = player;
		
		UpdateIcon();
		UpdateAmount();
		UpdateTooltip();

		EventBus.Instance.PlayerResourcesUpdated += OnPlayerResourcesUpdated;
	}

	public override void _Ready()
	{
		base._Ready();

		_text = GetNode<Text>(nameof(Text));
		_textureRect = GetNode<TextureRect>(nameof(TextureRect));
	}

	public override void _ExitTree()
	{
		EventBus.Instance.PlayerResourcesUpdated -= OnPlayerResourcesUpdated;
		
		base._ExitTree();
	}

	private void UpdateIcon()
	{
		if (_resource.Sprite is null)
			return;
		
		var icon = GD.Load<Texture2D>(_resource.Sprite);
		_textureRect.Texture = icon;
	}

	private void UpdateAmount()
	{
		var currentStockpile = GlobalRegistry.Instance.GetCurrentPlayerStockpile(_player);
		var resourceAmount = currentStockpile.FirstOrDefault(p => p.Resource.Equals(_resource.Id));
		if (resourceAmount is null)
			return;
		
		UpdateAmount(resourceAmount);
	}

	private void UpdateAmount(Payment to) => _text.Text = "[center]" + to.Amount;

	private void UpdateTooltip()
	{
		var stockpile = GlobalRegistry.Instance.GetCurrentPlayerStockpile(_player);
		var resourceAmount = stockpile.FirstOrDefault(p => p.Resource.Equals(_resource.Id))?.Amount 
		                     ?? 0;
		
		var income = GlobalRegistry.Instance.GetMaximumPlayerIncome(_player);
		var incomeAmount = income.FirstOrDefault(p => p.Resource.Equals(_resource.Id))?.Amount ?? 0;

		var incomeText = _resource is { IsConsumable: true, HasBank: true }
			? $"\nGain: {incomeAmount} (at the start of each planning phase)"
			: string.Empty;
		
		var storedAsResource = Data.Instance.Blueprint.Resources
			.FirstOrDefault(r => r.Id.Equals(_resource.StoredAs));
		var currentlyStored = GlobalRegistry.Instance.GetResourcesStoredAs(_resource.StoredAs, stockpile)
			.Select(r => r.Amount)
			.Sum();
		var storedAsAmount = stockpile.FirstOrDefault(p => p.Resource.Equals(_resource.StoredAs))?.Amount 
		                     ?? 0;
		var maximumStorageText = _resource.StoredAs.Equals(_resource.Id) is false && storedAsResource is not null
			? $"\nCapacity: {currentlyStored}/{storedAsAmount} (stored as {storedAsResource.DisplayName})"
			: string.Empty;

		var negativeDescription = _resource.NegativeIncomeDescription.Equals(string.Empty) is false 
		                          && resourceAmount < 0 
			? $"\n\n{_resource.NegativeIncomeDescription}"
			: string.Empty;

		var finalText = $"{_resource.DisplayName}\n\n" +
		                $"{_resource.Description}\n\n" +
		                $"Amount: {resourceAmount}" +
		                incomeText +
		                maximumStorageText +
		                negativeDescription;
		
		TooltipText = finalText.WrapToLines(Constants.MaxTooltipCharCount);
	}
	
	private void OnPlayerResourcesUpdated(Player player, IList<Payment> currentStockpile)
	{
		if (player.Equals(_player) is false)
			return;

		var resourceAmount = currentStockpile.FirstOrDefault(p => p.Resource.Equals(_resource.Id));
		if (resourceAmount is null)
			return;
		
		UpdateAmount(resourceAmount);
		UpdateTooltip();
	}
}
