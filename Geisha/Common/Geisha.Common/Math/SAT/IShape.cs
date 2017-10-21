namespace Geisha.Common.Math.SAT
{
    public interface IShape
    {
        bool IsCircle { get; }
        Vector2 Center { get; }
        double Radius { get; }

        Axis[] GetAxes();
        Vector2[] GetVertices();
    }
}