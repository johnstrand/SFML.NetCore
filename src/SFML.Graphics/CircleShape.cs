using System;
using System.Numerics;

using SFML.System;

namespace SFML.Graphics;

////////////////////////////////////////////////////////////
/// <summary>
/// Specialized shape representing a circle
/// </summary>
////////////////////////////////////////////////////////////
public class CircleShape : Shape
{
    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Default constructor
    /// </summary>
    ////////////////////////////////////////////////////////////
    public CircleShape() : this(0) { }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Construct the shape with an initial radius
    /// </summary>
    /// <param name="radius">Radius of the shape</param>
    ////////////////////////////////////////////////////////////
    public CircleShape(float radius) : this(radius, 30) { }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Construct the shape with an initial radius and point count
    /// </summary>
    /// <param name="radius">Radius of the shape</param>
    /// <param name="pointCount">Number of points of the shape</param>
    ////////////////////////////////////////////////////////////
    public CircleShape(float radius, uint pointCount)
    {
        Radius = radius;
        SetPointCount(pointCount);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Construct the shape from another shape
    /// </summary>
    /// <param name="copy">Shape to copy</param>
    ////////////////////////////////////////////////////////////
    public CircleShape(CircleShape copy) : base(copy)
    {
        Radius = copy.Radius;
        SetPointCount(copy.GetPointCount());
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// The radius of the shape
    /// </summary>
    ////////////////////////////////////////////////////////////
    public float Radius
    {
        get { return _myRadius; }
        set { _myRadius = value; Update(); }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Get the total number of points of the circle
    /// </summary>
    /// <returns>The total point count</returns>
    ////////////////////////////////////////////////////////////
    public override uint GetPointCount() => _myPointCount;

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Set the number of points of the circle.
    /// The count must be greater than 2 to define a valid shape.
    /// </summary>
    /// <param name="count">New number of points of the circle</param>
    ////////////////////////////////////////////////////////////
    public void SetPointCount(uint count)
    {
        _myPointCount = count;
        Update();
    }

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
    /// <param name="index">Index of the point to get, in range [0 .. PointCount - 1]</param>
    /// <returns>index-th point of the shape</returns>
    ////////////////////////////////////////////////////////////
    public override Vector2 GetPoint(uint index)
    {
        var angle = (index * 2 * MathF.PI / _myPointCount) - (MathF.PI / 2);
        var (y, x) = MathF.SinCos(angle).Mult(_myRadius);

        return new(_myRadius + x, _myRadius + y);
    }

    private float _myRadius;
    private uint _myPointCount;
}
