using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.Memory;

namespace Geisha.Engine.Physics.PhysicsEngine2D.Internal;

// TODO: How to make it thread safe?
internal struct PhysicsSceneData
{
    private static readonly PhysicsSceneData[] Scenes = new PhysicsSceneData[32];
    private static int _firstFreeIndex = 0;
    private const int NoFreeIndex = -1;

    static PhysicsSceneData()
    {
        for (var i = 0; i < Scenes.Length; i++)
        {
            Scenes[i]._index = i;
            Scenes[i]._nextFreeIndex = i + 1;
        }

        Scenes[^1]._nextFreeIndex = NoFreeIndex;
    }

    public static PhysicsSceneId Create(in PhysicsScene2DDefinition sceneDefinition)
    {
        if (_firstFreeIndex == NoFreeIndex)
        {
            throw new InvalidOperationException("There are no available free slots to allocate new Physics Scene.");
        }

        ref var scene = ref Scenes[_firstFreeIndex];

        Debug.Assert(scene._index == _firstFreeIndex, "Corrupted scene index.");

        scene._version++;
        scene.SimulationParameters = new SimulationParameters
        {
            Substeps = sceneDefinition.Substeps > 0 ? sceneDefinition.Substeps : 1,
            VelocityIterations = sceneDefinition.VelocityIterations > 0 ? sceneDefinition.VelocityIterations : 4,
            PositionIterations = sceneDefinition.PositionIterations > 0 ? sceneDefinition.PositionIterations : 4,
            PenetrationTolerance = sceneDefinition.PenetrationTolerance >= 0 ? sceneDefinition.PenetrationTolerance : 0.01
        };
        scene.TileSize = sceneDefinition.TileSize;
        scene.TileMap = new TileMap(sceneDefinition.TileSize);
        scene._firstFreeBodyIndex = NoFreeIndex;
        scene._bodyIndices = new List<BodyIndex>();
        scene._staticBodyCount = 0;
        scene._kinematicBodyCount = 0;
        scene._bodies = new List<RigidBodyData>();
        scene.Contacts = new List<ContactData>(256);
        scene.SensorOverlapCache = new SensorOverlapCache(256);
        scene.SensorOverlapEvents = new List<SensorOverlapEvent>(256);

        _firstFreeIndex = scene._nextFreeIndex;

        return scene.PhysicsSceneId;
    }

    public static void Destroy(PhysicsSceneId id)
    {
        ref var scene = ref Get(id);

        var index = scene._index;
        var version = scene._version;
        scene = default;
        scene._index = index;
        scene._version = version + 1;
        scene._nextFreeIndex = _firstFreeIndex;

        _firstFreeIndex = index;
    }

    public static ref PhysicsSceneData Get(PhysicsSceneId id)
    {
        if (!IsValid(id))
        {
            throw new ArgumentException("Invalid Physics Scene ID.");
        }

        return ref Scenes[id.Index];
    }

    public static bool IsValid(PhysicsSceneId id) => id.IsValid && Scenes[id.Index]._version == id.Version;

    // Indexing and versioning
    private PhysicsSceneId PhysicsSceneId => new(_index, _version);

    private int _index;
    private int _version;
    private int _nextFreeIndex;

    // Simulation parameters
    public SimulationParameters SimulationParameters;

    // Tile map
    public SizeD TileSize;
    public TileMap TileMap;

    // Sparse body array
    private struct BodyIndex : IUnmanaged<BodyIndex>
    {
        public const int NullIndex = -1;

        public int DenseIndex;
        public int Version;
        public int NextFreeIndex;
    }

    private int _firstFreeBodyIndex;
    private List<BodyIndex> _bodyIndices;
    private readonly Span<BodyIndex> GetBodyIndicesSpan() => CollectionsMarshal.AsSpan(_bodyIndices);

    // Dense body array
    private int _staticBodyCount;
    private int _kinematicBodyCount;
    private List<RigidBodyData> _bodies;
    public readonly Span<RigidBodyData> GetBodiesSpan() => CollectionsMarshal.AsSpan(_bodies);
    public readonly Span<RigidBodyData> GetStaticBodiesSpan() => GetBodiesSpan().Slice(0, _staticBodyCount);
    public readonly Span<RigidBodyData> GetKinematicBodiesSpan() => GetBodiesSpan().Slice(_staticBodyCount, _kinematicBodyCount);

    // Contacts
    public List<ContactData> Contacts;
    public readonly Span<ContactData> GetContactsSpan() => CollectionsMarshal.AsSpan(Contacts);

    // Sensors
    public SensorOverlapCache SensorOverlapCache;
    public List<SensorOverlapEvent> SensorOverlapEvents;

    public ref RigidBodyData CreateBody(BodyType bodyType)
    {
        int sparseIndex;
        var bodyIndicesSpan = GetBodyIndicesSpan();

        if (_firstFreeBodyIndex == NoFreeIndex)
        {
            sparseIndex = _bodyIndices.Count;
            _bodyIndices.Add(default);
            bodyIndicesSpan = GetBodyIndicesSpan();
            bodyIndicesSpan[sparseIndex].NextFreeIndex = NoFreeIndex;
        }
        else
        {
            sparseIndex = _firstFreeBodyIndex;
            _firstFreeBodyIndex = bodyIndicesSpan[sparseIndex].NextFreeIndex;
        }

        ref var bodyIndex = ref bodyIndicesSpan[sparseIndex];
        bodyIndex.DenseIndex = _bodies.Count;
        bodyIndex.Version++;

        var rigidBodyId = new RigidBodyId(PhysicsSceneId, sparseIndex, bodyIndex.Version);

        var body = new RigidBodyData
        {
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

        _bodies.Add(body);

        switch (bodyType)
        {
            case BodyType.Static:
                if (_kinematicBodyCount > 0)
                {
                    // Swap bodies to keep layout: all static, then, all kinematic.
                    SwapBodies(_staticBodyCount, bodyIndex.DenseIndex);
                }

                _staticBodyCount++;
                break;
            case BodyType.Kinematic:
                _kinematicBodyCount++;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(bodyType), bodyType, null);
        }

        Debug.Assert(BodiesLayoutIsValid(), "Invalid bodies layout.");

        var bodiesSpan = GetBodiesSpan();
        return ref bodiesSpan[bodyIndex.DenseIndex];
    }

    public void DestroyBody(RigidBodyId id)
    {
        ref var body = ref GetBodyData(id);

        if (body.ColliderType is ColliderType.Tile && body.EnableCollisionDetection)
        {
            TileMap.RemoveTile(ref body);
        }

        DestroyContactsForBody(ref body);

        var bodyIndicesSpan = GetBodyIndicesSpan();
        var bodiesSpan = GetBodiesSpan();

        var denseIndex = bodyIndicesSpan[id.Index].DenseIndex;

        switch (body.Type)
        {
            case BodyType.Static:
                _staticBodyCount--;

                if (_kinematicBodyCount > 0 && denseIndex < _staticBodyCount)
                {
                    SwapBodies(_staticBodyCount, denseIndex);
                    denseIndex = _staticBodyCount;
                    body = ref bodiesSpan[denseIndex];
                }

                break;
            case BodyType.Kinematic:
                _kinematicBodyCount--;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (denseIndex == _bodies.Count - 1)
        {
            // If the last element is being removed, just remove it.
            _bodies.RemoveAt(denseIndex);
        }
        else
        {
            // Otherwise swap-remove with last element.
            var oldDenseIndex = _bodies.Count - 1;
            bodiesSpan[denseIndex] = bodiesSpan[oldDenseIndex];
            _bodies.RemoveAt(oldDenseIndex);
            bodiesSpan = GetBodiesSpan();

            // Update index pointer.
            var movedBodyIndex = bodiesSpan[denseIndex].Id.Index;
            bodyIndicesSpan[movedBodyIndex].DenseIndex = denseIndex;
        }

        bodyIndicesSpan[id.Index].Version++;
        bodyIndicesSpan[id.Index].DenseIndex = BodyIndex.NullIndex;
        bodyIndicesSpan[id.Index].NextFreeIndex = _firstFreeBodyIndex;
        _firstFreeBodyIndex = id.Index;

        Debug.Assert(BodiesLayoutIsValid(), "Invalid bodies layout.");
    }

    public readonly ref RigidBodyData GetBodyData(RigidBodyId id)
    {
        if (!IsValidBodyId(id))
        {
            throw new ArgumentException("Invalid Rigid Body ID.");
        }

        var bodyIndicesSpan = GetBodyIndicesSpan();
        var bodiesSpan = GetBodiesSpan();

        ref var bodyIndex = ref bodyIndicesSpan[id.Index];
        return ref bodiesSpan[bodyIndex.DenseIndex];
    }

    public readonly bool IsValidBodyId(RigidBodyId id) => id.IsValid && GetBodyIndicesSpan()[id.Index].Version == id.Version;

    private void SwapBodies(int index1, int index2)
    {
        var bodyIndicesSpan = GetBodyIndicesSpan();
        var bodiesSpan = GetBodiesSpan();

        (bodiesSpan[index1], bodiesSpan[index2]) = (bodiesSpan[index2], bodiesSpan[index1]);
        bodyIndicesSpan[bodiesSpan[index1].Id.Index].DenseIndex = index1;
        bodyIndicesSpan[bodiesSpan[index2].Id.Index].DenseIndex = index2;
    }

    private void DestroyContactsForBody(ref RigidBodyData body)
    {
        while (body.FirstContactIndex != ContactData.Link.NullIndex)
        {
            var contactIndex = body.FirstContactIndex;
            var contactsSpan = GetContactsSpan();
            ref var contact = ref contactsSpan[contactIndex];
            ref var body1 = ref GetBodyData(contact.Link1.BodyId);
            ref var body2 = ref GetBodyData(contact.Link2.BodyId);

            if (contact.Link1.PrevIndex != ContactData.Link.NullIndex)
            {
                ref var prevContact = ref contactsSpan[contact.Link1.PrevIndex];
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
                ref var prevContact = ref contactsSpan[contact.Link2.PrevIndex];
                if (prevContact.Link1.BodyId == contact.Link2.BodyId)
                {
                    prevContact.Link1.NextIndex = contact.Link2.NextIndex;
                }
                else
                {
                    prevContact.Link2.NextIndex = contact.Link2.NextIndex;
                }
            }

            if (contact.Link1.NextIndex != ContactData.Link.NullIndex)
            {
                ref var nextContact = ref contactsSpan[contact.Link1.NextIndex];
                if (nextContact.Link1.BodyId == contact.Link1.BodyId)
                {
                    nextContact.Link1.PrevIndex = contact.Link1.PrevIndex;
                }
                else
                {
                    nextContact.Link2.PrevIndex = contact.Link1.PrevIndex;
                }
            }

            if (contact.Link2.NextIndex != ContactData.Link.NullIndex)
            {
                ref var nextContact = ref contactsSpan[contact.Link2.NextIndex];
                if (nextContact.Link1.BodyId == contact.Link2.BodyId)
                {
                    nextContact.Link1.PrevIndex = contact.Link2.PrevIndex;
                }
                else
                {
                    nextContact.Link2.PrevIndex = contact.Link2.PrevIndex;
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

            if (contactIndex == Contacts.Count - 1)
            {
                // If the last element is being removed, just remove it.
                Contacts.RemoveAt(contactIndex);
            }
            else
            {
                // Otherwise swap-remove with last element.
                contactsSpan[contactIndex] = contactsSpan[^1];
                Contacts.RemoveAt(Contacts.Count - 1);
                contactsSpan = GetContactsSpan();

                // Update contact links.
                var oldIndex = Contacts.Count;
                ref var swappedContact = ref contactsSpan[contactIndex];

                if (swappedContact.Link1.PrevIndex != ContactData.Link.NullIndex)
                {
                    ref var prevContact = ref contactsSpan[swappedContact.Link1.PrevIndex];

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
                    ref var prevContact = ref contactsSpan[swappedContact.Link2.PrevIndex];

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
                    ref var nextContact = ref contactsSpan[swappedContact.Link1.NextIndex];

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
                    ref var nextContact = ref contactsSpan[swappedContact.Link2.NextIndex];

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
            }
        }
    }

    private readonly bool BodiesLayoutIsValid()
    {
        var allowedType = BodyType.Static;
        foreach (ref var body in GetBodiesSpan())
        {
            if (body.Type == allowedType) continue;
            if (body.Type is BodyType.Kinematic && allowedType is BodyType.Static)
            {
                allowedType = BodyType.Kinematic;
                continue;
            }

            return false;
        }

        return true;
    }
}