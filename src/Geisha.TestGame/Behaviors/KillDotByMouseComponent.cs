using System.Linq;
using Geisha.Common.Math;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Physics.Components;

namespace Geisha.TestGame.Behaviors
{
    [SerializableComponent]
    public class KillDotByMouseComponent : BehaviorComponent
    {
        public override void OnFixedUpdate()
        {
            var input = Entity.GetComponent<InputComponent>();

            if (input.GetActionState("KillDot"))
            {
                var mousePosition = input.HardwareInput.MouseInput.Position;
                var dots = Entity.Scene.RootEntities
                    .Where(e => e.HasComponent<DieFromBoxComponent>())
                    .Where(d =>
                    {
                        var dotTransform = d.GetComponent<TransformComponent>();
                        var dotCollider = d.GetComponent<CircleColliderComponent>();
                        var dotCircle = new Circle(dotCollider.Radius).Transform(dotTransform.Create2DTransformationMatrix());
                        var mouseCircle = new Circle(mousePosition.WithY(-mousePosition.Y), 0.1);
                        return dotCircle.Overlaps(mouseCircle);
                    });

                foreach (var dot in dots)
                {
                    dot.Destroy();
                }
            }
        }
    }
}