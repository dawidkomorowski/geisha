using System;

namespace Geisha.Extensions.Tiled;

public sealed class InvalidTiledMapException : InvalidOperationException
{
    public InvalidTiledMapException(string message) : base($"Invalid Tiled map XML format: {message}.")
    {
    }
}