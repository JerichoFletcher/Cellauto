namespace JFCellauto.Structs;

public class Cell<T>(Vector coord, T value) where T : struct, Enum {
    public Vector Coord { get; set; } = coord;
    public T Value { get; set; } = value;
}
