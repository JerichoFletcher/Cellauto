using JerichoFletcher.Cellauto.Structs;

namespace JerichoFletcher.Cellauto.Algorithms;

/// <summary>
/// Provides common distance functions that may be applied for various purposes.
/// </summary>
public static class Distance {
    /// <summary>
    /// Computes the Manhattan (or taxicab) distance between two grid-space points.
    /// The Manhattan distance between the points <paramref name="a"/> and <paramref name="b"/> is defined as the sum of
    /// the absolute differences between the points in each of their corresponding coordinate components.
    /// </summary>
    /// <param name="a">The first point.</param>
    /// <param name="b">The second point.</param>
    /// <returns>The Manhattan distance between <paramref name="a"/> and <paramref name="b"/>.</returns>
    public static int Manhattan(Vector a, Vector b) {
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }

    /// <summary>
    /// Computes the Euclidean distance between two grid-space points.
    /// The Euclidean distance between the points <paramref name="a"/> and <paramref name="b"/> is defined as the square
    /// root of the sum of squared differences between the points in each of their corresponding coordinate components.
    /// </summary>
    /// <param name="a">The first point.</param>
    /// <param name="b">The second point.</param>
    /// <returns>The Euclidean distance between <paramref name="a"/> and <paramref name="b"/>.</returns>
    public static float Euclidean(Vector a, Vector b) {
        return (float)Math.Sqrt(Math.Pow(a.X - b.X, 2f) + Math.Pow(a.Y - b.Y, 2f));
    }
}
