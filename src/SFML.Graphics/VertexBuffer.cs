using System;
using System.Runtime.InteropServices;
using System.Security;

using SFML.System;

namespace SFML.Graphics;

////////////////////////////////////////////////////////////
/// <summary>
/// <para>Vertex buffer storage for one or more 2D primitives.</para>
/// <para>VertexBuffer is a simple wrapper around a dynamic buffer of vertices and a primitives type.</para>
/// <para>Unlike SFML.VertexArray, the vertex data is stored in graphics memory.</para>
/// </summary>
////////////////////////////////////////////////////////////
public class VertexBuffer : ObjectBase, IDrawable
{
    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>Usage specifiers</para>
    /// <para>
    /// If data is going to be updated once or more every frame,
    /// set the usage to Stream. If data is going
    /// to be set once and used for a long time without being
    /// modified, set the usage to Static.
    /// For everything else Dynamic should
    /// be a good compromise.
    /// </para>
    /// </summary>
    ////////////////////////////////////////////////////////////
    public enum UsageSpecifier
    {
        /// <summary>Constantly changing data.</summary>
        Stream,
        /// <summary>Occasionally changing data.</summary>
        Dynamic,
        /// <summary>Rarely changing data.</summary>
        Static
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>Whether or not the system supports vertex buffers</para>
    /// <para>
    /// This function should always be called before using
    /// the vertex buffer features. If it returns false, then
    /// any attempt to use sf::VertexBuffer will fail.
    /// </para>
    /// </summary>
    ///////////////////////////////////////////////////////////
    public static bool Available { get { return sfVertexBuffer_isAvailable(); } }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>
    /// Create a new vertex buffer with a specific
    /// PrimitiveType and usage specifier.
    /// </para>
    /// <para>
    /// Creates the vertex buffer, allocating enough graphcis
    /// memory to hold \p vertexCount vertices, and sets its
    /// primitive type to \p type and usage to \p usage.
    /// </para>
    /// </summary>
    /// <param name="vertexCount">Amount of vertices</param>
    /// <param name="primitiveType">Type of primitives</param>
    /// <param name="usageType">Usage specifier</param>
    ////////////////////////////////////////////////////////////
    public VertexBuffer(uint vertexCount, PrimitiveType primitiveType, UsageSpecifier usageType)
        : base(sfVertexBuffer_create(vertexCount, primitiveType, usageType))
    {
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Construct the vertex buffer from another vertex array
    /// </summary>
    /// <param name="copy">VertexBuffer to copy</param>
    ////////////////////////////////////////////////////////////
    public VertexBuffer(VertexBuffer copy)
        : base(sfVertexBuffer_copy(copy.CPointer))
    {
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Total vertex count
    /// </summary>
    ////////////////////////////////////////////////////////////
    public uint VertexCount { get { return sfVertexBuffer_getVertexCount(CPointer); } }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>OpenGL handle of the vertex buffer or 0 if not yet created</para>
    /// <para>
    /// You shouldn't need to use this property, unless you have
    /// very specific stuff to implement that SFML doesn't support,
    /// or implement a temporary workaround until a bug is fixed.
    /// </para>
    /// </summary>
    ////////////////////////////////////////////////////////////
    public uint NativeHandle
    {
        get { return sfVertexBuffer_getNativeHandle(CPointer); }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// The type of primitives drawn by the vertex buffer
    /// </summary>
    ////////////////////////////////////////////////////////////
    public PrimitiveType PrimitiveType
    {
        get { return sfVertexBuffer_getPrimitiveType(CPointer); }
        set { sfVertexBuffer_setPrimitiveType(CPointer, value); }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// The usage specifier for the vertex buffer
    /// </summary>
    ////////////////////////////////////////////////////////////
    public UsageSpecifier Usage
    {
        get { return sfVertexBuffer_getUsage(CPointer); }
        set { sfVertexBuffer_setUsage(CPointer, value); }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>Bind a vertex buffer for rendering.</para>
    /// <para>
    /// This function is not part of the graphics API, it mustn't
    /// be used when drawing SFML entities. It must be used only if
    /// you mix VertexBuffer with OpenGL code.
    /// </para>
    /// </summary>
    /// <param name="vertexBuffer">The vertex buffer to bind, can be null to use no vertex buffer</param>
    ////////////////////////////////////////////////////////////
    public static void Bind(VertexBuffer vertexBuffer)
    {
        sfVertexBuffer_bind(vertexBuffer?.CPointer ?? IntPtr.Zero);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>
    /// Update a part of the buffer from an array of vertices
    /// offset is specified as the number of vertices to skip
    /// from the beginning of the buffer.
    /// </para>
    /// <para>
    /// If offset is 0 and vertexCount is equal to the size of
    /// the currently created buffer, its whole contents are replaced.
    /// </para>
    /// <para>
    /// If offset is 0 and vertexCount is greater than the
    /// size of the currently created buffer, a new buffer is created
    /// containing the vertex data.
    /// </para>
    /// <para>
    /// If offset is 0 and vertexCount is less than the size of
    /// the currently created buffer, only the corresponding region
    /// is updated.
    /// </para>
    /// <para>
    /// If offset is not 0 and offset + vertexCount is greater
    /// than the size of the currently created buffer, the update fails.
    /// </para>
    /// <para>
    /// No additional check is performed on the size of the vertex
    /// array, passing invalid arguments will lead to undefined
    /// behavior.
    /// </para>
    /// </summary>
    /// <param name="vertices">Array of vertices to copy to the buffer</param>
    /// <param name="vertexCount">Number of vertices to copy</param>
    /// <param name="offset">Offset in the buffer to copy to</param>
    ////////////////////////////////////////////////////////////
    public bool Update(Vertex[] vertices, uint vertexCount, uint offset)
    {
        unsafe
        {
            fixed (Vertex* verts = vertices)
            {
                return sfVertexBuffer_update(CPointer, verts, vertexCount, offset);
            }
        }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>
    /// Update a part of the buffer from an array of vertices
    /// assuming an offset of 0 and a vertex count the length of the passed array.
    /// </para>
    /// <para>
    /// If offset is 0 and vertexCount is equal to the size of
    /// the currently created buffer, its whole contents are replaced.
    /// </para>
    /// <para>
    /// If offset is 0 and vertexCount is greater than the
    /// size of the currently created buffer, a new buffer is created
    /// containing the vertex data.
    /// </para>
    /// <para>
    /// If offset is 0 and vertexCount is less than the size of
    /// the currently created buffer, only the corresponding region
    /// is updated.
    /// </para>
    /// <para>
    /// If offset is not 0 and offset + vertexCount is greater
    /// than the size of the currently created buffer, the update fails.
    /// </para>
    /// <para>
    /// No additional check is performed on the size of the vertex
    /// array, passing invalid arguments will lead to undefined
    /// behavior.
    /// </para>
    /// </summary>
    /// <param name="vertices">Array of vertices to copy to the buffer</param>
    ////////////////////////////////////////////////////////////
    public bool Update(Vertex[] vertices)
    {
        return this.Update(vertices, (uint)vertices.Length, 0);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>
    /// Update a part of the buffer from an array of vertices
    /// assuming a vertex count the length of the passed array.
    /// </para>
    /// <para>
    /// If offset is 0 and vertexCount is equal to the size of
    /// the currently created buffer, its whole contents are replaced.
    /// </para>
    /// <para>
    /// If offset is 0 and vertexCount is greater than the
    /// size of the currently created buffer, a new buffer is created
    /// containing the vertex data.
    /// </para>
    /// <para>
    /// If offset is 0 and vertexCount is less than the size of
    /// the currently created buffer, only the corresponding region
    /// is updated.
    /// </para>
    /// <para>
    /// If offset is not 0 and offset + vertexCount is greater
    /// than the size of the currently created buffer, the update fails.
    /// </para>
    /// <para>
    /// No additional check is performed on the size of the vertex
    /// array, passing invalid arguments will lead to undefined
    /// behavior.
    /// </para>
    /// </summary>
    /// <param name="vertices">Array of vertices to copy to the buffer</param>
    /// <param name="offset">Offset in the buffer to copy to</param>
    ////////////////////////////////////////////////////////////
    public bool Update(Vertex[] vertices, uint offset)
    {
        return this.Update(vertices, (uint)vertices.Length, offset);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Copy the contents of another buffer into this buffer
    /// </summary>
    /// <param name="other">Vertex buffer whose contents to copy into first vertex buffer</param>
    ////////////////////////////////////////////////////////////
    public bool Update(VertexBuffer other)
    {
        return sfVertexBuffer_updateFromVertexBuffer(CPointer, other.CPointer);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Swap the contents of another buffer into this buffer
    /// </summary>
    /// <param name="other">Vertex buffer whose contents to swap with</param>
    ////////////////////////////////////////////////////////////
    public void Swap(VertexBuffer other)
    {
        sfVertexBuffer_swap(CPointer, other.CPointer);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Handle the destruction of the object
    /// </summary>
    /// <param name="disposing">Is the GC disposing the object, or is it an explicit call ?</param>
    ////////////////////////////////////////////////////////////
    protected override void Destroy(bool disposing)
    {
        sfVertexBuffer_destroy(CPointer);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Draw the vertex buffer to a render target
    /// </summary>
    /// <param name="target">Render target to draw to</param>
    /// <param name="states">Current render states</param>
    ////////////////////////////////////////////////////////////
    public void Draw(IRenderTarget target, RenderStates states)
    {
        var marshaledStates = states.Marshal();

        if (target is RenderWindow renderWindow)
        {
            sfRenderWindow_drawVertexBuffer(renderWindow.CPointer, CPointer, ref marshaledStates);
        }
        else if (target is RenderTexture renderTexture)
        {
            sfRenderTexture_drawVertexBuffer(renderTexture.CPointer, CPointer, ref marshaledStates);
        }
    }

    #region Imports
    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern IntPtr sfVertexBuffer_create(uint vertexCount, PrimitiveType type, UsageSpecifier usage);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern IntPtr sfVertexBuffer_copy(IntPtr copy);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfVertexBuffer_destroy(IntPtr cPointer);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern uint sfVertexBuffer_getVertexCount(IntPtr cPointer);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern unsafe bool sfVertexBuffer_update(IntPtr cPointer, Vertex* vertices, uint vertexCount, uint offset);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern bool sfVertexBuffer_updateFromVertexBuffer(IntPtr cPointer, IntPtr other);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfVertexBuffer_swap(IntPtr cPointer, IntPtr other);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern uint sfVertexBuffer_getNativeHandle(IntPtr cPointer);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfVertexBuffer_setPrimitiveType(IntPtr cPointer, PrimitiveType primitiveType);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern PrimitiveType sfVertexBuffer_getPrimitiveType(IntPtr cPointer);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfVertexBuffer_setUsage(IntPtr cPointer, UsageSpecifier usageType);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern UsageSpecifier sfVertexBuffer_getUsage(IntPtr cPointer);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfVertexBuffer_bind(IntPtr cPointer);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern bool sfVertexBuffer_isAvailable();

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfRenderWindow_drawVertexBuffer(IntPtr cPointer, IntPtr vertexArray, ref RenderStates.MarshalData states);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfRenderTexture_drawVertexBuffer(IntPtr cPointer, IntPtr vertexBuffer, ref RenderStates.MarshalData states);
    #endregion
}
