using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

using SFML.System;

namespace SFML.Window;

//////////////////////////////////////////////////////////////////
/// <summary>
/// This class defines a .NET interface to an SFML OpenGL Context
/// </summary>
//////////////////////////////////////////////////////////////////
public class Context : CriticalFinalizerObject
{
    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Default constructor
    /// </summary>
    ////////////////////////////////////////////////////////////
    public Context()
    {
        _myThis = sfContext_create();
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Finalizer
    /// </summary>
    ////////////////////////////////////////////////////////////
    ~Context()
    {
        sfContext_destroy(_myThis);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Activate or deactivate the context
    /// </summary>
    /// <param name="active">True to activate, false to deactivate</param>
    /// <returns>true on success, false on failure</returns>
    ////////////////////////////////////////////////////////////
    public bool SetActive(bool active)
    {
        return sfContext_setActive(_myThis, active);
    }

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Get the settings of the context.
    /// </summary>
    ////////////////////////////////////////////////////////////
    public ContextSettings Settings => sfContext_getSettings(_myThis);

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Global helper context
    /// </summary>
    ////////////////////////////////////////////////////////////
    public static Context Global => s_ourGlobalContext ??= new Context();

    ////////////////////////////////////////////////////////////
    /// <summary>
    /// Provide a string describing the object
    /// </summary>
    /// <returns>String description of the object</returns>
    ////////////////////////////////////////////////////////////
    public override string ToString()
    {
        return "[Context]";
    }

    private static Context s_ourGlobalContext;

    private readonly IntPtr _myThis = IntPtr.Zero;

    #region Imports
    [DllImport(CSFML.window, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern IntPtr sfContext_create();

    [DllImport(CSFML.window, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern void sfContext_destroy(IntPtr view);

    [DllImport(CSFML.window, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern bool sfContext_setActive(IntPtr view, bool active);

    [DllImport(CSFML.window, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
    private static extern ContextSettings sfContext_getSettings(IntPtr view);
    #endregion
}
