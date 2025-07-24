namespace Geisha.Extensions.Tiled;

public readonly record struct GlobalTileId
{
    private const uint FlippedHorizontallyFlag = 0x80000000;
    private const uint FlippedVerticallyFlag = 0x40000000;
    private const uint FlippedDiagonallyFlag = 0x20000000;
    private const uint RotatedHexagonal120Flag = 0x10000000;

    public static GlobalTileId Invalid { get; } = new(0);

    public GlobalTileId(uint value)
    {
        Value = value;
    }

    public uint Value { get; }

    public bool FlippedHorizontally => (Value & FlippedHorizontallyFlag) != 0;
    public bool FlippedVertically => (Value & FlippedVerticallyFlag) != 0;
    public bool FlippedDiagonally => (Value & FlippedDiagonallyFlag) != 0;
    public bool RotatedHexagonal120 => (Value & RotatedHexagonal120Flag) != 0;
    public bool HasFlippingFlags => FlippedHorizontally || FlippedVertically || FlippedDiagonally || RotatedHexagonal120;

    public GlobalTileId ClearFlippingFlags()
    {
        return new GlobalTileId(Value & ~(FlippedHorizontallyFlag | FlippedVerticallyFlag | FlippedDiagonallyFlag | RotatedHexagonal120Flag));
    }
}