using System.Numerics;

namespace SFML.Graphics;

////////////////////////////////////////////////////////////
/// <summary>
/// Specialized shape representing a rectangle
/// </summary>
////////////////////////////////////////////////////////////
public class RectangleShape : Shape
{
    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Default constructor
    /// </summary>
    ////////////////////////////////////////////////////////////
    public RectangleShape() :
        this(new Vector2(0, 0))
    {
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Construct the shape with an initial size
    /// </summary>
    /// <param name="size">Size of the shape</param>
    ////////////////////////////////////////////////////////////
    public RectangleShape(Vector2 size)
    {
        Size = size;
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Construct the shape from another shape
    /// </summary>
    /// <param name="copy">Shape to copy</param>
    ////////////////////////////////////////////////////////////
    public RectangleShape(RectangleShape copy) :
        base(copy)
    {
        Size = copy.Size;
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// The size of the rectangle
    /// </summary>
    ////////////////////////////////////////////////////////////
    public Vector2 Size
    {
        get { return _mySize; }
        set { _mySize = value; Update(); }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Get the total number of points of the rectangle.
    /// </summary>
    /// <returns>The total point count. For rectangle shapes,
    /// this number is always 4.</returns>
    ////////////////////////////////////////////////////////////
    public override uint GetPointCount() => 4;

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>Get the position of a point</para>
    /// <para>
    /// The returned point is in local coordinates, that is,
    /// the shape's transforms (position, rotation, scale) are
    /// not taken into account.
    /// The result is undefined if index is out of the valid range.
    /// </para>
    /// </summary>
    /// <param name="index">Index of the point to get, in range [0 .. 3]</param>
    /// <returns>index-th point of the shape</returns>
    ////////////////////////////////////////////////////////////
    public override Vector2 GetPoint(uint index)
    {
        return index switch
        {
            1 => new Vector2(_mySize.X, 0),
            2 => new Vector2(_mySize.X, _mySize.Y),
            3 => new Vector2(0, _mySize.Y),
            _ => new Vector2(0, 0),
        };
    }

    private Vector2 _mySize;
}
