using System;

namespace SFML.Graphics;

////////////////////////////////////////////////////////////
/// <summary>
/// <para>Types of primitives that a VertexArray can render.</para>
/// <para>
/// Points and lines have no area, therefore their thickness
/// will always be 1 pixel, regardless the current transform
/// and view.
/// </para>
/// </summary>
////////////////////////////////////////////////////////////
public enum PrimitiveType
{
    /// <summary>
    /// List of individual points
    /// </summary>
    Points,

    /// <summary>
    /// List of individual lines
    /// </summary>
    Lines,

    /// <summary>
    /// List of connected lines, a point uses the previous point to form a line
    /// </summary>
    LineStrip,

    /// <summary>
    /// List of connected lines, a point uses the previous point to form a line
    /// </summary>
    [Obsolete("LinesStrip is deprecated, please use LineStrip")]
    LinesStrip = LineStrip,

    /// <summary>
    /// List of individual triangles
    /// </summary>
    Triangles,

    /// <summary>
    /// List of connected triangles, a point uses the two previous points to form a triangle
    /// </summary>
    TriangleStrip,

    /// <summary>
    /// List of connected triangles, a point uses the two previous points to form a triangle
    /// </summary>
    [Obsolete("TrianglesStrip is deprecated, please use TriangleStrip")]
    TrianglesStrip = TriangleStrip,

    /// <summary>
    /// List of connected triangles, a point uses the common center and the previous point to form a triangle
    /// </summary>
    TriangleFan,

    /// <summary>
    /// List of connected triangles, a point uses the common center and the previous point to form a triangle
    /// </summary>
    [Obsolete("TrianglesFan is deprecated, please use TriangleFan")]
    TrianglesFan = TriangleFan,

    /// <summary>
    /// List of individual quads
    /// </summary>
    Quads,
}
