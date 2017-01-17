namespace Geisha.Common.Geometry.Shape
{
    public interface IShape<TShape>
    {
        TShape Transform(Matrix3 transform);
    }
}