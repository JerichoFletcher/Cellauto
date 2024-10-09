using JFCellauto.Structs;

namespace JFCellauto.Algorithms;

public static class Distance {
    public static int Manhattan(Vector a, Vector b) {
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }

    public static float Euclidean(Vector a, Vector b) {
        return (float)Math.Sqrt(Math.Pow(a.X - b.X, 2f) + Math.Pow(a.Y - b.Y, 2f));
    }
}
