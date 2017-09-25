namespace Geisha.Common.Math.Shape
{
    public interface IShape<TShape>
    {
        TShape Transform(Matrix3 transform);
    }
}