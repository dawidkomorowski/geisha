using System.Linq;

namespace Geisha.Engine.Core.SceneModel
{
    /// <summary>
    ///     Provides functionality to map between <see cref="Entity" /> and <see cref="EntityDefinition" /> in both directions,
    /// </summary>
    internal class EntityDefinitionMapper
    {
        public EntityDefinition ToDefinition(Entity entity)
        {
            return new EntityDefinition
            {
                Children = entity.Children.Select(ToDefinition).ToList()
            };
        }

        public Entity FromDefinition(EntityDefinition entityDefinition)
        {
            var entity = new Entity();
            foreach (var definition in entityDefinition.Children)
            {
                entity.AddChild(FromDefinition(definition));
            }

            return entity;
        }
    }
}