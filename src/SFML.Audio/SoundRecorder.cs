using System;
using System.Runtime.InteropServices;
using System.Security;

using SFML.System;

namespace SFML.Audio;

////////////////////////////////////////////////////////////
/// <summary>
/// Base class intended for capturing sound data
/// </summary>
////////////////////////////////////////////////////////////
public abstract class SoundRecorder : ObjectBase
{
    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Default constructor
    /// </summary>
    ////////////////////////////////////////////////////////////
    protected SoundRecorder() : base(IntPtr.Zero)
    {
        _myStartCallback = new StartCallback(OnStart);
        _myProcessCallback = new ProcessCallback(ProcessSamples);
        _myStopCallback = new StopCallback(OnStop);

        CPointer = sfSoundRecorder_create(_myStartCallback, _myProcessCallback, _myStopCallback, IntPtr.Zero);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>Start the capture using the default sample rate (44100 Hz).</para>
    /// <para>Please note that only one capture can happen at the same time.</para>
    /// </summary>
    ////////////////////////////////////////////////////////////
    public bool Start()
    {
        return Start(44100);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>Start the capture.</para>
    /// <para>
    /// The sampleRate parameter defines the number of audio samples
    /// captured per second. The higher, the better the quality
    /// (for example, 44100 samples/sec is CD quality).
    /// This function uses its own thread so that it doesn't block
    /// the rest of the program while the capture runs.
    /// </para>
    /// <para>Please note that only one capture can happen at the same time.</para>
    /// </summary>
    /// <param name="sampleRate"> Sound frequency; the more samples, the higher the quality (44100 by default = CD quality)</param>
    ////////////////////////////////////////////////////////////
    public bool Start(uint sampleRate)
    {
        return sfSoundRecorder_start(CPointer, sampleRate);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Stop the capture
    /// </summary>
    ////////////////////////////////////////////////////////////
    public void Stop()
    {
        sfSoundRecorder_stop(CPointer);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Sample rate of the sound recorder.
    /// </summary>
    ///
    /// <remarks>
    /// The sample rate defines the number of audio samples
    /// captured per second. The higher, the better the quality
    /// (for example, 44100 samples/sec is CD quality).
    /// </remarks>
    ////////////////////////////////////////////////////////////
    public uint SampleRate => sfSoundRecorder_getSampleRate(CPointer);

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Get/Set the channel count of the audio capture device
    /// </summary>
    ///
    /// <remarks>
    /// This method allows you to specify the number of channels
    /// used for recording. Currently only 16-bit mono (1) and
    /// 16-bit stereo (2) are supported.
    /// </remarks>
    ////////////////////////////////////////////////////////////
    public uint ChannelCount
    {
        get { return sfSoundRecorder_getChannelCount(CPointer); }
        set { sfSoundRecorder_setChannelCount(CPointer, value); }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Check if the system supports audio capture.
    /// </summary>
    ///
    /// <remarks>
    /// This function should always be called before using
    /// the audio capture features. If it returns false, then
    /// any attempt to use the SoundRecorder or one of its derived
    /// classes will fail.
    /// </remarks>
    ////////////////////////////////////////////////////////////
    public static bool IsAvailable => sfSoundRecorder_isAvailable();

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Provide a string describing the object
    /// </summary>
    /// <returns>String description of the object</returns>
    ////////////////////////////////////////////////////////////
    public override string ToString()
    {
        return $"[SoundRecorder] SampleRate({SampleRate})";
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>Start capturing audio data.</para>
    /// <para>
    /// This virtual function may be overridden by a derived class
    /// if something has to be done every time a new capture
    /// starts. If not, this function can be ignored; the default
    /// implementation does nothing.
    /// </para>
    /// </summary>
    /// <returns>False to abort recording audio data, true to continue</returns>
    ////////////////////////////////////////////////////////////
    protected virtual bool OnStart()
    {
        // Does nothing by default
        return true;
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>Process a new chunk of recorded samples.</para>
    /// <para>
    /// This virtual function is called every time a new chunk of
    /// recorded data is available. The derived class can then do
    /// whatever it wants with it (storing it, playing it, sending
    /// it over the network, etc.).
    /// </para>
    /// </summary>
    /// <param name="samples">Array of samples to process</param>
    /// <returns>False to stop recording audio data, true to continue</returns>
    ////////////////////////////////////////////////////////////
    protected abstract bool OnProcessSamples(short[] samples);

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>Stop capturing audio data.</para>
    /// <para>
    /// This virtual function may be overridden by a derived class
    /// if something has to be done every time the capture
    /// ends. If not, this function can be ignored; the default
    /// implementation does nothing.
    /// </para>
    /// </summary>
    ////////////////////////////////////////////////////////////
    protected virtual void OnStop()
    {
        // Does nothing by default
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// <para>
    /// The processing interval controls the period
    /// between calls to the onProcessSamples function. You may
    /// want to use a small interval if you want to process the
    /// recorded data in real time, for example.
    /// </para>
    /// <para>
    /// Note: this is only a hint, the actual period may vary.
    /// So don't rely on this parameter to implement precise timing.
    /// </para>
    /// <para>The default processing interval is 100 ms.</para>
    /// </summary>
    ////////////////////////////////////////////////////////////
    protected void SetProcessingInterval(Time interval)
    {
        sfSoundRecorder_setProcessingInterval(CPointer, interval);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Get the list of the names of all available audio capture devices
    /// </summary>
    ////////////////////////////////////////////////////////////
    public static string[] AvailableDevices
    {
        get
        {
            unsafe
            {
                var devicesPtr = sfSoundRecorder_getAvailableDevices(out var count);
                var devices = new string[count];
                for (uint i = 0; i < count; ++i)
                {
                    devices[i] = Marshal.PtrToStringAnsi(devicesPtr[i]);
                }

                return devices;
            }
        }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Get the name of the default audio capture device
    /// </summary>
    ////////////////////////////////////////////////////////////
    public static string DefaultDevice
    {
        get { return Marshal.PtrToStringAnsi(sfSoundRecorder_getDefaultDevice()); }
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Set the audio capture device
    /// </summary>
    /// <param name="name">The name of the audio capture device</param>
    /// <returns>True, if it was able to set the requested device</returns>
    ////////////////////////////////////////////////////////////
    public bool SetDevice(string name)
    {
        return sfSoundRecorder_setDevice(CPointer, name);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Get the name of the current audio capture device
    /// </summary>
    /// <returns>The name of the current audio capture device</returns>
    ////////////////////////////////////////////////////////////
    public string GetDevice()
    {
        return Marshal.PtrToStringAnsi(sfSoundRecorder_getDevice(CPointer));
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Handle the destruction of the object
    /// </summary>
    /// <param name="disposing">Is the GC disposing the object, or is it an explicit call ?</param>
    ////////////////////////////////////////////////////////////
    protected override void Destroy(bool disposing)
    {
        sfSoundRecorder_destroy(CPointer);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Function called directly by the C library ; convert
    /// arguments and forward them to the internal virtual function
    /// </summary>
    /// <param name="samples">Pointer to the array of samples</param>
    /// <param name="nbSamples">Number of samples in the array</param>
    /// <param name="userData">User data -- unused</param>
    /// <returns>False to stop recording audio data, true to continue</returns>
    ////////////////////////////////////////////////////////////
    private bool ProcessSamples(IntPtr samples, uint nbSamples, IntPtr userData)
    {
        var samplesArray = new short[nbSamples];
        Marshal.Copy(samples, samplesArray, 0, samplesArray.Length);

        return OnProcessSamples(samplesArray);
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate bool StartCallback();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate bool ProcessCallback(IntPtr samples, uint nbSamples, IntPtr userData);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void StopCallback();

    private readonly StartCallback _myStartCallback;
    private readonly ProcessCallback _myProcessCallback;
    private readonly StopCallback _myStopCallback;

    #region Imports
    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern IntPtr sfSoundRecorder_create(StartCallback onStart, ProcessCallback onProcess, StopCallback onStop, IntPtr userData);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfSoundRecorder_destroy(IntPtr soundRecorder);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern bool sfSoundRecorder_start(IntPtr soundRecorder, uint sampleRate);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfSoundRecorder_stop(IntPtr soundRecorder);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern uint sfSoundRecorder_getSampleRate(IntPtr soundRecorder);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern bool sfSoundRecorder_isAvailable();

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfSoundRecorder_setProcessingInterval(IntPtr soundRecorder, Time interval);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern unsafe IntPtr* sfSoundRecorder_getAvailableDevices(out uint count);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern IntPtr sfSoundRecorder_getDefaultDevice();

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode), SuppressUnmanagedCodeSecurity]
    private static extern bool sfSoundRecorder_setDevice(IntPtr soundRecorder, string name);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern IntPtr sfSoundRecorder_getDevice(IntPtr soundRecorder);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfSoundRecorder_setChannelCount(IntPtr soundRecorder, uint channelCount);

    [DllImport(CSFML.audio, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern uint sfSoundRecorder_getChannelCount(IntPtr soundRecorder);
    #endregion
}
