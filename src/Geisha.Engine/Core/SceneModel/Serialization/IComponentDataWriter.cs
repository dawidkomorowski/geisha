﻿namespace Geisha.Engine.Core.SceneModel.Serialization
{
    public interface IComponentDataWriter
    {
        void WriteIntProperty(string propertyName, int value);
        void WriteDoubleProperty(string propertyName, double value);
        void WriteStringProperty(string propertyName, string? value);
    }
}