using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeCommon;
using LowAgeCommon.Extensions;
using LowAgeData.Domain.Common;

public partial class InitiativePanel : Control
{
	public event Action<ActorNode?> ActorHovered = delegate { };
	public event Action<ActorNode?> ActorSelected = delegate { }; // TODO also handle deselect and force select outside of this
	
	private const float PanelMoveDuration = 0.05f;
	
	private NinePatchRect _background = null!;
	private HBoxContainer _container = null!;

	private bool _hidden = false;
	private IList<ActorNode> _currentInitiativeQueue = [];
	private ActorNode? _currentlySelectedActor;
	private InitiativeButton? _previouslyHoveredButton;
	private readonly Dictionary<ActorNode, InitiativeButton> _buttonsByActor = new();
	
	public override void _Ready()
	{
		base._Ready();
		
		_background = GetNode<NinePatchRect>("Background");
		_container = GetNode<HBoxContainer>($"{nameof(ScrollContainer)}/{nameof(MarginContainer)}/{nameof(HBoxContainer)}");
		
		Reset();
		HidePanel();

		EventBus.Instance.InitiativeQueueUpdated += OnInitiativeQueueUpdated;
		EventBus.Instance.NewTileFocused += OnNewTileFocused;
	}

	public override void _ExitTree()
	{
		EventBus.Instance.InitiativeQueueUpdated -= OnInitiativeQueueUpdated;
		EventBus.Instance.NewTileFocused -= OnNewTileFocused;
		
		base._ExitTree();
	}

	private void OnInitiativeQueueUpdated(IList<ActorNode> actors)
	{
		// TODO handle if there were previously selected actors that are still in the queue now

		_currentInitiativeQueue = actors;
		
		if (_currentInitiativeQueue.IsEmpty())
		{
			HidePanel();
			return;
		}
		
		Reset();
		Populate();

		if (_hidden)
		{
			ShowPanel();
		}
	}

	private void Populate()
	{
		foreach (var actor in _currentInitiativeQueue)
		{
			var initiativeButton = InitiativeButton.InstantiateAsChild(actor, _container);
			initiativeButton.Clicked += OnInitiativeButtonClicked;
			initiativeButton.Hovering += OnInitiativeButtonHovering;
			_buttonsByActor[actor] = initiativeButton;
		}
	}

	private void OnInitiativeButtonClicked(ActorNode actor)
	{
		_currentlySelectedActor = actor switch
		{
			_ when _currentlySelectedActor is null => actor,
			_ when _currentlySelectedActor.Equals(actor) => null,
			_ => actor,
		};
		
		ActorSelected(_currentlySelectedActor);
	}

	private void OnInitiativeButtonHovering(bool flag, ActorNode actor)
	{
		if (_previouslyHoveredButton != null && _previouslyHoveredButton.Actor.Equals(actor))
			return;
		
		ActorHovered(flag ? actor : null);
	}

	private void HidePanel()
	{
		_currentlySelectedActor = null;
		_hidden = true;
		
		var tween = CreateTween();
		tween.TweenProperty(this, "offset_top", -_background.Size.Y * 4, PanelMoveDuration)
			.FromCurrent()
			.SetTrans(Tween.TransitionType.Quad);

		var tweenHide = CreateTween();
		tweenHide.TweenProperty(this, "visible", false, PanelMoveDuration)
			.FromCurrent()
			.SetDelay(PanelMoveDuration);
	}

	private void ShowPanel()
	{
		Visible = true;
		_hidden = false;
		
		var tween = CreateTween();
		tween.TweenProperty(this, "offset_top", 0, PanelMoveDuration)
			.FromCurrent()
			.SetTrans(Tween.TransitionType.Quad);
	}

	private void Reset()
	{
		_buttonsByActor.Clear();
		_previouslyHoveredButton = null;
		
		foreach (var child in _container.GetChildren())
		{
			if (child is InitiativeButton initiativeButton)
			{
				initiativeButton.Clicked -= OnInitiativeButtonClicked;
				initiativeButton.Hovering -= OnInitiativeButtonHovering;
			}
			
			child.QueueFree();
		}
	}

	private void OnNewTileFocused(Vector2Int mapPosition, Terrain terrain, IList<EntityNode>? occupants)
	{
		_previouslyHoveredButton?.SetHovering(false);

		if (occupants is null || occupants.IsEmpty() || _currentInitiativeQueue.IsEmpty())
			return;

		if (occupants.FirstOrDefault(o => o is ActorNode { HasInitiative: true }) 
		    is not ActorNode actorWithInitiative)
			return;

		if (_buttonsByActor.TryGetValue(actorWithInitiative, out var button) is false)
			return;
		
		_previouslyHoveredButton = button;
		button.SetHovering(true);
	}
}
