using System.ComponentModel.Composition;
using System.IO;
using BallEscape.Behaviors;
using Geisha.Common.Geometry;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Rendering.Components;
using Geisha.Framework.Rendering;

namespace BallEscape
{
    [Export]
    public class PrefabFactory
    {
        private readonly IRenderer2D _renderer2D;

        [ImportingConstructor]
        public PrefabFactory(IRenderer2D renderer2D)
        {
            _renderer2D = renderer2D;
        }

        public Entity CreateWorld()
        {
            var entity = new Entity();

            entity.AddComponent(new Transform {Translation = Vector3.One, Rotation = Vector3.Zero, Scale = Vector3.One});

            var texture = LoadTexture("World.bmp");
            entity.AddComponent(new SpriteRenderer {Sprite = CreateSpriteFromTexture(texture), SortingLayerName = "Background"});

            return entity;
        }

        public Entity CreatePlayer()
        {
            var entity = new Entity {Name = "Player"};

            entity.AddComponent(new Transform {Translation = Vector3.Zero, Rotation = Vector3.Zero, Scale = Vector3.One});

            var texture = LoadTexture("Player.png");
            entity.AddComponent(new SpriteRenderer {Sprite = CreateSpriteFromTexture(texture), SortingLayerName = "Dynamic"});

            var inputMapping = PlayerInputMapping.CreateInputMapping();
            entity.AddComponent(new InputComponent {InputMapping = inputMapping});

            entity.AddComponent(new Movement {Speed = 5});
            entity.AddComponent(new SetUpPlayerInput());
            entity.AddComponent(new RespectWorldBoundary());

            return entity;
        }

        public Entity CreateEnemy()
        {
            var entity = new Entity {Name = "Enemy"};

            entity.AddComponent(new Transform {Translation = Vector3.Zero, Rotation = Vector3.Zero, Scale = Vector3.One});

            var texture = LoadTexture("Enemy.png");
            entity.AddComponent(new SpriteRenderer {Sprite = CreateSpriteFromTexture(texture), SortingLayerName = "Dynamic"});

            entity.AddComponent(new Movement {Speed = 2});
            entity.AddComponent(new RespectWorldBoundary());
            entity.AddComponent(new FollowPlayer());
            entity.AddComponent(new KillEntityWithName("Player"));

            return entity;
        }

        public Entity CreateHole(int xPos, int yPos)
        {
            var entity = new Entity();

            entity.AddComponent(new Transform {Translation = new Vector3(xPos, yPos, 0), Rotation = Vector3.Zero, Scale = Vector3.One});

            var texture = LoadTexture("Hole.png");
            entity.AddComponent(new SpriteRenderer {Sprite = CreateSpriteFromTexture(texture), SortingLayerName = "Static"});

            entity.AddComponent(new KillEntityWithName("Player", "Enemy"));

            return entity;
        }

        public Entity CreateEnemySpawnPoint(int xPos, int yPos)
        {
            var entity = new Entity();

            entity.AddComponent(new Transform {Translation = new Vector3(xPos, yPos, 0), Rotation = Vector3.Zero, Scale = Vector3.One});

            var texture = LoadTexture("EnemySpawnPoint.png");
            entity.AddComponent(new SpriteRenderer {Sprite = CreateSpriteFromTexture(texture), SortingLayerName = "Static"});

            entity.AddComponent(new SpawnEnemy(this));

            return entity;
        }

        private Sprite CreateSpriteFromTexture(ITexture texture)
        {
            return new Sprite
            {
                PixelsPerUnit = 1,
                SourceTexture = texture,
                SourceAnchor = texture.Dimension / 2,
                SourceDimension = texture.Dimension,
                SourceUV = Vector2.Zero
            };
        }

        private ITexture LoadTexture(string textureName)
        {
            var filePath = Path.Combine("Assets", textureName);
            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                return _renderer2D.CreateTexture(fileStream);
            }
        }
    }
}