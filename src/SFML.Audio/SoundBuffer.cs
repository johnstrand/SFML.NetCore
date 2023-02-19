using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

using SFML.System;

namespace SFML.Audio;

////////////////////////////////////////////////////////////
/// <summary>
/// Storage for audio samples defining a sound
/// </summary>
////////////////////////////////////////////////////////////
public class SoundBuffer : ObjectBase
{
    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>Construct a sound buffer from a file</para>
    /// <para>
    /// Here is a complete list of all the supported audio formats:
    /// ogg, wav, flac, aiff, au, raw, paf, svx, nist, voc, ircam,
    /// w64, mat4, mat5 pvf, htk, sds, avr, sd2, caf, wve, mpc2k, rf64.
    /// </para>
    /// </summary>
    /// <param name="filename">Path of the sound file to load</param>
    /// <exception cref="LoadingFailedException" />
    ////////////////////////////////////////////////////////////
    public SoundBuffer(string filename) :
        base(sfSoundBuffer_createFromFile(filename))
    {
        if (CPointer == IntPtr.Zero)
        {
            throw new LoadingFailedException("sound buffer", filename);
        }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>Construct a sound buffer from a custom stream.</para>
    /// <para>
    /// Here is a complete list of all the supported audio formats:
    /// ogg, wav, flac, aiff, au, raw, paf, svx, nist, voc, ircam,
    /// w64, mat4, mat5 pvf, htk, sds, avr, sd2, caf, wve, mpc2k, rf64.
    /// </para>
    /// </summary>
    /// <param name="stream">Source stream to read from</param>
    /// <exception cref="LoadingFailedException" />
    ////////////////////////////////////////////////////////////
    public SoundBuffer(Stream stream) :
        base(IntPtr.Zero)
    {
        using (StreamAdaptor adaptor = new(stream))
        {
            CPointer = sfSoundBuffer_createFromStream(adaptor.InputStreamPtr);
        }

        if (CPointer == IntPtr.Zero)
        {
            throw new LoadingFailedException("sound buffer");
        }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>Construct a sound buffer from a file in memory.</para>
    /// <para>
    /// Here is a complete list of all the supported audio formats:
    /// ogg, wav, flac, aiff, au, raw, paf, svx, nist, voc, ircam,
    /// w64, mat4, mat5 pvf, htk, sds, avr, sd2, caf, wve, mpc2k, rf64.
    /// </para>
    /// </summary>
    /// <param name="bytes">Byte array containing the file contents</param>
    /// <exception cref="LoadingFailedException" />
    ////////////////////////////////////////////////////////////
    public SoundBuffer(byte[] bytes) :
        base(IntPtr.Zero)
    {
        var pin = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try
        {
            CPointer = sfSoundBuffer_createFromMemory(pin.AddrOfPinnedObject(), Convert.ToUInt64(bytes.Length));
        }
        finally
        {
            pin.Free();
        }
        if (CPointer == IntPtr.Zero)
        {
            throw new LoadingFailedException("sound buffer");
        }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Construct a sound buffer from an array of samples
    /// </summary>
    /// <param name="samples">Array of samples</param>
    /// <param name="channelCount">Channel count</param>
    /// <param name="sampleRate">Sample rate</param>
    /// <exception cref="LoadingFailedException" />
    ////////////////////////////////////////////////////////////
    public SoundBuffer(short[] samples, uint channelCount, uint sampleRate) :
        base(IntPtr.Zero)
    {
        unsafe
        {
            fixed (short* samplesPtr = samples)
            {
                CPointer = sfSoundBuffer_createFromSamples(samplesPtr, (uint)samples.Length, channelCount, sampleRate);
            }
        }

        if (CPointer == IntPtr.Zero)
        {
            throw new LoadingFailedException("sound buffer");
        }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Construct a sound buffer from another sound buffer
    /// </summary>
    /// <param name="copy">Sound buffer to copy</param>
    ////////////////////////////////////////////////////////////
    public SoundBuffer(SoundBuffer copy) :
        base(sfSoundBuffer_copy(copy.CPointer))
    {
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>Save the sound buffer to an audio file.</para>
    /// <para>
    /// Here is a complete list of all the supported audio formats:
    /// ogg, wav, flac, aiff, au, raw, paf, svx, nist, voc, ircam,
    /// w64, mat4, mat5 pvf, htk, sds, avr, sd2, caf, wve, mpc2k, rf64.
    /// </para>
    /// </summary>
    /// <param name="filename">Path of the sound file to write</param>
    /// <returns>True if saving has been successful</returns>
    ////////////////////////////////////////////////////////////
    public bool SaveToFile(string filename)
    {
        return sfSoundBuffer_saveToFile(CPointer, filename);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>Sample rate of the sound buffer.</para>
    /// <para>
    /// The sample rate is the number of audio samples played per
    /// second. The higher, the better the quality.
    /// </para>
    /// </summary>
    ////////////////////////////////////////////////////////////
    public uint SampleRate
    {
        get { return sfSoundBuffer_getSampleRate(CPointer); }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Number of channels (1 = mono, 2 = stereo)
    /// </summary>
    ////////////////////////////////////////////////////////////
    public uint ChannelCount
    {
        get { return sfSoundBuffer_getChannelCount(CPointer); }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Total duration of the buffer
    /// </summary>
    ////////////////////////////////////////////////////////////
    public Time Duration
    {
        get { return sfSoundBuffer_getDuration(CPointer); }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>Array of audio samples stored in the buffer.</para>
    /// <para>
    /// The format of the returned samples is 16 bits signed integer
    /// (sf::Int16).
    /// </para>
    /// </summary>
    ////////////////////////////////////////////////////////////
    public short[] Samples
    {
        get
        {
            var samplesArray = new short[sfSoundBuffer_getSampleCount(CPointer)];
            Marshal.Copy(sfSoundBuffer_getSamples(CPointer), samplesArray, 0, samplesArray.Length);
            return samplesArray;
        }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Provide a string describing the object
    /// </summary>
    /// <returns>String description of the object</returns>
    ////////////////////////////////////////////////////////////
    public override string ToString()
    {
        return "[SoundBuffer]" +
               " SampleRate(" + SampleRate + ")" +
               " ChannelCount(" + ChannelCount + ")" +
               " Duration(" + Duration + ")";
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Handle the destruction of the object
    /// </summary>
    /// <param name="disposing">Is the GC disposing the object, or is it an explicit call ?</param>
    ////////////////////////////////////////////////////////////
    protected override void Destroy(bool disposing)
    {
        sfSoundBuffer_destroy(CPointer);
    }

    #region Imports
    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode), SuppressUnmanagedCodeSecurity]
    private static extern IntPtr sfSoundBuffer_createFromFile(string filename);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern unsafe IntPtr sfSoundBuffer_createFromStream(IntPtr stream);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern unsafe IntPtr sfSoundBuffer_createFromMemory(IntPtr data, ulong size);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern unsafe IntPtr sfSoundBuffer_createFromSamples(short* samples, ulong sampleCount, uint channelsCount, uint sampleRate);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern IntPtr sfSoundBuffer_copy(IntPtr soundBuffer);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfSoundBuffer_destroy(IntPtr soundBuffer);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode), SuppressUnmanagedCodeSecurity]
    private static extern bool sfSoundBuffer_saveToFile(IntPtr soundBuffer, string filename);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern IntPtr sfSoundBuffer_getSamples(IntPtr soundBuffer);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern ulong sfSoundBuffer_getSampleCount(IntPtr soundBuffer);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern uint sfSoundBuffer_getSampleRate(IntPtr soundBuffer);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern uint sfSoundBuffer_getChannelCount(IntPtr soundBuffer);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern Time sfSoundBuffer_getDuration(IntPtr soundBuffer);
    #endregion
}
