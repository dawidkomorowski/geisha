using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Core.Spatial;

// TODO: Scaffolding for spatial grid prototyping.
public sealed class SpatialGrid
{
    public SpatialGrid(double cellSize) : this(new SizeD(cellSize, cellSize))
    {
    }

    public SpatialGrid(SizeD cellSize)
    {
        CellSize = cellSize;
    }

    public SizeD CellSize { get; }
}