﻿using System;
using low_age_data.Domain.Entities;
using low_age_prototype_common;

public class EntityPlacedEvent : IGameEvent
{
    public EntityPlacedEvent(EntityId blueprintId, Vector2<int> mapPosition, Guid instanceId, 
        ActorRotation actorRotation)
    {
        BlueprintId = blueprintId;
        MapPosition = mapPosition;
        InstanceId = instanceId;
        ActorRotation = actorRotation;
    }
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public EntityId BlueprintId { get; }
    public Vector2<int> MapPosition { get; }
    public Guid InstanceId { get; }
    public ActorRotation ActorRotation { get; }
}