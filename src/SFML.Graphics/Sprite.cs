using System;
using System.Runtime.InteropServices;
using System.Security;

using SFML.System;

namespace SFML.Graphics;

////////////////////////////////////////////////////////////
/// <summary>
/// This class defines a sprite : texture, transformations,
/// color, and draw on screen
/// </summary>
/// <remarks>
/// See also the note on coordinates and undistorted rendering in SFML.Graphics.Transformable.
/// </remarks>
////////////////////////////////////////////////////////////
public class Sprite : Transformable, IDrawable
{
    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Default constructor
    /// </summary>
    ////////////////////////////////////////////////////////////
    public Sprite() :
        base(sfSprite_create())
    {
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Construct the sprite from a source texture
    /// </summary>
    /// <param name="texture">Source texture to assign to the sprite</param>
    ////////////////////////////////////////////////////////////
    public Sprite(Texture texture) :
        base(sfSprite_create())
    {
        Texture = texture;
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Construct the sprite from a source texture
    /// </summary>
    /// <param name="texture">Source texture to assign to the sprite</param>
    /// <param name="rectangle">Sub-rectangle of the texture to assign to the sprite</param>
    ////////////////////////////////////////////////////////////
    public Sprite(Texture texture, IntRect rectangle) :
        base(sfSprite_create())
    {
        Texture = texture;
        TextureRect = rectangle;
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Construct the sprite from another sprite
    /// </summary>
    /// <param name="copy">Sprite to copy</param>
    ////////////////////////////////////////////////////////////
    public Sprite(Sprite copy) :
        base(sfSprite_copy(copy.CPointer))
    {
        Origin = copy.Origin;
        Position = copy.Position;
        Rotation = copy.Rotation;
        Scale = copy.Scale;
        Texture = copy.Texture;
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Global color of the object
    /// </summary>
    ////////////////////////////////////////////////////////////
    public Color Color
    {
        get { return sfSprite_getColor(CPointer); }
        set { sfSprite_setColor(CPointer, value); }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Source texture displayed by the sprite
    /// </summary>
    ////////////////////////////////////////////////////////////
    public Texture Texture
    {
        get { return _myTexture; }
        set { _myTexture = value; sfSprite_setTexture(CPointer, value != null ? value.CPointer : IntPtr.Zero, false); }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Sub-rectangle of the source image displayed by the sprite
    /// </summary>
    ////////////////////////////////////////////////////////////
    public IntRect TextureRect
    {
        get { return sfSprite_getTextureRect(CPointer); }
        set { sfSprite_setTextureRect(CPointer, value); }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>Get the local bounding rectangle of the entity.</para>
    /// <para>
    /// The returned rectangle is in local coordinates, which means
    /// that it ignores the transformations (translation, rotation,
    /// scale, ...) that are applied to the entity.
    /// In other words, this function returns the bounds of the
    /// entity in the entity's coordinate system.
    /// </para>
    /// </summary>
    /// <returns>Local bounding rectangle of the entity</returns>
    ////////////////////////////////////////////////////////////
    public FloatRect GetLocalBounds()
    {
        return sfSprite_getLocalBounds(CPointer);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>Get the global bounding rectangle of the entity.</para>
    /// <para>
    /// The returned rectangle is in global coordinates, which means
    /// that it takes in account the transformations (translation,
    /// rotation, scale, ...) that are applied to the entity.
    /// In other words, this function returns the bounds of the
    /// sprite in the global 2D world's coordinate system.
    /// </para>
    /// </summary>
    /// <returns>Global bounding rectangle of the entity</returns>
    ////////////////////////////////////////////////////////////
    public FloatRect GetGlobalBounds()
    {
        // we don't use the native getGlobalBounds function,
        // because we override the object's transform
        return Transform.TransformRect(GetLocalBounds());
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Provide a string describing the object
    /// </summary>
    /// <returns>String description of the object</returns>
    ////////////////////////////////////////////////////////////
    public override string ToString() => $"[Sprite] Color({Color}) Texture({Texture}) TextureRect({TextureRect})";

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Draw the sprite to a render target
    /// </summary>
    /// <param name="target">Render target to draw to</param>
    /// <param name="states">Current render states</param>
    ////////////////////////////////////////////////////////////
    public void Draw(IRenderTarget target, RenderStates states)
    {
        states.Transform *= Transform;
        var marshaledStates = states.Marshal();

        if (target is RenderWindow renderWindow)
        {
            sfRenderWindow_drawSprite(renderWindow.CPointer, CPointer, ref marshaledStates);
        }
        else if (target is RenderTexture renderTexture)
        {
            sfRenderTexture_drawSprite(renderTexture.CPointer, CPointer, ref marshaledStates);
        }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Handle the destruction of the object
    /// </summary>
    /// <param name="disposing">Is the GC disposing the object, or is it an explicit call ?</param>
    ////////////////////////////////////////////////////////////
    protected override void Destroy(bool disposing)
    {
        sfSprite_destroy(CPointer);
    }

    private Texture _myTexture;

    #region Imports

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern IntPtr sfSprite_create();

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern IntPtr sfSprite_copy(IntPtr sprite);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfSprite_destroy(IntPtr cPointer);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfSprite_setColor(IntPtr cPointer, Color color);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern Color sfSprite_getColor(IntPtr cPointer);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfRenderWindow_drawSprite(IntPtr cPointer, IntPtr sprite, ref RenderStates.MarshalData states);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfRenderTexture_drawSprite(IntPtr cPointer, IntPtr sprite, ref RenderStates.MarshalData states);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfSprite_setTexture(IntPtr cPointer, IntPtr texture, bool adjustToNewSize);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfSprite_setTextureRect(IntPtr cPointer, IntRect rect);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern IntRect sfSprite_getTextureRect(IntPtr cPointer);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern FloatRect sfSprite_getLocalBounds(IntPtr cPointer);
    #endregion
}
