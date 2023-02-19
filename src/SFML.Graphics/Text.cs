using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

using SFML.System;

namespace SFML.Graphics;

////////////////////////////////////////////////////////////
/// <summary>
/// This class defines a graphical 2D text, that can be drawn on screen
/// </summary>
/// <remarks>
/// See also the note on coordinates and undistorted rendering in SFML.Graphics.Transformable.
/// </remarks>
////////////////////////////////////////////////////////////
public class Text : Transformable, IDrawable
{
    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Enumerate the string drawing styles
    /// </summary>
    ////////////////////////////////////////////////////////////
    [Flags]
    public enum Styles
    {
        /// <summary>Regular characters, no style</summary>
        Regular = 0,

        /// <summary>Bold characters</summary>
        Bold = 1 << 0,

        /// <summary>Italic characters</summary>
        Italic = 1 << 1,

        /// <summary>Underlined characters</summary>
        Underlined = 1 << 2,

        /// <summary>Strike through characters</summary>
        StrikeThrough = 1 << 3
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Default constructor
    /// </summary>
    ////////////////////////////////////////////////////////////
    public Text() :
        this("", null)
    {
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Construct the text from a string and a font
    /// </summary>
    /// <param name="str">String to display</param>
    /// <param name="font">Font to use</param>
    ////////////////////////////////////////////////////////////
    public Text(string str, Font font) :
        this(str, font, 30)
    {
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Construct the text from a string, font and size
    /// </summary>
    /// <param name="str">String to display</param>
    /// <param name="font">Font to use</param>
    /// <param name="characterSize">Base characters size</param>
    ////////////////////////////////////////////////////////////
    public Text(string str, Font font, uint characterSize) :
        base(sfText_create())
    {
        DisplayedString = str;
        Font = font;
        CharacterSize = characterSize;
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Construct the text from another text
    /// </summary>
    /// <param name="copy">Text to copy</param>
    ////////////////////////////////////////////////////////////
    public Text(Text copy) :
        base(sfText_copy(copy.CPointer))
    {
        Origin = copy.Origin;
        Position = copy.Position;
        Rotation = copy.Rotation;
        Scale = copy.Scale;

        Font = copy.Font;
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Fill color of the object
    /// </summary>
    ///
    /// <remarks>
    /// <para>Deprecated. Use <see cref="FillColor"/> instead.</para>
    /// <para>
    /// By default, the text's fill color is opaque white.
    /// Setting the fill color to a transparent color with an outline
    /// will cause the outline to be displayed in the fill area of the text.
    /// </para>
    /// </remarks>
    ////////////////////////////////////////////////////////////
    [Obsolete($"Use {nameof(FillColor)} instead")]
    public Color Color
    {
        get { return sfText_getFillColor(CPointer); }
        set { sfText_setFillColor(CPointer, value); }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Fill color of the object
    /// </summary>
    ///
    /// <remarks>
    /// By default, the text's fill color is opaque white.
    /// Setting the fill color to a transparent color with an outline
    /// will cause the outline to be displayed in the fill area of the text.
    /// </remarks>
    ////////////////////////////////////////////////////////////
    public Color FillColor
    {
        get { return sfText_getFillColor(CPointer); }
        set { sfText_setFillColor(CPointer, value); }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Outline color of the object
    /// </summary>
    ///
    /// <remarks>
    /// By default, the text's outline color is opaque black.
    /// </remarks>
    ////////////////////////////////////////////////////////////
    public Color OutlineColor
    {
        get { return sfText_getOutlineColor(CPointer); }
        set { sfText_setOutlineColor(CPointer, value); }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Thickness of the object's outline
    /// </summary>
    ///
    /// <remarks>
    /// <para>By default, the outline thickness is 0.</para>
    /// <para>Be aware that using a negative value for the outline
    /// thickness will cause distorted rendering.</para>
    /// </remarks>
    ////////////////////////////////////////////////////////////
    public float OutlineThickness
    {
        get { return sfText_getOutlineThickness(CPointer); }
        set { sfText_setOutlineThickness(CPointer, value); }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// String which is displayed
    /// </summary>
    ////////////////////////////////////////////////////////////
    public string DisplayedString
    {
        get
        {
            // Get a pointer to the source string (UTF-32)
            var source = sfText_getUnicodeString(CPointer);

            // Find its length (find the terminating 0)
            uint length = 0;
            unsafe
            {
                for (var ptr = (uint*)source.ToPointer(); *ptr != 0; ++ptr)
                {
                    length++;
                }
            }

            // Copy it to a byte array
            var sourceBytes = new byte[length * 4];
            Marshal.Copy(source, sourceBytes, 0, sourceBytes.Length);

            // Convert it to a C# string
            return Encoding.UTF32.GetString(sourceBytes);
        }

        set
        {
            // Copy the string to a null-terminated UTF-32 byte array
            var utf32 = Encoding.UTF32.GetBytes(value + '\0');

            // Pass it to the C API
            unsafe
            {
                fixed (byte* ptr = utf32)
                {
                    sfText_setUnicodeString(CPointer, (IntPtr)ptr);
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Font used to display the text
    /// </summary>
    ////////////////////////////////////////////////////////////
    public Font Font
    {
        get { return _myFont; }
        set { _myFont = value; sfText_setFont(CPointer, value != null ? value.CPointer : IntPtr.Zero); }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Base size of characters
    /// </summary>
    ////////////////////////////////////////////////////////////
    public uint CharacterSize
    {
        get { return sfText_getCharacterSize(CPointer); }
        set { sfText_setCharacterSize(CPointer, value); }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Size of the letter spacing factor
    /// </summary>
    ////////////////////////////////////////////////////////////
    public float LetterSpacing
    {
        get { return sfText_getLetterSpacing(CPointer); }
        set { sfText_setLetterSpacing(CPointer, value); }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Size of the line spacing factor
    /// </summary>
    ////////////////////////////////////////////////////////////
    public float LineSpacing
    {
        get { return sfText_getLineSpacing(CPointer); }
        set { sfText_setLineSpacing(CPointer, value); }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Style of the text (see Styles enum)
    /// </summary>
    ////////////////////////////////////////////////////////////
    public Styles Style
    {
        get { return sfText_getStyle(CPointer); }
        set { sfText_setStyle(CPointer, value); }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Return the visual position of the Index-th character of the text,
    /// in coordinates relative to the text
    /// (note : translation, origin, rotation and scale are not applied)
    /// </summary>
    /// <param name="index">Index of the character</param>
    /// <returns>Position of the Index-th character (end of text if Index is out of range)</returns>
    ////////////////////////////////////////////////////////////
    public Vector2 FindCharacterPos(uint index)
    {
        return sfText_findCharacterPos(CPointer, index);
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
        return sfText_getLocalBounds(CPointer);
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
    public override string ToString()
    {
        return "[Text]" +
               " FillColor(" + FillColor + ")" +
               " OutlineColor(" + OutlineColor + ")" +
               " String(" + DisplayedString + ")" +
               " Font(" + Font + ")" +
               " CharacterSize(" + CharacterSize + ")" +
               " OutlineThickness(" + OutlineThickness + ")" +
               " Style(" + Style + ")";
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Draw the text to a render target
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
            sfRenderWindow_drawText(renderWindow.CPointer, CPointer, ref marshaledStates);
        }
        else if (target is RenderTexture renderTexture)
        {
            sfRenderTexture_drawText(renderTexture.CPointer, CPointer, ref marshaledStates);
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
        sfText_destroy(CPointer);
    }

    private Font _myFont;

    #region Imports
    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern IntPtr sfText_create();

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern IntPtr sfText_copy(IntPtr text);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfText_destroy(IntPtr cPointer);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, Obsolete]
    private static extern void sfText_setColor(IntPtr cPointer, Color color);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfText_setFillColor(IntPtr cPointer, Color color);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfText_setOutlineColor(IntPtr cPointer, Color color);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfText_setOutlineThickness(IntPtr cPointer, float thickness);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, Obsolete]
    private static extern Color sfText_getColor(IntPtr cPointer);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern Color sfText_getFillColor(IntPtr cPointer);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern Color sfText_getOutlineColor(IntPtr cPointer);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern float sfText_getOutlineThickness(IntPtr cPointer);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfRenderWindow_drawText(IntPtr cPointer, IntPtr text, ref RenderStates.MarshalData states);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfRenderTexture_drawText(IntPtr cPointer, IntPtr text, ref RenderStates.MarshalData states);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfText_setUnicodeString(IntPtr cPointer, IntPtr text);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfText_setFont(IntPtr cPointer, IntPtr font);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfText_setCharacterSize(IntPtr cPointer, uint size);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfText_setLineSpacing(IntPtr cPointer, float spacingFactor);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfText_setLetterSpacing(IntPtr cPointer, float spacingFactor);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfText_setStyle(IntPtr cPointer, Styles style);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern IntPtr sfText_getString(IntPtr cPointer);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern IntPtr sfText_getUnicodeString(IntPtr cPointer);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern uint sfText_getCharacterSize(IntPtr cPointer);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern float sfText_getLetterSpacing(IntPtr cPointer);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern float sfText_getLineSpacing(IntPtr cPointer);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern Styles sfText_getStyle(IntPtr cPointer);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern FloatRect sfText_getRect(IntPtr cPointer);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern Vector2 sfText_findCharacterPos(IntPtr cPointer, uint index);

    [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern FloatRect sfText_getLocalBounds(IntPtr cPointer);
    #endregion
}
