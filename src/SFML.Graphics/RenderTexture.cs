using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security;

using SFML.System;
using SFML.Window;

namespace SFML.Graphics;

////////////////////////////////////////////////////////////
/// <summary>
/// Target for off-screen 2D rendering into an texture
/// </summary>
////////////////////////////////////////////////////////////
public class RenderTexture : ObjectBase, IRenderTarget
{
    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Create the render-texture with the given dimensions
    /// </summary>
    /// <param name="width">Width of the render-texture</param>
    /// <param name="height">Height of the render-texture</param>
    ////////////////////////////////////////////////////////////
    public RenderTexture(uint width, uint height) :
        this(width, height, default)
    {
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Create the render-texture with the given dimensions and
    /// a ContextSettings.
    /// </summary>
    /// <param name="width">Width of the render-texture</param>
    /// <param name="height">Height of the render-texture</param>
    /// <param name="contextSettings">A ContextSettings struct representing settings for the RenderTexture</param>
    ////////////////////////////////////////////////////////////
    public RenderTexture(uint width, uint height, ContextSettings contextSettings) :
        base(sfRenderTexture_createWithSettings(width, height, ref contextSettings))
    {
        _myDefaultView = new View(sfRenderTexture_getDefaultView(CPointer));
        Texture = new Texture(sfRenderTexture_getTexture(CPointer));
        GC.SuppressFinalize(_myDefaultView);
        GC.SuppressFinalize(Texture);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Activate of deactivate the render texture as the current target
    /// for rendering
    /// </summary>
    /// <param name="active">True to activate, false to deactivate (true by default)</param>
    /// <returns>True if operation was successful, false otherwise</returns>
    ////////////////////////////////////////////////////////////
    public bool SetActive(bool active)
    {
        return sfRenderTexture_setActive(CPointer, active);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Enable or disable texture repeating
    /// </summary>
    ///
    /// <remarks>
    /// This property is similar to <see cref="Texture.Repeated"/>.
    /// This parameter is disabled by default.
    /// </remarks>
    ////////////////////////////////////////////////////////////
    public bool Repeated
    {
        get { return sfRenderTexture_isRepeated(CPointer); }
        set { sfRenderTexture_setRepeated(CPointer, value); }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Size of the rendering region of the render texture
    /// </summary>
    ////////////////////////////////////////////////////////////
    public Vector2u Size
    {
        get { return sfRenderTexture_getSize(CPointer); }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Default view of the render texture
    /// </summary>
    ////////////////////////////////////////////////////////////
    public View DefaultView
    {
        get { return new View(_myDefaultView); }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Return the current active view
    /// </summary>
    /// <returns>The current view</returns>
    ////////////////////////////////////////////////////////////
    public View GetView()
    {
        return new View(sfRenderTexture_getView(CPointer));
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Change the current active view
    /// </summary>
    /// <param name="view">New view</param>
    ////////////////////////////////////////////////////////////
    public void SetView(View view)
    {
        sfRenderTexture_setView(CPointer, view.CPointer);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Get the viewport of a view applied to this target
    /// </summary>
    /// <param name="view">Target view</param>
    /// <returns>Viewport rectangle, expressed in pixels in the current target</returns>
    ////////////////////////////////////////////////////////////
    public IntRect GetViewport(View view)
    {
        return sfRenderTexture_getViewport(CPointer, view.CPointer);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>
    /// Convert a point from target coordinates to world
    /// coordinates, using the current view
    /// </para>
    /// <para>
    /// This function is an overload of the MapPixelToCoords
    /// function that implicitly uses the current view.
    /// It is equivalent to:
    /// target.MapPixelToCoords(point, target.GetView());
    /// </para>
    /// </summary>
    /// <param name="point">Pixel to convert</param>
    /// <returns>The converted point, in "world" coordinates</returns>
    ////////////////////////////////////////////////////////////
    public Vector2 MapPixelToCoords(Vector2i point)
    {
        return MapPixelToCoords(point, GetView());
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>Convert a point from target coordinates to world coordinates</para>
    /// <para>
    /// This function finds the 2D position that matches the
    /// given pixel of the render-target. In other words, it does
    /// the inverse of what the graphics card does, to find the
    /// initial position of a rendered pixel.
    /// </para>
    /// <para>
    /// Initially, both coordinate systems (world units and target pixels)
    /// match perfectly. But if you define a custom view or resize your
    /// render-target, this assertion is not true anymore, ie. a point
    /// located at (10, 50) in your render-target may map to the point
    /// (150, 75) in your 2D world -- if the view is translated by (140, 25).
    /// </para>
    /// <para>
    /// For render-windows, this function is typically used to find
    /// which point (or object) is located below the mouse cursor.
    /// </para>
    /// <para>
    /// This version uses a custom view for calculations, see the other
    /// overload of the function if you want to use the current view of the
    /// render-target.
    /// </para>
    /// </summary>
    /// <param name="point">Pixel to convert</param>
    /// <param name="view">The view to use for converting the point</param>
    /// <returns>The converted point, in "world" coordinates</returns>
    ////////////////////////////////////////////////////////////
    public Vector2 MapPixelToCoords(Vector2i point, View view)
    {
        return sfRenderTexture_mapPixelToCoords(CPointer, point, view != null ? view.CPointer : IntPtr.Zero);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>
    /// Convert a point from world coordinates to target
    /// coordinates, using the current view
    /// </para>
    /// <para>
    /// This function is an overload of the mapCoordsToPixel
    /// function that implicitly uses the current view.
    /// It is equivalent to:
    /// target.MapCoordsToPixel(point, target.GetView());
    /// </para>
    /// </summary>
    /// <param name="point">Point to convert</param>
    /// <returns>The converted point, in target coordinates (pixels)</returns>
    ////////////////////////////////////////////////////////////
    public Vector2i MapCoordsToPixel(Vector2 point)
    {
        return MapCoordsToPixel(point, GetView());
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>Convert a point from world coordinates to target coordinates</para>
    /// <para>
    /// This function finds the pixel of the render-target that matches
    /// the given 2D point. In other words, it goes through the same process
    /// as the graphics card, to compute the final position of a rendered point.
    /// </para>
    /// <para>
    /// Initially, both coordinate systems (world units and target pixels)
    /// match perfectly. But if you define a custom view or resize your
    /// render-target, this assertion is not true anymore, ie. a point
    /// located at (150, 75) in your 2D world may map to the pixel
    /// (10, 50) of your render-target -- if the view is translated by (140, 25).
    /// </para>
    /// <para>
    /// This version uses a custom view for calculations, see the other
    /// overload of the function if you want to use the current view of the
    /// render-target.
    /// </para>
    /// </summary>
    /// <param name="point">Point to convert</param>
    /// <param name="view">The view to use for converting the point</param>
    /// <returns>The converted point, in target coordinates (pixels)</returns>
    ////////////////////////////////////////////////////////////
    public Vector2i MapCoordsToPixel(Vector2 point, View view)
    {
        return sfRenderTexture_mapCoordsToPixel(CPointer, point, view != null ? view.CPointer : IntPtr.Zero);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Generate a mipmap using the current texture data
    /// </summary>
    ///
    /// <remarks>
    /// This function is similar to <see cref="Texture.GenerateMipmap"/> and operates
    /// on the texture used as the target for drawing.
    /// Be aware that any draw operation may modify the base level image data.
    /// For this reason, calling this function only makes sense after all
    /// drawing is completed and display has been called. Not calling display
    /// after subsequent drawing will lead to undefined behavior if a mipmap
    /// had been previously generated.
    /// </remarks>
    ///
    /// <returns>True if mipmap generation was successful, false if unsuccessful</returns>
    ////////////////////////////////////////////////////////////
    public bool GenerateMipmap()
    {
        return sfRenderTexture_generateMipmap(CPointer);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Clear the entire render texture with black color
    /// </summary>
    ////////////////////////////////////////////////////////////
    public void Clear()
    {
        sfRenderTexture_clear(CPointer, Color.Black);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Clear the entire render texture with a single color
    /// </summary>
    /// <param name="color">Color to use to clear the texture</param>
    ////////////////////////////////////////////////////////////
    public void Clear(Color color)
    {
        sfRenderTexture_clear(CPointer, color);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Update the contents of the target texture
    /// </summary>
    ////////////////////////////////////////////////////////////
    public void Display()
    {
        sfRenderTexture_display(CPointer);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Target texture of the render texture
    /// </summary>
    ////////////////////////////////////////////////////////////
    public Texture Texture { get; }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// The maximum anti-aliasing level supported by the system
    /// </summary>
    ////////////////////////////////////////////////////////////
    public static uint MaximumAntialiasingLevel => sfRenderTexture_getMaximumAntialiasingLevel();

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Control the smooth filter
    /// </summary>
    ////////////////////////////////////////////////////////////
    public bool Smooth
    {
        get { return sfRenderTexture_isSmooth(CPointer); }
        set { sfRenderTexture_setSmooth(CPointer, value); }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Draw a drawable object to the render-target, with default render states
    /// </summary>
    /// <param name="drawable">Object to draw</param>
    ////////////////////////////////////////////////////////////
    public void Draw(IDrawable drawable)
    {
        Draw(drawable, RenderStates.Default);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Draw a drawable object to the render-target
    /// </summary>
    /// <param name="drawable">Object to draw</param>
    /// <param name="states">Render states to use for drawing</param>
    ////////////////////////////////////////////////////////////
    public void Draw(IDrawable drawable, RenderStates states)
    {
        drawable.Draw(this, states);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Draw primitives defined by an array of vertices, with default render states
    /// </summary>
    /// <param name="vertices">Pointer to the vertices</param>
    /// <param name="type">Type of primitives to draw</param>
    ////////////////////////////////////////////////////////////
    public void Draw(Vertex[] vertices, PrimitiveType type)
    {
        Draw(vertices, type, RenderStates.Default);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Draw primitives defined by an array of vertices
    /// </summary>
    /// <param name="vertices">Pointer to the vertices</param>
    /// <param name="type">Type of primitives to draw</param>
    /// <param name="states">Render states to use for drawing</param>
    ////////////////////////////////////////////////////////////
    public void Draw(Vertex[] vertices, PrimitiveType type, RenderStates states)
    {
        Draw(vertices, 0, (uint)vertices.Length, type, states);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Draw primitives defined by a sub-array of vertices, with default render states
    /// </summary>
    /// <param name="vertices">Array of vertices to draw</param>
    /// <param name="start">Index of the first vertex to draw in the array</param>
    /// <param name="count">Number of vertices to draw</param>
    /// <param name="type">Type of primitives to draw</param>
    ////////////////////////////////////////////////////////////
    public void Draw(Vertex[] vertices, uint start, uint count, PrimitiveType type)
    {
        Draw(vertices, start, count, type, RenderStates.Default);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Draw primitives defined by a sub-array of vertices
    /// </summary>
    /// <param name="vertices">Pointer to the vertices</param>
    /// <param name="start">Index of the first vertex to use in the array</param>
    /// <param name="count">Number of vertices to draw</param>
    /// <param name="type">Type of primitives to draw</param>
    /// <param name="states">Render states to use for drawing</param>
    ////////////////////////////////////////////////////////////
    public unsafe void Draw(Vertex[] vertices, uint start, uint count, PrimitiveType type, RenderStates states)
    {
        var marshaledStates = states.Marshal();
        fixed (Vertex* vertexPtr = vertices)
        {
            sfRenderTexture_drawPrimitives(CPointer, vertexPtr + start, count, type, ref marshaledStates);
        }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>Save the current OpenGL render states and matrices.</para>
    /// <para>
    /// This function can be used when you mix SFML drawing
    /// and direct OpenGL rendering. Combined with PopGLStates,
    /// it ensures that:
    /// \li SFML's internal states are not messed up by your OpenGL code
    /// \li your OpenGL states are not modified by a call to a SFML function
    /// </para>
    /// <para>
    /// More specifically, it must be used around code that
    /// calls Draw functions. Example:
    /// </para>
    /// <para>
    /// // OpenGL code here...
    /// window.PushGLStates();
    /// window.Draw(...);
    /// window.Draw(...);
    /// window.PopGLStates();
    /// // OpenGL code here...
    /// </para>
    /// <para>
    /// Note that this function is quite expensive: it saves all the
    /// possible OpenGL states and matrices, even the ones you
    /// don't care about. Therefore it should be used wisely.
    /// It is provided for convenience, but the best results will
    /// be achieved if you handle OpenGL states yourself (because
    /// you know which states have really changed, and need to be
    /// saved and restored). Take a look at the ResetGLStates
    /// function if you do so.
    /// </para>
    /// </summary>
    ////////////////////////////////////////////////////////////
    public void PushGLStates()
    {
        sfRenderTexture_pushGLStates(CPointer);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>Restore the previously saved OpenGL render states and matrices.</para>
    /// <para>
    /// See the description of PushGLStates to get a detailed
    /// description of these functions.
    /// </para>
    /// </summary>
    ////////////////////////////////////////////////////////////
    public void PopGLStates()
    {
        sfRenderTexture_popGLStates(CPointer);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>Reset the internal OpenGL states so that the target is ready for drawing.</para>
    /// <para>
    /// This function can be used when you mix SFML drawing
    /// and direct OpenGL rendering, if you choose not to use
    /// PushGLStates/PopGLStates. It makes sure that all OpenGL
    /// states needed by SFML are set, so that subsequent Draw()
    /// calls will work as expected.
    /// </para>
    /// <para>Example:</para>
    /// <para>
    /// // OpenGL code here...
    /// glPushAttrib(...);
    /// window.ResetGLStates();
    /// window.Draw(...);
    /// window.Draw(...);
    /// glPopAttrib(...);
    /// // OpenGL code here...
    /// </para>
    /// </summary>
    ////////////////////////////////////////////////////////////
    public void ResetGLStates()
    {
        sfRenderTexture_resetGLStates(CPointer);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Provide a string describing the object
    /// </summary>
    /// <returns>String description of the object</returns>
    ////////////////////////////////////////////////////////////
    public override string ToString()
    {
        return "[RenderTexture]" +
               " Size(" + Size + ")" +
               " Texture(" + Texture + ")" +
               " DefaultView(" + DefaultView + ")" +
               " View(" + GetView() + ")";
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Handle the destruction of the object
    /// </summary>
    /// <param name="disposing">Is the GC disposing the object, or is it an explicit call ?</param>
    ////////////////////////////////////////////////////////////
    protected override void Destroy(bool disposing)
    {
        if (!disposing)
        {
            Context.Global.SetActive(true);
        }

        sfRenderTexture_destroy(CPointer);

        if (disposing)
        {
            _myDefaultView.Dispose();
            Texture.Dispose();
        }

        if (!disposing)
        {
            Context.Global.SetActive(false);
        }
    }

    private readonly View _myDefaultView;

    #region Imports
    [Obsolete("sfRenderTexture_create is obselete. Use sfRenderTexture_createWithSettings instead.")]
    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern IntPtr sfRenderTexture_create(uint width, uint height, bool depthBuffer);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern IntPtr sfRenderTexture_createWithSettings(uint width, uint height, ref ContextSettings settings);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfRenderTexture_destroy(IntPtr cPointer);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfRenderTexture_clear(IntPtr cPointer, Color clearColor);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern Vector2u sfRenderTexture_getSize(IntPtr cPointer);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern bool sfRenderTexture_setActive(IntPtr cPointer, bool active);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern bool sfRenderTexture_saveGLStates(IntPtr cPointer);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern bool sfRenderTexture_restoreGLStates(IntPtr cPointer);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern bool sfRenderTexture_display(IntPtr cPointer);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfRenderTexture_setView(IntPtr cPointer, IntPtr view);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern IntPtr sfRenderTexture_getView(IntPtr cPointer);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern IntPtr sfRenderTexture_getDefaultView(IntPtr cPointer);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern IntRect sfRenderTexture_getViewport(IntPtr cPointer, IntPtr targetView);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern Vector2i sfRenderTexture_mapCoordsToPixel(IntPtr cPointer, Vector2 point, IntPtr view);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern Vector2 sfRenderTexture_mapPixelToCoords(IntPtr cPointer, Vector2i point, IntPtr view);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern IntPtr sfRenderTexture_getTexture(IntPtr cPointer);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern uint sfRenderTexture_getMaximumAntialiasingLevel();

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfRenderTexture_setSmooth(IntPtr cPointer, bool smooth);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern bool sfRenderTexture_isSmooth(IntPtr cPointer);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfRenderTexture_setRepeated(IntPtr cPointer, bool repeated);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern bool sfRenderTexture_isRepeated(IntPtr cPointer);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern bool sfRenderTexture_generateMipmap(IntPtr cPointer);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern unsafe void sfRenderTexture_drawPrimitives(IntPtr cPointer, Vertex* vertexPtr, uint vertexCount, PrimitiveType type, ref RenderStates.MarshalData renderStates);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfRenderTexture_pushGLStates(IntPtr cPointer);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfRenderTexture_popGLStates(IntPtr cPointer);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfRenderTexture_resetGLStates(IntPtr cPointer);
    #endregion
}
