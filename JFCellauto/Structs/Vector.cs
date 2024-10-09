using System.Diagnostics.CodeAnalysis;

namespace JFCellauto.Structs;

public struct Vector(int x, int y) {
    public int Y { get; set; } = y;
    public int X { get; set; } = x;

    public override bool Equals([NotNullWhen(true)] object? obj) {
        return obj != null
            && obj is Vector other
            && X == other.X
            && Y == other.Y;
    }

    public override int GetHashCode() {
        var x = X.GetHashCode();
        var y = Y.GetHashCode();
        return X.GetHashCode() ^ (y << 1) ^ (y >> 31);
    }

    public static bool operator ==(Vector left, Vector right) {
        return left.Equals(right);
    }

    public static bool operator !=(Vector left, Vector right) {
        return !(left == right);
    }
}
