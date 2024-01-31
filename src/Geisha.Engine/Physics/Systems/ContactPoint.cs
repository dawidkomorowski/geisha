using System;
using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Physics.Systems;

// TODO Add documentation?
public readonly struct ContactPoint
{
    public Vector2 WorldPosition { get; }
    public Vector2 LocalPositionA { get; }
    public Vector2 LocalPositionB { get; }
    public Vector2 CollisionNormal { get; }
    public double SeparationDepth { get; }
}

internal readonly struct Contact
{
    public KinematicBody Body1 { get; }
    public StaticBody Body2 { get; } // TODO How to uniformly represent static and kinematic bodies?
    public ContactPoint Point { get; }
    public bool IsValid { get; } // TODO Temp solution to know if contact exists.
}

internal static class ContractGenerator
{
    public static Contact GenerateContactForCircleVsCircle(in Circle c1, in Circle c2, in SeparationInfo separationInfo)
    {
        throw new NotImplementedException();
    }
}