using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security;

using SFML.System;

namespace SFML.Audio;

////////////////////////////////////////////////////////////
/// <summary>
/// Abstract base class for streamed audio sources
/// </summary>
////////////////////////////////////////////////////////////
public abstract class SoundStream : ObjectBase
{
    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Default constructor
    /// </summary>
    ////////////////////////////////////////////////////////////
    protected SoundStream() : base(IntPtr.Zero)
    {
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>Start or resume playing the audio stream.</para>
    /// <para>
    /// This function starts the stream if it was stopped, resumes
    /// it if it was paused, and restarts it from beginning if it
    /// was it already playing.
    /// This function uses its own thread so that it doesn't block
    /// the rest of the program while the stream is played.
    /// </para>
    /// </summary>
    ////////////////////////////////////////////////////////////
    public void Play()
    {
        sfSoundStream_play(CPointer);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>Pause the audio stream.</para>
    /// <para>
    /// This function pauses the stream if it was playing,
    /// otherwise (stream already paused or stopped) it has no effect.
    /// </para>
    /// </summary>
    ////////////////////////////////////////////////////////////
    public void Pause()
    {
        sfSoundStream_pause(CPointer);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>Stop playing the audio stream.</para>
    /// <para>
    /// This function stops the stream if it was playing or paused,
    /// and does nothing if it was already stopped.
    /// It also resets the playing position (unlike pause()).
    /// </para>
    /// </summary>
    ////////////////////////////////////////////////////////////
    public void Stop()
    {
        sfSoundStream_stop(CPointer);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>Sample rate of the stream</para>
    /// <para>
    /// The sample rate is the number of audio samples played per
    /// second. The higher, the better the quality.
    /// </para>
    /// </summary>
    ////////////////////////////////////////////////////////////
    public uint SampleRate
    {
        get { return sfSoundStream_getSampleRate(CPointer); }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Number of channels (1 = mono, 2 = stereo)
    /// </summary>
    ////////////////////////////////////////////////////////////
    public uint ChannelCount
    {
        get { return sfSoundStream_getChannelCount(CPointer); }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Current status of the sound stream (see SoundStatus enum)
    /// </summary>
    ////////////////////////////////////////////////////////////
    public SoundStatus Status
    {
        get { return sfSoundStream_getStatus(CPointer); }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>Flag if the music should loop after reaching the end.</para>
    /// <para>
    /// If set, the music will restart from beginning after
    /// reaching the end and so on, until it is stopped or
    /// Loop = false is set.
    /// The default looping state for music is false.
    /// </para>
    /// </summary>
    ////////////////////////////////////////////////////////////
    public bool Loop
    {
        get { return sfSoundStream_getLoop(CPointer); }
        set { sfSoundStream_setLoop(CPointer, value); }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>Pitch of the stream.</para>
    /// <para>
    /// The pitch represents the perceived fundamental frequency
    /// of a sound; thus you can make a sound more acute or grave
    /// by changing its pitch. A side effect of changing the pitch
    /// is to modify the playing speed of the sound as well.
    /// The default value for the pitch is 1.
    /// </para>
    /// </summary>
    ////////////////////////////////////////////////////////////
    public float Pitch
    {
        get { return sfSoundStream_getPitch(CPointer); }
        set { sfSoundStream_setPitch(CPointer, value); }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>Volume of the stream.</para>
    /// <para>
    /// The volume is a value between 0 (mute) and 100 (full volume).
    /// The default value for the volume is 100.
    /// </para>
    /// </summary>
    ////////////////////////////////////////////////////////////
    public float Volume
    {
        get { return sfSoundStream_getVolume(CPointer); }
        set { sfSoundStream_setVolume(CPointer, value); }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>3D position of the stream in the audio scene.</para>
    /// <para>
    /// Only sounds with one channel (mono sounds) can be
    /// spatialized.
    /// The default position of a sound is (0, 0, 0).
    /// </para>
    /// </summary>
    ////////////////////////////////////////////////////////////
    public Vector3 Position
    {
        get { return sfSoundStream_getPosition(CPointer); }
        set { sfSoundStream_setPosition(CPointer, value); }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>Make the stream's position relative to the listener or absolute.</para>
    /// <para>
    /// Making a sound relative to the listener will ensure that it will always
    /// be played the same way regardless the position of the listener.
    /// This can be useful for non-spatialized sounds, sounds that are
    /// produced by the listener, or sounds attached to it.
    /// The default value is false (position is absolute).
    /// </para>
    /// </summary>
    ////////////////////////////////////////////////////////////
    public bool RelativeToListener
    {
        get { return sfSoundStream_isRelativeToListener(CPointer); }
        set { sfSoundStream_setRelativeToListener(CPointer, value); }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>Minimum distance of the music.</para>
    /// <para>
    /// The "minimum distance" of a sound is the maximum
    /// distance at which it is heard at its maximum volume. Further
    /// than the minimum distance, it will start to fade out according
    /// to its attenuation factor. A value of 0 ("inside the head
    /// of the listener") is an invalid value and is forbidden.
    /// The default value of the minimum distance is 1.
    /// </para>
    /// </summary>
    ////////////////////////////////////////////////////////////
    public float MinDistance
    {
        get { return sfSoundStream_getMinDistance(CPointer); }
        set { sfSoundStream_setMinDistance(CPointer, value); }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>Attenuation factor of the stream.</para>
    /// <para>
    /// The attenuation is a multiplicative factor which makes
    /// the music more or less loud according to its distance
    /// from the listener. An attenuation of 0 will produce a
    /// non-attenuated sound, i.e. its volume will always be the same
    /// whether it is heard from near or from far. On the other hand,
    /// an attenuation value such as 100 will make the sound fade out
    /// very quickly as it gets further from the listener.
    /// The default value of the attenuation is 1.
    /// </para>
    /// </summary>
    ////////////////////////////////////////////////////////////
    public float Attenuation
    {
        get { return sfSoundStream_getAttenuation(CPointer); }
        set { sfSoundStream_setAttenuation(CPointer, value); }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>Current playing position of the stream.</para>
    /// <para>
    /// The playing position can be changed when the music is
    /// either paused or playing.
    /// </para>
    /// </summary>
    ////////////////////////////////////////////////////////////
    public Time PlayingOffset
    {
        get { return sfSoundStream_getPlayingOffset(CPointer); }
        set { sfSoundStream_setPlayingOffset(CPointer, value); }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Provide a string describing the object
    /// </summary>
    /// <returns>String description of the object</returns>
    ////////////////////////////////////////////////////////////
    public override string ToString()
    {
        return "[SoundStream]" +
               " SampleRate(" + SampleRate + ")" +
               " ChannelCount(" + ChannelCount + ")" +
               " Status(" + Status + ")" +
               " Loop(" + Loop + ")" +
               " Pitch(" + Pitch + ")" +
               " Volume(" + Volume + ")" +
               " Position(" + Position + ")" +
               " RelativeToListener(" + RelativeToListener + ")" +
               " MinDistance(" + MinDistance + ")" +
               " Attenuation(" + Attenuation + ")" +
               " PlayingOffset(" + PlayingOffset + ")";
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Set the audio stream parameters, you must call it before Play()
    /// </summary>
    /// <param name="channelCount">Number of channels</param>
    /// <param name="sampleRate">Sample rate, in samples per second</param>
    ////////////////////////////////////////////////////////////
    protected void Initialize(uint channelCount, uint sampleRate)
    {
        _myGetDataCallback = new GetDataCallbackType(GetData);
        _mySeekCallback = new SeekCallbackType(Seek);
        CPointer = sfSoundStream_create(_myGetDataCallback, _mySeekCallback, channelCount, sampleRate, IntPtr.Zero);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Virtual function called each time new audio data is needed to feed the stream
    /// </summary>
    /// <param name="samples">Array of samples to fill for the stream</param>
    /// <returns>True to continue playback, false to stop</returns>
    ////////////////////////////////////////////////////////////
    protected abstract bool OnGetData(out short[] samples);

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Virtual function called to seek into the stream
    /// </summary>
    /// <param name="timeOffset">New position</param>
    ////////////////////////////////////////////////////////////
    protected abstract void OnSeek(Time timeOffset);

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Handle the destruction of the object
    /// </summary>
    /// <param name="disposing">Is the GC disposing the object, or is it an explicit call ?</param>
    ////////////////////////////////////////////////////////////
    protected override void Destroy(bool disposing)
    {
        sfSoundStream_destroy(CPointer);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Structure mapping the C library arguments passed to the data callback
    /// </summary>
    ////////////////////////////////////////////////////////////
    [StructLayout(LayoutKind.Sequential)]
    private struct Chunk
    {
#pragma warning disable IDE1006 // Naming Styles
        public unsafe short* samples;
        public uint sampleCount;
#pragma warning restore IDE1006 // Naming Styles
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Called each time new audio data is needed to feed the stream
    /// </summary>
    /// <param name="dataChunk">Data chunk to fill with new audio samples</param>
    /// <param name="userData">User data -- unused</param>
    /// <returns>True to continue playback, false to stop</returns>
    ////////////////////////////////////////////////////////////
    private bool GetData(ref Chunk dataChunk, IntPtr userData)
    {
        if (OnGetData(out _myTempBuffer))
        {
            unsafe
            {
                fixed (short* samplesPtr = _myTempBuffer)
                {
                    dataChunk.samples = samplesPtr;
                    dataChunk.sampleCount = (uint)_myTempBuffer.Length;
                }
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Called to seek in the stream
    /// </summary>
    /// <param name="timeOffset">New position</param>
    /// <param name="userData">User data -- unused</param>
    /// <returns>If false is returned, the playback is aborted</returns>
    ////////////////////////////////////////////////////////////
    private void Seek(Time timeOffset, IntPtr userData)
    {
        OnSeek(timeOffset);
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate bool GetDataCallbackType(ref Chunk dataChunk, IntPtr userData);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void SeekCallbackType(Time timeOffset, IntPtr userData);

    private GetDataCallbackType _myGetDataCallback;
    private SeekCallbackType _mySeekCallback;
    private short[] _myTempBuffer;

    #region Imports
    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern IntPtr sfSoundStream_create(GetDataCallbackType onGetData, SeekCallbackType onSeek, uint channelCount, uint sampleRate, IntPtr userData);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfSoundStream_destroy(IntPtr soundStreamStream);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfSoundStream_play(IntPtr soundStream);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfSoundStream_pause(IntPtr soundStream);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfSoundStream_stop(IntPtr soundStream);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern SoundStatus sfSoundStream_getStatus(IntPtr soundStream);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern uint sfSoundStream_getChannelCount(IntPtr soundStream);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern uint sfSoundStream_getSampleRate(IntPtr soundStream);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfSoundStream_setLoop(IntPtr soundStream, bool loop);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfSoundStream_setPitch(IntPtr soundStream, float pitch);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfSoundStream_setVolume(IntPtr soundStream, float volume);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfSoundStream_setPosition(IntPtr soundStream, Vector3 position);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfSoundStream_setRelativeToListener(IntPtr soundStream, bool relative);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfSoundStream_setMinDistance(IntPtr soundStream, float minDistance);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfSoundStream_setAttenuation(IntPtr soundStream, float attenuation);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfSoundStream_setPlayingOffset(IntPtr soundStream, Time timeOffset);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern bool sfSoundStream_getLoop(IntPtr soundStream);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern float sfSoundStream_getPitch(IntPtr soundStream);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern float sfSoundStream_getVolume(IntPtr soundStream);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern Vector3 sfSoundStream_getPosition(IntPtr soundStream);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern bool sfSoundStream_isRelativeToListener(IntPtr soundStream);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern float sfSoundStream_getMinDistance(IntPtr soundStream);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern float sfSoundStream_getAttenuation(IntPtr soundStream);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern Time sfSoundStream_getPlayingOffset(IntPtr soundStream);
    #endregion
}
