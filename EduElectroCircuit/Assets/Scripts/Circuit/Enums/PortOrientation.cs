/// <summary>
/// Describes the relative orientation of two ports based on their direction and position.
/// </summary>
public enum PortOrientation
{
    /// <summary>
    /// Ports have collinear direction vectors and are aligned.
    /// </summary>
    INLINE,
    /// <summary>
    /// Ports have collinear direction vectors and are not aligned.
    /// </summary>
    Z_LINE,
    /// <summary>
    /// Ports have non-collinear direction vectors, forming an angle.
    /// </summary>
    CORNER
}
