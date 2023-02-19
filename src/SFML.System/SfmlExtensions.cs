namespace SFML.System;
/// <summary>
/// Extension methods to various bits of functionality without modifying the target types
/// </summary>
public static class SfmlExtensions
{
    /// <summary>
    /// Multiply a (float, float) <paramref name="tuple"/> by a <paramref name="factor"/>
    /// </summary>
    public static (float a, float b) Mult(this (float a, float b) tuple, float factor)
    {
        return (tuple.a * factor, tuple.b * factor);
    }
}
