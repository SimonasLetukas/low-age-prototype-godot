using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Godot;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Resources;

public partial class GameMetrics : Node
{
    private const string GameMetricsLocation = "user://metrics";
    private const int TurnCsvIndex = 1;

    private readonly IList<MetricsRow> _preparedRows = [];
    private readonly IList<MetricsRow> _rowsInSaveQueue = [];
    private bool _initiativeQueueWasUpdatedThisTurn;
    
    public override void _Ready()
    {
        base._Ready();
        
        Callable.From(ConnectSignals).CallDeferred();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (_rowsInSaveQueue.Count == 0)
            return;

        var row = _rowsInSaveQueue.First();

        try
        {
            if (RowIsAlreadySaved(row))
            {
                _rowsInSaveQueue.Remove(row);
                return;
            }

            SaveRow(row);
            _rowsInSaveQueue.Remove(row);
        }
        catch (IOException)
        {
            if (Log.VerboseDebugEnabled)
                Log.Info(nameof(GameMetrics), nameof(_Process), 
                    $"Could not save row for player '{row.PlayerStableId}' and turn '{row.Turn}', will retry.");
        }
    }

    public override void _ExitTree()
    {
        EventBus.Instance.PhaseStarted -= OnPhaseStarted;
        EventBus.Instance.ResourcesSpent -= OnResourcesSpent;
        EventBus.Instance.ResourcesGained -= OnResourcesGained;
        EventBus.Instance.PlayerResourcesUpdated -= OnPlayerResourcesUpdated;
        EventBus.Instance.InitiativeQueueUpdated -= OnInitiativeQueueUpdated;
        EventBus.Instance.RawDamageDone -= OnRawDamageDone;
        EventBus.Instance.FinalDamageDone -= OnFinalDamageDone;
        EventBus.Instance.UnitMoved -= OnUnitMoved;
        EventBus.Instance.ActorHealed -= OnActorHealed;
        EventBus.Instance.EntityDestroyed -= OnEntityDestroyed;
        
        base._ExitTree();
    }

    private void ConnectSignals()
    {
        EventBus.Instance.PhaseStarted += OnPhaseStarted;
        EventBus.Instance.ResourcesSpent += OnResourcesSpent;
        EventBus.Instance.ResourcesGained += OnResourcesGained;
        EventBus.Instance.PlayerResourcesUpdated += OnPlayerResourcesUpdated;
        EventBus.Instance.InitiativeQueueUpdated += OnInitiativeQueueUpdated;
        EventBus.Instance.RawDamageDone += OnRawDamageDone;
        EventBus.Instance.FinalDamageDone += OnFinalDamageDone;
        EventBus.Instance.UnitMoved += OnUnitMoved;
        EventBus.Instance.ActorHealed += OnActorHealed;
        EventBus.Instance.EntityDestroyed += OnEntityDestroyed;
    }

    private static bool RowIsAlreadySaved(MetricsRow row)
    {
        var path = GetFilePath();
        if (File.Exists(path) is false)
            return false;

        using var stream = new StreamReader(path);
        using var csv = new CsvReader(stream, CultureInfo.InvariantCulture);
        
        var records = csv.GetRecords<MetricsRow>();
        return records.Any(r => r.PlayerStableId.Equals(row.PlayerStableId) 
                                && r.Turn.Equals(row.Turn));
    }

    private static void SaveRow(MetricsRow row)
    {
        if (DirAccess.Open(GameMetricsLocation) is null)
        {
            var dir = DirAccess.Open("user://");
            dir.MakeDirRecursive(GameMetricsLocation);
        }
        
        var path = GetFilePath();
        var fileExists = File.Exists(path); 
        
        using var stream = new StreamWriter(path, append: true); 
        using var csv = new CsvWriter(stream, CultureInfo.InvariantCulture);
        
        if (fileExists is false)
        {
            csv.WriteHeader<MetricsRow>(); 
            csv.NextRecord();
        }

        csv.WriteRecord(row); 
        csv.NextRecord();
    }

    private static string GetFilePath()
    {
        var gameId = GlobalRegistry.Instance.GameId;
        var godotPath = $"{GameMetricsLocation}/{gameId}.csv";
        var path = ProjectSettings.GlobalizePath(godotPath);
        return path;
    }

    private void FlushPreparedRows()
    {
        if (_preparedRows.Count == 0)
            return;

        foreach (var preparedRow in _preparedRows)
        {
            _rowsInSaveQueue.Add(preparedRow);
        }

        _preparedRows.Clear();
    }
    
    private void AddStartOfTurnMetrics()
    {
        var players = Players.Instance.GetAll();
        var entities = GlobalRegistry.Instance.GetEntities().ToList();
        var revealedTiles = GlobalRegistry.Instance.GetRevealedTilesByAllPlayers();
        
        foreach (var player in players)
        {
            var row = _preparedRows.First(r => r.PlayerStableId.Equals(player.StableId));

            var playerUnits = entities
                .Where(e => e is UnitNode unit && unit.Player.Equals(player))
                .Cast<UnitNode>()
                .ToList();

            row.NumberOfUnits = playerUnits.Count;
            row.UnitHpSum = (int)playerUnits.Sum(u => 
                u.Health?.CurrentAmount ?? 0 + u.Shields?.CurrentAmount ?? 0);
            
            var playerStructures = entities
                .Where(e => e is StructureNode structure && structure.Player.Equals(player))
                .Cast<StructureNode>()
                .ToList();
            
            row.NumberOfStructures = playerStructures.Count;
            row.StructureHpSum = (int)playerStructures.Sum(s => 
                s.Health?.CurrentAmount ?? 0 + s.Shields?.CurrentAmount ?? 0);

            var playerRevealedTiles = revealedTiles
                .Where(t => t.IsVisibleTo(player))
                .ToList();
            
            row.TilesVisibleStartOfTurn = playerRevealedTiles.Count;
        }
    }
    
    private void OnPhaseStarted(int turn, TurnPhase phase)
    {
        if (phase.Equals(TurnPhase.Planning) is false)
            return;
        
        if (_preparedRows.Any())
            FlushPreparedRows();

        var players = Players.Instance.GetAll();
        foreach (var player in players)
        {
            var row = new MetricsRow
            {
                PlayerStableId = player.StableId,
                PlayerName = player.Name,
                FactionId = player.Faction.ToString(),
                Turn = turn
            };
            _preparedRows.Add(row);
        }

        _initiativeQueueWasUpdatedThisTurn = false;

        AddStartOfTurnMetrics();
    }
    
    private void OnResourcesSpent(Player player, IList<Payment> resourcesSpent, bool isRefund)
    {
        var row = _preparedRows.First(r => r.PlayerStableId.Equals(player.StableId));
        foreach (var resourceSpent in resourcesSpent)
        {
            var spentAmount = isRefund ? resourceSpent.Amount * -1 : resourceSpent.Amount;
            
            if (resourceSpent.Resource.Equals(ResourceId.Scraps))
                row.ResourcesScrapsSpent += spentAmount;
            
            if (resourceSpent.Resource.Equals(ResourceId.MeleeWeapon))
                row.ResourcesMeleeWeaponsSpent += spentAmount;
            
            if (resourceSpent.Resource.Equals(ResourceId.RangedWeapon))
                row.ResourcesRangedWeaponsSpent += spentAmount;
            
            if (resourceSpent.Resource.Equals(ResourceId.SpecialWeapon))
                row.ResourcesSpecialWeaponsSpent += spentAmount;
        }
    }
    
    private void OnResourcesGained(Player player, IList<Payment> resourcesGained)
    {
        var row = _preparedRows.FirstOrDefault(r => r.PlayerStableId.Equals(player.StableId));
        
        // Could be null during initialization.
        if (row is null)
            return;
        
        foreach (var resourceGained in resourcesGained)
        {
            if (resourceGained.Resource.Equals(ResourceId.Scraps))
                row.ResourcesScrapsGained += resourceGained.Amount;
            
            if (resourceGained.Resource.Equals(ResourceId.Celestium))
                row.ResourcesCelestiumGained += resourceGained.Amount;
            
            if (resourceGained.Resource.Equals(ResourceId.Population))
                row.ResourcesPopulationGained += resourceGained.Amount;
            
            if (resourceGained.Resource.Equals(ResourceId.MeleeWeapon))
                row.ResourcesMeleeWeaponsGained += resourceGained.Amount;
            
            if (resourceGained.Resource.Equals(ResourceId.RangedWeapon))
                row.ResourcesRangedWeaponsGained += resourceGained.Amount;
            
            if (resourceGained.Resource.Equals(ResourceId.SpecialWeapon))
                row.ResourcesSpecialWeaponsGained += resourceGained.Amount;
            
            if (resourceGained.Resource.Equals(ResourceId.WeaponStorage))
                row.ResourcesWeaponStorageGained += resourceGained.Amount;
            
            if (resourceGained.Resource.Equals(ResourceId.Faith))
                row.ResourcesFaithGained += resourceGained.Amount;
        }
    }
    
    private void OnPlayerResourcesUpdated(Player player, IList<Payment> currentStockpile)
    {
        var row = _preparedRows.FirstOrDefault(r => r.PlayerStableId.Equals(player.StableId));
        
        // Could be null during initialization.
        if (row is null)
            return;

        var meleeWeapons = currentStockpile.FirstOrDefault(r => 
            r.Resource.Equals(ResourceId.MeleeWeapon))?.Amount ?? 0;
        
        var rangedWeapons = currentStockpile.FirstOrDefault(r => 
            r.Resource.Equals(ResourceId.RangedWeapon))?.Amount ?? 0;
        
        var specialWeapons = currentStockpile.FirstOrDefault(r => 
            r.Resource.Equals(ResourceId.SpecialWeapon))?.Amount ?? 0;
        
        var weaponStorage = currentStockpile.FirstOrDefault(r => 
            r.Resource.Equals(ResourceId.WeaponStorage))?.Amount ?? 0;

        row.ResourcesWeaponsSpaceRemaining = weaponStorage - meleeWeapons - rangedWeapons - specialWeapons;
    }
    
    private void OnInitiativeQueueUpdated(IList<ActorNode> actors)
    {
        if (_initiativeQueueWasUpdatedThisTurn)
            return;
        
        var players = Players.Instance.GetAll();
        
        foreach (var player in players)
        {
            var row = _preparedRows.FirstOrDefault(r => r.PlayerStableId.Equals(player.StableId));
            
            // Could be null during initialization.
            if (row is null)
                return;

            var playerActors = actors.Where(a => a.Player.Equals(player)).ToList();

            row.ActorsWithActionAtActionPhaseStart = playerActors.Count;
            row.InitiativeSumAtActionPhaseStart = playerActors.Sum(a => a.Initiative?.CurrentAmount ?? 0);
        }

        _initiativeQueueWasUpdatedThisTurn = true;
    }
    
    private void OnRawDamageDone(EntityNode to, EntityNode from, int amount, DamageType type)
    {
        var sourcePlayerRow = _preparedRows.First(r => r.PlayerStableId.Equals(from.Player.StableId));
        
        if (type.Equals(DamageType.Melee))
        {
            sourcePlayerRow.RawMeleeDamageDone += amount;
        }

        if (type.Equals(DamageType.Ranged))
        {
            sourcePlayerRow.RawRangedDamageDone += amount;
        }
    }
    
    private void OnFinalDamageDone(EntityNode to, EntityNode from, int amount, DamageType type)
    {
        var targetPlayerRow = _preparedRows.First(r => r.PlayerStableId.Equals(to.Player.StableId));
        var sourcePlayerRow = _preparedRows.First(r => r.PlayerStableId.Equals(from.Player.StableId));

        if (type.Equals(DamageType.Melee))
        {
            targetPlayerRow.MeleeDamageReceived += amount;
            sourcePlayerRow.FinalMeleeDamageDone += amount;
        }

        if (type.Equals(DamageType.Ranged))
        {
            targetPlayerRow.RangedDamageReceived += amount;
            sourcePlayerRow.FinalRangedDamageDone += amount;
        }
        
        if (type.Equals(DamageType.Pure))
        {
            targetPlayerRow.PureDamageReceived += amount;
            sourcePlayerRow.FinalPureDamageDone += amount;
        }
    }
    
    private void OnUnitMoved(UnitNode unit, float movementSpent)
    {
        var row = _preparedRows.First(r => r.PlayerStableId.Equals(unit.Player.StableId));
        row.MovedAmount += movementSpent;
    }
    
    private void OnActorHealed(ActorNode actor, int amount)
    {
        var row = _preparedRows.First(r => r.PlayerStableId.Equals(actor.Player.StableId));
        row.HealingDone += amount;
    }
    
    private void OnEntityDestroyed(EntityNode entity, EntityNode? source)
    {
        if (source is null || source.Equals(entity))
            return;
        
        var row = _preparedRows.First(r => r.PlayerStableId.Equals(source.Player.StableId));
        row.KilledAmount++;
    }

    private class MetricsRow
    {
        [Index(0)] public required int PlayerStableId { get; init; }
        [Index(TurnCsvIndex)] public required int Turn { get; init; }
        [Index(2)] public required string PlayerName { get; init; }
        [Index(3)] public required string FactionId { get; init; }
        
        public int ResourcesScrapsSpent { get; set; }
        public int ResourcesMeleeWeaponsSpent { get; set; }
        public int ResourcesRangedWeaponsSpent { get; set; }
        public int ResourcesSpecialWeaponsSpent { get; set; }
        
        public int ResourcesScrapsGained { get; set; }
        public int ResourcesCelestiumGained { get; set; }
        public int ResourcesPopulationGained { get; set; }
        public int ResourcesMeleeWeaponsGained { get; set; }
        public int ResourcesRangedWeaponsGained { get; set; }
        public int ResourcesSpecialWeaponsGained { get; set; }
        public int ResourcesWeaponStorageGained { get; set; }
        public int ResourcesFaithGained { get; set; }
        
        public int ResourcesWeaponsSpaceRemaining { get; set; }
        
        public int NumberOfUnits { get; set; }
        public int UnitHpSum { get; set; }
        public int NumberOfStructures { get; set; }
        public int StructureHpSum { get; set; }
        
        public int RawMeleeDamageDone { get; set; }
        public int FinalMeleeDamageDone { get; set; }
        public int MeleeDamageReceived { get; set; }
        public int RawRangedDamageDone { get; set; }
        public int FinalRangedDamageDone { get; set; }
        public int RangedDamageReceived { get; set; }
        public int FinalPureDamageDone { get; set; }
        public int PureDamageReceived { get; set; }
        
        public int HealingDone { get; set; }
        public float MovedAmount { get; set; }
        public int KilledAmount { get; set; }
        public float InitiativeSumAtActionPhaseStart { get; set; }
        public int ActorsWithActionAtActionPhaseStart { get; set; }
        public int TilesVisibleStartOfTurn { get; set; }
    }
}