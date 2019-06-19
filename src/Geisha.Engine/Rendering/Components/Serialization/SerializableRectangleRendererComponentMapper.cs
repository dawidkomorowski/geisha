﻿using Geisha.Common.Math.Serialization;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Framework.Rendering;

namespace Geisha.Engine.Rendering.Components.Serialization
{
    internal sealed class SerializableRectangleRendererComponentMapper
        : SerializableComponentMapperAdapter<RectangleRendererComponent, SerializableRectangleRendererComponent>
    {
        protected override SerializableRectangleRendererComponent MapToSerializable(RectangleRendererComponent component)
        {
            return new SerializableRectangleRendererComponent
            {
                Visible = component.Visible,
                SortingLayerName = component.SortingLayerName,
                OrderInLayer = component.OrderInLayer,
                Dimension = SerializableVector2.FromVector2(component.Dimension),
                ColorArgb = component.Color.ToArgb(),
                FillInterior = component.FillInterior
            };
        }

        protected override RectangleRendererComponent MapFromSerializable(SerializableRectangleRendererComponent serializableComponent)
        {
            return new RectangleRendererComponent
            {
                Visible = serializableComponent.Visible,
                SortingLayerName = serializableComponent.SortingLayerName,
                OrderInLayer = serializableComponent.OrderInLayer,
                Dimension = SerializableVector2.ToVector2(serializableComponent.Dimension),
                Color = Color.FromArgb(serializableComponent.ColorArgb),
                FillInterior = serializableComponent.FillInterior
            };
        }
    }
}