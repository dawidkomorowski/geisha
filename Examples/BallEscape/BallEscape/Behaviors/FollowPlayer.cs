using System.Linq;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;

namespace BallEscape.Behaviors
{
    public class FollowPlayer : Behavior
    {
        private Entity _player;

        public override void OnStart()
        {
            _player = Entity.Scene.RootEntities.SingleOrDefault(e => e.Name == "Player");
        }

        public override void OnFixedUpdate()
        {
            if (_player == null) return;

            var myPosition = Entity.GetComponent<Transform>().Translation;
            var playerPosition = _player.GetComponent<Transform>().Translation;
            var directionToPlayer = (playerPosition - myPosition).ToVector2();

            var movement = Entity.GetComponent<Movement>();
            movement.AddMovementInput(directionToPlayer);
        }
    }
}