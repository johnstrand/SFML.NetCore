using System;
using System.Numerics;

namespace SFML.Graphics;

////////////////////////////////////////////////////////////
/// <summary>
/// Decomposed transform defined by a position, a rotation and a scale
/// </summary>
/// <remarks>
/// A note on coordinates and undistorted rendering:
/// By default, SFML (or more exactly, OpenGL) may interpolate drawable objects
/// such as sprites or texts when rendering. While this allows transitions
/// like slow movements or rotations to appear smoothly, it can lead to
/// unwanted results in some cases, for example blurred or distorted objects.
/// In order to render a SFML.Graphics.Drawable object pixel-perfectly, make sure
/// the involved coordinates allow a 1:1 mapping of pixels in the window
/// to texels (pixels in the texture). More specifically, this means:
/// * The object's position, origin and scale have no fractional part
/// * The object's and the view's rotation are a multiple of 90 degrees
/// * The view's center and size have no fractional part
/// </remarks>
////////////////////////////////////////////////////////////
public class Transformable : ObjectBase
{
    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Default constructor
    /// </summary>
    ////////////////////////////////////////////////////////////
    public Transformable() :
        base(IntPtr.Zero)
    {
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Construct the transformable from another transformable
    /// </summary>
    /// <param name="transformable">Transformable to copy</param>
    ////////////////////////////////////////////////////////////
    public Transformable(Transformable transformable) :
        base(IntPtr.Zero)
    {
        Origin = transformable.Origin;
        Position = transformable.Position;
        Rotation = transformable.Rotation;
        Scale = transformable.Scale;
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Position of the object
    /// </summary>
    ////////////////////////////////////////////////////////////
    public Vector2 Position
    {
        get
        {
            return _myPosition;
        }
        set
        {
            _myPosition = value;
            _myTransformNeedUpdate = true;
            _myInverseNeedUpdate = true;
        }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Rotation of the object
    /// </summary>
    ////////////////////////////////////////////////////////////
    public float Rotation
    {
        get
        {
            return _myRotation;
        }
        set
        {
            _myRotation = value;
            _myTransformNeedUpdate = true;
            _myInverseNeedUpdate = true;
        }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Scale of the object
    /// </summary>
    ////////////////////////////////////////////////////////////
    public Vector2 Scale
    {
        get
        {
            return _myScale;
        }
        set
        {
            _myScale = value;
            _myTransformNeedUpdate = true;
            _myInverseNeedUpdate = true;
        }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// The origin of an object defines the center point for
    /// all transformations (position, scale, rotation).
    /// The coordinates of this point must be relative to the
    /// top-left corner of the object, and ignore all
    /// transformations (position, scale, rotation).
    /// </summary>
    ////////////////////////////////////////////////////////////
    public Vector2 Origin
    {
        get
        {
            return _myOrigin;
        }
        set
        {
            _myOrigin = value;
            _myTransformNeedUpdate = true;
            _myInverseNeedUpdate = true;
        }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// The combined transform of the object
    /// </summary>
    ////////////////////////////////////////////////////////////
    public Transform Transform
    {
        get
        {
            if (_myTransformNeedUpdate)
            {
                _myTransformNeedUpdate = false;

                var angle = -_myRotation * 3.141592654F / 180.0F;
                var (sine, cosine) = MathF.SinCos(angle);
                var sxc = _myScale.X * cosine;
                var syc = _myScale.Y * cosine;
                var sxs = _myScale.X * sine;
                var sys = _myScale.Y * sine;
                var tx = (-_myOrigin.X * sxc) - (_myOrigin.Y * sys) + _myPosition.X;
                var ty = (_myOrigin.X * sxs) - (_myOrigin.Y * syc) + _myPosition.Y;

                _myTransform = new Transform(sxc, sys, tx,
                                            -sxs, syc, ty,
                                            0.0F, 0.0F, 1.0F);
            }
            return _myTransform;
        }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// The combined transform of the object
    /// </summary>
    ////////////////////////////////////////////////////////////
    public Transform InverseTransform
    {
        get
        {
            if (_myInverseNeedUpdate)
            {
                _myInverseTransform = Transform.GetInverse();
                _myInverseNeedUpdate = false;
            }
            return _myInverseTransform;
        }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Construct the object from its internal C pointer
    /// </summary>
    /// <param name="cPointer">Pointer to the object in the C library</param>
    ////////////////////////////////////////////////////////////
    protected Transformable(IntPtr cPointer) :
        base(cPointer)
    {
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Handle the destruction of the object
    /// </summary>
    /// <param name="disposing">Is the GC disposing the object, or is it an explicit call ?</param>
    ////////////////////////////////////////////////////////////
    protected override void Destroy(bool disposing)
    {
        // Does nothing, this instance is either pure C# (if created by the user)
        // or not the final object (if used as a base for a drawable class)
    }

    private Vector2 _myOrigin = new(0, 0);
    private Vector2 _myPosition = new(0, 0);
    private float _myRotation;
    private Vector2 _myScale = new(1, 1);
    private Transform _myTransform;
    private Transform _myInverseTransform;
    private bool _myTransformNeedUpdate = true;
    private bool _myInverseNeedUpdate = true;
}
