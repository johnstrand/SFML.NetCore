using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace SFML.Graphics;

////////////////////////////////////////////////////////////
/// <summary>
/// Utility class for manipulating 32-bits RGBA colors
/// </summary>
////////////////////////////////////////////////////////////
[StructLayout(LayoutKind.Sequential)]
public struct Color : IEquatable<Color>
{
    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Construct the color from its red, green and blue components
    /// </summary>
    /// <param name="red">Red component</param>
    /// <param name="green">Green component</param>
    /// <param name="blue">Blue component</param>
    ////////////////////////////////////////////////////////////
    public Color(byte red, byte green, byte blue) : this(red, green, blue, 255) { }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Construct the color from its red, green, blue and alpha components
    /// </summary>
    /// <param name="red">Red component</param>
    /// <param name="green">Green component</param>
    /// <param name="blue">Blue component</param>
    /// <param name="alpha">Alpha (transparency) component</param>
    ////////////////////////////////////////////////////////////
    public Color(byte red, byte green, byte blue, byte alpha)
    {
        R = red;
        G = green;
        B = blue;
        A = alpha;
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Construct the color from its red, green and blue components, as stored in a <see cref="Vector3"/>. The values should be in the range of 0 to 1, inclusive
    /// </summary>
    /// <param name="vector">A vector containing color information, with <see cref="Vector3.X"/> as <see cref="R"/>, <see cref="Vector3.Y"/> as <see cref="G"/>, and <see cref="Vector3.Z"/> as <see cref="B"/></param>
    ////////////////////////////////////////////////////////////
    public Color(Vector3 vector) : this(vector.X, vector.Y, vector.Z)
    {
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Construct the color from its red, green, blue and alpha components, as stored in a <see cref="Vector4"/>. The values should be in the range of 0 to 1, inclusive
    /// </summary>
    /// <param name="vector">A vector containing color information, with <see cref="Vector4.X"/> as <see cref="R"/>, <see cref="Vector4.Y"/> as <see cref="G"/>, <see cref="Vector4.Z"/> as <see cref="B"/>, and <see cref="Vector4.W"/> as <see cref="A"/></param>
    ////////////////////////////////////////////////////////////
    public Color(Vector4 vector) : this(vector.X, vector.Y, vector.Z, vector.W)
    {
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Construct the color from its red, green, blue and alpha components. The values should be in the range of 0 to 1, inclusive
    /// </summary>
    /// <param name="red">Red component</param>
    /// <param name="green">Green component</param>
    /// <param name="blue">Blue component</param>
    /// <param name="alpha">Alpha (transparency) component</param>
    ////////////////////////////////////////////////////////////
    public Color(float red, float green, float blue, float alpha = 1.0f)
    {
        R = (byte)(red * 255);
        G = (byte)(green * 255);
        B = (byte)(blue * 255);
        A = (byte)(alpha * 255);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Construct the color from 32-bit unsigned integer
    /// </summary>
    /// <param name="color">Number containing the RGBA components (in that order)</param>
    ////////////////////////////////////////////////////////////
    public Color(uint color)
    {
        unchecked
        {
            R = (byte)(color >> 24);
            G = (byte)(color >> 16);
            B = (byte)(color >> 8);
            A = (byte)color;
        }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Construct the color from another
    /// </summary>
    /// <param name="color">Color to copy</param>
    ////////////////////////////////////////////////////////////
    public Color(Color color) : this(color.R, color.G, color.B, color.A) { }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Convert a color to a 32-bit unsigned integer
    /// </summary>
    /// <returns>Color represented as a 32-bit unsigned integer</returns>
    ////////////////////////////////////////////////////////////
    public uint ToInteger() => (uint)((R << 24) | (G << 16) | (B << 8) | A);

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Provide a string describing the object
    /// </summary>
    /// <returns>String description of the object</returns>
    ////////////////////////////////////////////////////////////
    public override string ToString() => $"[Color] R({R}) G({G}) B({B}) A({A})";

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Compare color and object and checks if they are equal
    /// </summary>
    /// <param name="obj">Object to check</param>
    /// <returns>Object and color are equal</returns>
    ////////////////////////////////////////////////////////////
    public override bool Equals(object obj) => obj is Color color && Equals(color);

    ///////////////////////////////////////////////////////////
    /// <summary>
    /// Compare two colors and checks if they are equal
    /// </summary>
    /// <param name="other">Color to check</param>
    /// <returns>Colors are equal</returns>
    ////////////////////////////////////////////////////////////
    public bool Equals(Color other) => (R == other.R) && (G == other.G) && (B == other.B) && (A == other.A);

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Provide a integer describing the object
    /// </summary>
    /// <returns>Integer description of the object</returns>
    ////////////////////////////////////////////////////////////
    public override int GetHashCode() => (R << 24) | (G << 16) | (B << 8) | A;

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Compare two colors and checks if they are equal
    /// </summary>
    /// <returns>Colors are equal</returns>
    ////////////////////////////////////////////////////////////
    public static bool operator ==(Color left, Color right) => left.Equals(right);

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Compare two colors and checks if they are not equal
    /// </summary>
    /// <returns>Colors are not equal</returns>
    ////////////////////////////////////////////////////////////
    public static bool operator !=(Color left, Color right) => !left.Equals(right);

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// This operator returns the component-wise sum of two colors.
    /// Components that exceed 255 are clamped to 255.
    /// </summary>
    /// <returns>Result of left + right</returns>
    ////////////////////////////////////////////////////////////
    public static Color operator +(Color left, Color right)
    {
        return new Color((byte)Math.Min(left.R + right.R, 255),
                         (byte)Math.Min(left.G + right.G, 255),
                         (byte)Math.Min(left.B + right.B, 255),
                         (byte)Math.Min(left.A + right.A, 255));
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// This operator returns the component-wise subtraction of two colors.
    /// Components below 0 are clamped to 0.
    /// </summary>
    /// <returns>Result of left - right</returns>
    ////////////////////////////////////////////////////////////
    public static Color operator -(Color left, Color right)
    {
        return new Color((byte)Math.Max(left.R - right.R, 0),
                         (byte)Math.Max(left.G - right.G, 0),
                         (byte)Math.Max(left.B - right.B, 0),
                         (byte)Math.Max(left.A - right.A, 0));
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// This operator returns the component-wise multiplication of two colors.
    /// Components above 255 are clamped to 255.
    /// </summary>
    /// <returns>Result of left * right</returns>
    ////////////////////////////////////////////////////////////
    public static Color operator *(Color left, Color right)
    {
        return new Color((byte)(left.R * right.R / 255),
                         (byte)(left.G * right.G / 255),
                         (byte)(left.B * right.B / 255),
                         (byte)(left.A * right.A / 255));
    }

    /// <summary>Red component of the color</summary>
    public byte R;

    /// <summary>Green component of the color</summary>
    public byte G;

    /// <summary>Blue component of the color</summary>
    public byte B;

    /// <summary>Alpha (transparent) component of the color</summary>
    public byte A;

    /// <summary>Predefined black color</summary>
    public static readonly Color Black = new(0, 0, 0);

    /// <summary>Predefined white color</summary>
    public static readonly Color White = new(255, 255, 255);

    /// <summary>Predefined red color</summary>
    public static readonly Color Red = new(255, 0, 0);

    /// <summary>Predefined green color</summary>
    public static readonly Color Green = new(0, 255, 0);

    /// <summary>Predefined blue color</summary>
    public static readonly Color Blue = new(0, 0, 255);

    /// <summary>Predefined yellow color</summary>
    public static readonly Color Yellow = new(255, 255, 0);

    /// <summary>Predefined magenta color</summary>
    public static readonly Color Magenta = new(255, 0, 255);

    /// <summary>Predefined cyan color</summary>
    public static readonly Color Cyan = new(0, 255, 255);

    /// <summary>Predefined (black) transparent color</summary>
    public static readonly Color Transparent = new(0, 0, 0, 0);
}
