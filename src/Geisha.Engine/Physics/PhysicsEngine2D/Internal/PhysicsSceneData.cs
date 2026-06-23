using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Physics.PhysicsEngine2D.Internal;

internal struct PhysicsSceneData
{
    // TODO: Maybe it is worth to encapsulate free-list logic into dedicated generic data structure?
    private static readonly PhysicsSceneData[] Scenes = new PhysicsSceneData[32];
    private static int FirstFreeIndex = 0;
    private const int NoFreeIndex = -1;

    static PhysicsSceneData()
    {
        for (var i = 0; i < Scenes.Length; i++)
        {
            Scenes[i].NextFreeIndex = i + 1;
        }

        Scenes[^1].NextFreeIndex = NoFreeIndex;
    }

    public static PhysicsSceneId Create(in PhysicsScene2DDefinition sceneDefinition)
    {
        if (FirstFreeIndex == NoFreeIndex)
        {
            throw new InvalidOperationException("There are no available free slots to allocate new Physics Scene.");
        }

        var scene = new PhysicsSceneData
        {
            Index = FirstFreeIndex,
            Version = Scenes[FirstFreeIndex].Version + 1,
            NextFreeIndex = Scenes[FirstFreeIndex].NextFreeIndex,
            SimulationParameters = new SimulationParameters
            {
                Substeps = sceneDefinition.Substeps > 0 ? sceneDefinition.Substeps : 1,
                VelocityIterations = sceneDefinition.VelocityIterations > 0 ? sceneDefinition.VelocityIterations : 4,
                PositionIterations = sceneDefinition.PositionIterations > 0 ? sceneDefinition.PositionIterations : 4,
                PenetrationTolerance = sceneDefinition.PenetrationTolerance >= 0 ? sceneDefinition.PenetrationTolerance : 0.01
            },
            TileSize = sceneDefinition.TileSize,
            TileMap = new TileMap(sceneDefinition.TileSize),
            BodyIndices = new List<BodyIndex>(),
            Bodies = new List<RigidBodyData>(),
            StaticBodyIndices = new List<int>(),
            KinematicBodyIndices = new List<int>(),
            Contacts = new List<ContactData>(256),
            SensorOverlapCache = new SensorOverlapCache(256),
            SensorOverlapEvents = new List<SensorOverlapEvent>(256)
        };

        Scenes[FirstFreeIndex] = scene;

        FirstFreeIndex = Scenes[FirstFreeIndex].NextFreeIndex;

        return scene.PhysicsSceneId;
    }

    public static void Destroy(PhysicsSceneId id)
    {
        // TODO: Implement deletion of allocated physics scene by physics system.
        // TODO: Reuse list slots.
        throw new NotImplementedException("Implement scene destroy.");
    }

    public static ref PhysicsSceneData Get(PhysicsSceneId id)
    {
        if (!id.IsValid)
        {
            throw new ArgumentException("Invalid scene ID.");
        }

        // TODO: Validate ID.
        return ref Scenes[id.Index];
    }

    private PhysicsSceneId PhysicsSceneId => new(Index, Version);

    public int Index;
    public int Version;
    public int NextFreeIndex;

    public SimulationParameters SimulationParameters;

    public SizeD TileSize;
    public TileMap TileMap;

    public List<BodyIndex> BodyIndices;
    public Span<BodyIndex> BodyIndicesSpan => CollectionsMarshal.AsSpan(BodyIndices);
    public List<RigidBodyData> Bodies;
    public Span<RigidBodyData> BodiesSpan => CollectionsMarshal.AsSpan(Bodies);
    public List<int> StaticBodyIndices;
    public List<int> KinematicBodyIndices;

    public List<ContactData> Contacts;
    public Span<ContactData> ContactsSpan => CollectionsMarshal.AsSpan(Contacts);

    public SensorOverlapCache SensorOverlapCache;
    public List<SensorOverlapEvent> SensorOverlapEvents;

    public ref RigidBodyData CreateBody(BodyType bodyType)
    {
        var rigidBodyId = new RigidBodyId(PhysicsSceneId, BodyIndices.Count, 1);

        // TODO: Reuse index slots.
        BodyIndices.Add(default);
        ref var bodyIndex = ref BodyIndicesSpan[rigidBodyId.Index];
        bodyIndex.DenseIndex = Bodies.Count;
        bodyIndex.Version = rigidBodyId.Version;

        var body = new RigidBodyData
        {
            RuntimeId = RuntimeId.Next(),
            Id = rigidBodyId,
            Type = bodyType,
            CollisionNormalFilter = CollisionNormalFilter.None,
            EnableCollisionDetection = true,
            CollisionLayer = uint.MaxValue,
            CollisionMask = uint.MaxValue,
            ContactCount = 0,
            FirstContactIndex = ContactData.Link.NullIndex,
            LastContactIndex = ContactData.Link.NullIndex
        };

        Bodies.Add(body);

        switch (bodyType)
        {
            case BodyType.Static:
                StaticBodyIndices.Add(bodyIndex.DenseIndex);
                break;
            case BodyType.Kinematic:
                KinematicBodyIndices.Add(bodyIndex.DenseIndex);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(bodyType), bodyType, null);
        }

        return ref BodiesSpan[Bodies.Count - 1];
    }

    public void DestroyBody(RigidBodyId id)
    {
        ref var body = ref GetBodyData(id);

        if (body.ColliderType is ColliderType.Tile && body.EnableCollisionDetection)
        {
            TileMap.RemoveTile(ref body);
        }

        DestroyContactsForBody(ref body);

        var denseIndex = BodyIndicesSpan[id.Index].DenseIndex;

        switch (body.Type)
        {
            case BodyType.Static:
                StaticBodyIndices.Remove(denseIndex);
                break;
            case BodyType.Kinematic:
                KinematicBodyIndices.Remove(denseIndex);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (denseIndex == Bodies.Count - 1)
        {
            // If the last element is being removed, just remove it.
            Bodies.RemoveAt(denseIndex);
        }
        else
        {
            // Otherwise swap-remove with last element.
            var oldDenseIndex = Bodies.Count - 1;
            BodiesSpan[denseIndex] = BodiesSpan[oldDenseIndex];
            Bodies.RemoveAt(oldDenseIndex);

            // Update index pointer.
            var movedBodyIndex = BodiesSpan[denseIndex].Id.Index;
            BodyIndicesSpan[movedBodyIndex].DenseIndex = denseIndex;

            // Update body type indices.
            switch (BodiesSpan[denseIndex].Type)
            {
                case BodyType.Static:
                    StaticBodyIndices.Remove(oldDenseIndex);
                    StaticBodyIndices.Add(denseIndex);
                    break;
                case BodyType.Kinematic:
                    KinematicBodyIndices.Remove(oldDenseIndex);
                    KinematicBodyIndices.Add(denseIndex);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        BodyIndicesSpan[id.Index].Version++;
        BodyIndicesSpan[id.Index].DenseIndex = BodyIndex.NullIndex;
    }

    public ref RigidBodyData GetBodyData(RigidBodyId id)
    {
        if (!id.IsValid)
        {
            throw new ArgumentException("Invalid body ID.");
        }

        var bodyIndex = BodyIndicesSpan[id.Index];
        if (bodyIndex.Version != id.Version)
        {
            throw new ArgumentException("Version mismatch detected. The ID is no longer valid.");
        }

        return ref BodiesSpan[bodyIndex.DenseIndex];
    }

    public struct BodyIndex
    {
        public const int NullIndex = -1;

        public int DenseIndex;
        public int Version;
    }

    private void DestroyContactsForBody(ref RigidBodyData body)
    {
        var contactIndex = body.FirstContactIndex;

        while (contactIndex != ContactData.Link.NullIndex)
        {
            ref var contact = ref ContactsSpan[contactIndex];
            ref var body1 = ref GetBodyData(contact.Link1.BodyId);
            ref var body2 = ref GetBodyData(contact.Link2.BodyId);

            if (contact.Link1.PrevIndex != ContactData.Link.NullIndex)
            {
                // TODO: Assert for debugging only - random crash is observed in Benchmark application when
                //       moving from one physics benchmark to another.
                Debug.Assert(contact.Link1.PrevIndex >= 0 && contact.Link1.PrevIndex < ContactsSpan.Length);
                ref var prevContact = ref ContactsSpan[contact.Link1.PrevIndex];
                if (prevContact.Link1.BodyId == contact.Link1.BodyId)
                {
                    prevContact.Link1.NextIndex = contact.Link1.NextIndex;
                }
                else
                {
                    prevContact.Link2.NextIndex = contact.Link1.NextIndex;
                }
            }

            if (contact.Link2.PrevIndex != ContactData.Link.NullIndex)
            {
                ref var prevContact = ref ContactsSpan[contact.Link2.PrevIndex];
                if (prevContact.Link1.BodyId == contact.Link2.BodyId)
                {
                    prevContact.Link1.NextIndex = contact.Link2.NextIndex;
                }
                else
                {
                    prevContact.Link2.NextIndex = contact.Link2.NextIndex;
                }
            }

            if (body1.FirstContactIndex == contactIndex)
            {
                body1.FirstContactIndex = contact.Link1.NextIndex;
            }

            if (body2.FirstContactIndex == contactIndex)
            {
                body2.FirstContactIndex = contact.Link2.NextIndex;
            }

            if (body1.LastContactIndex == contactIndex)
            {
                body1.LastContactIndex = contact.Link1.PrevIndex;
            }

            if (body2.LastContactIndex == contactIndex)
            {
                body2.LastContactIndex = contact.Link2.PrevIndex;
            }

            body1.ContactCount--;
            body2.ContactCount--;

            var nextIndex = contact.Link1.BodyId == body.Id ? contact.Link1.NextIndex : contact.Link2.NextIndex;

            if (contactIndex == Contacts.Count - 1)
            {
                // If the last element is being removed, just remove it.
                Contacts.RemoveAt(contactIndex);
            }
            else
            {
                // Otherwise swap-remove with last element.
                ContactsSpan[contactIndex] = ContactsSpan[^1];
                Contacts.RemoveAt(Contacts.Count - 1);

                // Update contact links.
                var oldIndex = Contacts.Count;
                ref var swappedContact = ref ContactsSpan[contactIndex];

                if (swappedContact.Link1.PrevIndex != ContactData.Link.NullIndex)
                {
                    ref var prevContact = ref ContactsSpan[swappedContact.Link1.PrevIndex];

                    if (prevContact.Link1.NextIndex == oldIndex)
                    {
                        prevContact.Link1.NextIndex = contactIndex;
                    }

                    if (prevContact.Link2.NextIndex == oldIndex)
                    {
                        prevContact.Link2.NextIndex = contactIndex;
                    }
                }

                if (swappedContact.Link2.PrevIndex != ContactData.Link.NullIndex)
                {
                    ref var prevContact = ref ContactsSpan[swappedContact.Link2.PrevIndex];

                    if (prevContact.Link1.NextIndex == oldIndex)
                    {
                        prevContact.Link1.NextIndex = contactIndex;
                    }

                    if (prevContact.Link2.NextIndex == oldIndex)
                    {
                        prevContact.Link2.NextIndex = contactIndex;
                    }
                }

                if (swappedContact.Link1.NextIndex != ContactData.Link.NullIndex)
                {
                    ref var nextContact = ref ContactsSpan[swappedContact.Link1.NextIndex];

                    if (nextContact.Link1.PrevIndex == oldIndex)
                    {
                        nextContact.Link1.PrevIndex = contactIndex;
                    }

                    if (nextContact.Link2.PrevIndex == oldIndex)
                    {
                        nextContact.Link2.PrevIndex = contactIndex;
                    }
                }

                if (swappedContact.Link2.NextIndex != ContactData.Link.NullIndex)
                {
                    ref var nextContact = ref ContactsSpan[swappedContact.Link2.NextIndex];

                    if (nextContact.Link1.PrevIndex == oldIndex)
                    {
                        nextContact.Link1.PrevIndex = contactIndex;
                    }

                    if (nextContact.Link2.PrevIndex == oldIndex)
                    {
                        nextContact.Link2.PrevIndex = contactIndex;
                    }
                }

                ref var swappedContactBody1 = ref GetBodyData(swappedContact.Link1.BodyId);
                ref var swappedContactBody2 = ref GetBodyData(swappedContact.Link2.BodyId);

                if (swappedContactBody1.FirstContactIndex == oldIndex)
                {
                    swappedContactBody1.FirstContactIndex = contactIndex;
                }

                if (swappedContactBody2.FirstContactIndex == oldIndex)
                {
                    swappedContactBody2.FirstContactIndex = contactIndex;
                }

                if (swappedContactBody1.LastContactIndex == oldIndex)
                {
                    swappedContactBody1.LastContactIndex = contactIndex;
                }

                if (swappedContactBody2.LastContactIndex == oldIndex)
                {
                    swappedContactBody2.LastContactIndex = contactIndex;
                }

                if (nextIndex == oldIndex)
                {
                    nextIndex = contactIndex;
                }
            }

            contactIndex = nextIndex;
        }
    }
}