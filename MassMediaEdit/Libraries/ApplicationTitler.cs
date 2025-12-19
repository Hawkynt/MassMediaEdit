using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
#if NETFX_4
#endif
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;

namespace System.Windows.Forms {
  /// <summary>
  /// This class allows automatically setting window and console titles to the applications' name and version number.
  /// </summary>
  internal static class ApplicationTitler {
    /// <summary>
    /// Sets the form title to the current application's version and product name.
    /// </summary>
    /// <param name="this">This Form.</param>
    public static void SetFormTitle(this Form @this) {
      if (@this == null)
        throw new NullReferenceException();

      @this.Text = System.ApplicationTitler.Title;
      var icon = System.ApplicationTitler.Icon;
      if (icon != null)
        @this.Icon = icon;

    }

    /// <summary>
    /// Sets the icon tooltip.
    /// </summary>
    /// <param name="this">The this.</param>
    public static void SetIconTooltip(this NotifyIcon @this) {
      if (@this == null)
        throw new NullReferenceException();

      @this.Text = System.ApplicationTitler.Title;
    }
  }
}

namespace System {

  /// <summary>
  /// This class allows modification of the console title and icon according to the executables' information.
  /// </summary>
  internal static class ApplicationTitler {

    /// <summary>
    /// Holds and restores the old state of the console (title+icon).
    /// </summary>
    private class InitialConsoleState {
      /// <summary>
      /// P/Invoke methods
      /// </summary>
      private static class NativeMethods {

        public enum WindowMessageType : uint {
          GetIcon = 0x7f,
          SetIcon = 0x80,
        }

        public enum IconType : uint {
          Small = 0,
          Big = 1,
        }

        public enum IndexType : int {
          Icon = -14,
          SmallIcon = -34,
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, WindowMessageType Msg, IconType wParam, IntPtr lParam);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetConsoleWindow();

        public static IntPtr GetClassLongPointer(IntPtr hWnd, IndexType nIndex) {
          return IntPtr.Size > 4
            ? _GetClassLongPtr64(hWnd, nIndex)
            : new IntPtr(_GetClassLongPtr32(hWnd, nIndex));
        }

        [DllImport("user32.dll", EntryPoint = "GetClassLong", SetLastError = true)]
        private static extern uint _GetClassLongPtr32(IntPtr hWnd, IndexType nIndex);

        [DllImport("user32.dll", EntryPoint = "GetClassLongPtr", SetLastError = true)]
        private static extern IntPtr _GetClassLongPtr64(IntPtr hWnd, IndexType nIndex);
      }

      /// <summary>
      /// The keeps the old console title from program startup.
      /// </summary>
      private readonly string _initialTitle;

      /// <summary>
      /// The native console handle
      /// </summary>
      private readonly IntPtr _consoleHandle;

      /// <summary>
      /// The small console image
      /// </summary>
      private readonly IntPtr _smallImage;

      /// <summary>
      /// The large console image
      /// </summary>
      private readonly IntPtr _largeImage;

      public InitialConsoleState() {
        this._initialTitle = Console.Title;

        var handle = this._consoleHandle = NativeMethods.GetConsoleWindow();
        if (handle != IntPtr.Zero) {
          this._smallImage = NativeMethods.SendMessage(handle, NativeMethods.WindowMessageType.GetIcon, NativeMethods.IconType.Small, IntPtr.Zero);
          if (this._smallImage == IntPtr.Zero)
            this._smallImage = NativeMethods.GetClassLongPointer(handle, NativeMethods.IndexType.SmallIcon);

          this._largeImage = NativeMethods.SendMessage(handle, NativeMethods.WindowMessageType.GetIcon, NativeMethods.IconType.Big, IntPtr.Zero);
          if (this._largeImage == IntPtr.Zero)
            this._largeImage = NativeMethods.GetClassLongPointer(handle, NativeMethods.IndexType.Icon);

        }

        // register to change back the title on program end
        AppDomain.CurrentDomain.ProcessExit += (_, __) => this._RestoreOldConsoleTitle();
        AppDomain.CurrentDomain.DomainUnload += (_, __) => this._RestoreOldConsoleTitle();
      }

      public Icon Icon {
        set {
          if (value == null || this._consoleHandle == IntPtr.Zero)
            return;

          NativeMethods.SendMessage(this._consoleHandle, NativeMethods.WindowMessageType.SetIcon, NativeMethods.IconType.Small, value.Handle);
          NativeMethods.SendMessage(this._consoleHandle, NativeMethods.WindowMessageType.SetIcon, NativeMethods.IconType.Big, value.Handle);
        }
      }

      public string Title { set { Console.Title = value; } }

      /// <summary>
      /// Restores the old console title.
      /// </summary>
      private void _RestoreOldConsoleTitle() {
        try {
          Console.Title = this._initialTitle;
          if (this._consoleHandle == IntPtr.Zero)
            return;

          NativeMethods.SendMessage(this._consoleHandle, NativeMethods.WindowMessageType.SetIcon, NativeMethods.IconType.Small, this._smallImage);
          NativeMethods.SendMessage(this._consoleHandle, NativeMethods.WindowMessageType.SetIcon, NativeMethods.IconType.Big, this._largeImage);

        } catch (Exception) {
          ;
        }
      }
    }

    /// <summary>
    /// Determines whether it is beta build or not.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if it is beta build; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsBetaBuild {
      get {
#if DEBUG
        return true;
#else
        return false;
#endif
      }
    }

    /// <summary>
    /// Caches the product name.
    /// </summary>
    private static string _cacheProductName;

    /// <summary>
    /// Gets the name of the product.
    /// </summary>
    /// <returns></returns>
    public static string ProductName {
      get {

        // return from cache if possible
        if (_cacheProductName != null)
          return _cacheProductName;

        var assembly = Assembly.GetEntryAssembly();

        // query assembly product
        var attribute = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false).FirstOrDefault();
        if (attribute != null) {
          var result = ((AssemblyProductAttribute)attribute).Product;
          if (result != null && result.Trim().Length > 0)
            return _cacheProductName = result;
        }

        var mainType = assembly.EntryPoint.ReflectedType;

        // win32 version info
        {
          var result = FileVersionInfo.GetVersionInfo(mainType.Module.FullyQualifiedName).ProductName;
          if (result != null && result.Trim().Length > 0)
            return _cacheProductName = result;
        }

        // query assembly title
        attribute = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false).FirstOrDefault();
        if (attribute != null) {
          var result = ((AssemblyTitleAttribute)attribute).Title;
          if (result != null && result.Trim().Length > 0)
            return _cacheProductName = result;
        }

        // fake with namespace
        var ns = mainType.Namespace;

        // last ditch... use the main type
        if (string.IsNullOrEmpty(ns))
          return _cacheProductName = mainType.Name;

        var lastDot = ns.LastIndexOf(".");
        return _cacheProductName = lastDot != -1 && lastDot < ns.Length - 1 ? ns.Substring(lastDot + 1) : ns;
      }
    }

    /// <summary>
    /// Caches the version.
    /// </summary>
    private static string _cacheVersion;

    /// <summary>
    /// Gets the version.
    /// </summary>
    /// <returns></returns>
    public static string Version {
      get {

        // return from cache if possible
        if (_cacheVersion != null)
          return _cacheVersion;

        var assembly = Assembly.GetEntryAssembly();

        // query assembly version
        var attribute = assembly.GetCustomAttributes(typeof(AssemblyVersionAttribute), false).FirstOrDefault();
        if (attribute != null) {
          var result = ((AssemblyVersionAttribute)attribute).Version;
          if (result != null && result.Trim().Length > 0)
            return _cacheVersion = result;
        }

        // query assembly informational version
        attribute = assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false).FirstOrDefault();
        if (attribute != null) {
          var result = ((AssemblyInformationalVersionAttribute)attribute).InformationalVersion;
          if (result != null && result.Trim().Length > 0)
            return _cacheVersion = result;
        }

        var mainType = assembly.EntryPoint.ReflectedType;

        // win32 version info
        {
          var result = FileVersionInfo.GetVersionInfo(mainType.Module.FullyQualifiedName).ProductVersion;
          if (result != null && result.Trim().Length > 0)
            return _cacheVersion = result;
        }

        // query assembly title
        attribute = assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false).FirstOrDefault();
        if (attribute != null) {
          var result = ((AssemblyFileVersionAttribute)attribute).Version;
          if (result != null && result.Trim().Length > 0)
            return _cacheVersion = result;
        }

        return _cacheVersion = "1.0.0.0";
      }
    }

    /// <summary>
    /// Gets the title to use for windows.
    /// </summary>
    public static string Title {
      get {
        var version = Version;
        var split = version.Split('+')[0].Split('.');

        var splitLength = split.Length;
        var result = string.Format(
          "{0} v{1}.{2}.{3}.{4} {5}",
          ProductName,
          split[0],
          splitLength > 1 ? split[1] : "0",
          _GetStaticFieldOrDefault("RepositoryVersion.CheckoutVersion", o => (ulong)o / 100, splitLength > 2 ? ulong.Parse(split[2]) : 0),
          _GetStaticFieldOrDefault("RepositoryVersion.CheckoutVersion", o => (ulong)o % 100, splitLength > 3 ? ulong.Parse(split[3]) : 0),
          IsBetaBuild ? "(beta)" : string.Empty
        );

        return result;
      }
    }

    private static Icon _cacheIcon;
    /// <summary>
    /// Tries to get the icon of the currently running executable.
    /// </summary>
    /// <value>
    /// The icon.
    /// </value>
    public static Icon Icon {
      get {
        var result = _cacheIcon;
        if (result != null)
          return result;

        try {
          result = ExtractAssociatedIcon(Assembly.GetEntryAssembly().Location);
          return _cacheIcon = result;
        } catch (Exception) {
          return null;
        }
      }
    }


    /// <summary>
    /// The old state of the conosle on startup.
    /// </summary>
    private static InitialConsoleState _oldState;

    /// <summary>
    /// Sets the console title to the current application's version and product name.
    /// </summary>
    public static void SetConsoleTitle() {
      if (_oldState == null)
        _oldState = new InitialConsoleState();

      _oldState.Title = Title;
      var icon = Icon;
      if (icon != null)
        _oldState.Icon = icon;

    }

    /// <summary>
    /// Tries to get a certain static field using reflection.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="fullFieldName">Full name of the field.</param>
    /// <param name="converter">The converter.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>The value from the converter or the default value.</returns>
    private static TResult _GetStaticFieldOrDefault<TResult>(string fullFieldName, Func<object, TResult> converter, TResult defaultValue = default) {
      if (fullFieldName == null || fullFieldName.Trim().Length < 1)
        throw new ArgumentNullException(nameof(fullFieldName));

      if (converter == null)
        throw new ArgumentNullException(nameof(converter));

      var parts = fullFieldName.Split('.');

      // malformed
      if (parts.Length < 2)
        return defaultValue;

      var typeName = string.Join(".", parts.Take(parts.Length - 1).ToArray());
      var fieldName = parts[parts.Length - 1];
      var type = Type.GetType(typeName);

      // type not found
      if (type == null)
        return defaultValue;

      var field = type.GetField(fieldName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

      // field not found
      if (field == null)
        return defaultValue;

      try {
        var value = field.GetValue(null);
        return converter(value);

      } catch (Exception) {

        // reading field crashed or converting value crashed
        return defaultValue;
      }
    }


    /// <summary>
    /// Returns an icon representation of an image contained in the specified file.
    /// This function is identical to System.Drawing.Icon.ExtractAssociatedIcon, xcept this version works.
    /// </summary>
    /// <param name="filePath">The path to the file that contains an image.</param>
    /// <returns>The System.Drawing.Icon representation of the image contained in the specified file.</returns>
    /// <exception cref="System.ArgumentException">filePath does not indicate a valid file.</exception>
    public static Icon ExtractAssociatedIcon(string filePath) {
      if (filePath == null)
        throw new ArgumentNullException(nameof(filePath));

      var index = 0;
      Uri uri;
      try {
        uri = new Uri(filePath);
      } catch (UriFormatException) {
        filePath = Path.GetFullPath(filePath);
        uri = new Uri(filePath);
      }

      if (!uri.IsFile)
        return null;

      if (!File.Exists(filePath))
        throw new FileNotFoundException(filePath);

      var iconPath = new StringBuilder(260);
      iconPath.Append(filePath);

      var handle = SafeNativeMethods.ExtractAssociatedIcon(new HandleRef(null, IntPtr.Zero), iconPath, ref index);
      return handle == IntPtr.Zero ? null : Icon.FromHandle(handle);
    }


    /// <summary>
    /// This class suppresses stack walks for unmanaged code permission.
    /// (System.Security.SuppressUnmanagedCodeSecurityAttribute is applied to this class.)
    /// This class is for methods that are safe for anyone to call.
    /// Callers of these methods are not required to perform a full security review to make sure that the
    /// usage is secure because the methods are harmless for any caller.
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    private static class SafeNativeMethods {
      [DllImport("shell32.dll", EntryPoint = @"ExtractAssociatedIcon", CharSet = CharSet.Auto)]
      public static extern IntPtr ExtractAssociatedIcon(HandleRef hInst, StringBuilder iconPath, ref int index);
    }
  }
}
