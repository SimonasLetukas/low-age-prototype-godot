using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using LowAgeData.Domain.Entities.Actors.Structures;
using LowAgeData.Domain.Entities.Actors.Units;
using LowAgeTests.Helpers;

namespace LowAgeTests;

public class EqualityTests
{
    private readonly IFixture _fixture = new Fixture()
        .Customize(new AutoMoqCustomization
        {
            ConfigureMembers = true
        });

    private readonly FixtureCustomization<Structure> _structureBlueprint;
    private readonly StructureNode _structure;
    
    private readonly FixtureCustomization<Unit> _unitBlueprint;
    private readonly UnitNode _unit;

    public EqualityTests()
    {
        Data.Instance.ReadBlueprint();
        _structureBlueprint = _fixture
            .For<Structure>()
            .With(x => x.Sprite, "res://assets/sprites/structures/revs/boss post front indexed 2x3.png")
            .With(x => x.BackSideSprite, "res://assets/sprites/structures/revs/boss post back indexed 2x3.png");
        _structure = StructureNode.Instance();
        _structure._Ready();
        _structure.Renderer._Ready();
        
        _unitBlueprint = _fixture
            .For<Unit>()
            .With(x => x.Sprite, "res://assets/sprites/structures/revs/boss post front indexed 2x3.png");
        _unit = UnitNode.Instance();
        _unit._Ready();
        _unit.Renderer._Ready();
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenDifferentTypesOfEntitiesHaveTheSameInstanceId()
    {
        var id = Guid.NewGuid();
        
        var structureBlueprint = _structureBlueprint.Create();
        _structure.SetBlueprint(structureBlueprint);
        _structure.InstanceId = id;
        
        var unitBlueprint = _unitBlueprint.Create();
        _unit.SetBlueprint(unitBlueprint);
        _unit.InstanceId = id;

        _structure.Should().Be(_unit);
    }
    
    [Fact]
    public void Equals_ShouldReturnTrue_WhenSameTypeOfEntitiesHaveTheSameInstanceId()
    {
        var id = Guid.NewGuid();
        
        var unitBlueprint = _unitBlueprint.Create();
        _unit.SetBlueprint(unitBlueprint);
        _unit.InstanceId = id;
        
        var unit2 = UnitNode.Instance();
        unit2._Ready();
        unit2.Renderer._Ready();
        unit2.SetBlueprint(unitBlueprint);
        unit2.InstanceId = id;

        _unit.Should().Be(unit2);
    }
    
    [Fact]
    public void Equals_ShouldReturnFalse_WhenDifferentTypeOfEntitiesHaveDifferentInstanceId()
    {
        var structureBlueprint = _structureBlueprint.Create();
        _structure.SetBlueprint(structureBlueprint);
        
        var unitBlueprint = _unitBlueprint.Create();
        _unit.SetBlueprint(unitBlueprint);

        _structure.Should().NotBe(_unit);
    }
    
    [Fact]
    public void Equals_ShouldReturnFalse_WhenSameTypeOfEntitiesHaveDifferentInstanceId()
    {
        var unitBlueprint = _unitBlueprint.Create();
        _unit.SetBlueprint(unitBlueprint);
        
        var unit2 = UnitNode.Instance();
        unit2._Ready();
        unit2.Renderer._Ready();
        unit2.SetBlueprint(unitBlueprint);

        _unit.Should().NotBe(unit2);
    }
}