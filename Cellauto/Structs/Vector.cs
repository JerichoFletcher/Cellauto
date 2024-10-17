using System.Diagnostics.CodeAnalysis;

namespace JerichoFletcher.Cellauto.Structs;

/// <summary>
/// A data structure that may represent a two-dimensional position or direction.
/// </summary>
/// <param name="x">The component of the vector on the X-axis.</param>
/// <param name="y">The component of the vector on the Y-axis.</param>
public struct Vector(int x, int y) {
    /// <summary>The X-component of the vector.</summary>
    public int X { get; set; } = x;
    /// <summary>The Y-component of the vector.</summary>
    public int Y { get; set; } = y;

    public override readonly bool Equals([NotNullWhen(true)] object? obj) {
        return obj != null
            && obj is Vector other
            && X == other.X
            && Y == other.Y;
    }

    public override readonly int GetHashCode() {
        var x = X.GetHashCode();
        var y = Y.GetHashCode();
        return X.GetHashCode() ^ y << 1 ^ y >> 31;
    }

    public static bool operator ==(Vector left, Vector right) {
        return left.Equals(right);
    }

    public static bool operator !=(Vector left, Vector right) {
        return !(left == right);
    }
}
