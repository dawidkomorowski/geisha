namespace Geisha.Engine.Core.GameLoop;

internal interface ITransformInterpolationGameLoopStep
{
    void SnapshotTransforms();
    void InterpolateTransforms(double interpolationFactor);
}