namespace Classes;

public enum StereoscopicMode {
  None = 0,
  SideBySideLeftFirst = 1,
  SideBySideRightFirst = 11,
  HorizontalOverUnderLeftFirst = 3,
  HorizontalOverUnderRightFirst = 2,
  CheckerboardLeftFirst = 5,
  CheckerboardRightFirst = 4,
  RowInterleavedLeftFirst = 7,
  RowInterleavedRightFirst = 6,
  ColumnInterleavedLeftFirst = 9,
  ColumnInterleavedRightFirst = 8,
  AnaglyphCyanRed = 10,
  AnaglyphGreenMagenta = 12,
  Unknown,
}

/// <summary>
/// Represents the frame rate mode of a video stream.
/// </summary>
public enum FrameRateMode {
  Unknown,
  Constant,
  Variable
}

/// <summary>
/// Represents the scan type of a video stream.
/// </summary>
public enum ScanType {
  Unknown,
  Progressive,
  Interlaced
}

public class VideoStream : MediaStream {

  internal VideoStream(SectionDictionary values) : base(values) {
  }

  #region Dimensions

  /// <summary>Gets the width in pixels.</summary>
  public int WidthInPixels => this.GetSomeIntOrDefault("width");
  
  /// <summary>Gets the height in pixels.</summary>
  public int HeightInPixels => this.GetSomeIntOrDefault("height");
  
  /// <summary>Gets the stored height in pixels (may differ from display height due to padding).</summary>
  public int StoredHeightInPixels => this.GetSomeIntOrDefault("stored_height");
  
  /// <summary>Gets the sampled width in pixels.</summary>
  public int SampledWidthInPixels => this.GetSomeIntOrDefault("sampled_width");
  
  /// <summary>Gets the sampled height in pixels.</summary>
  public int SampledHeightInPixels => this.GetSomeIntOrDefault("sampled_height");
  
  /// <summary>Gets the pixel aspect ratio.</summary>
  public double PixelAspectRatio => this.GetDoubleOrDefault("pixel aspect ratio", defaultValue: 1.0);
  
  /// <summary>Gets the display aspect ratio as a decimal value.</summary>
  public double DisplayAspectRatio => this.GetDoubleOrDefault("display aspect ratio");
  
  /// <summary>Gets the display aspect ratio as a string (e.g., "16:9").</summary>
  public string DisplayAspectRatioString => this.GetStringOrDefault("display aspect ratio", 1);

  #endregion

  #region Frame Rate

  /// <summary>Gets the frame rate in frames per second.</summary>
  public double FramesPerSecond => this.GetDoubleOrDefault("frame rate");
  
  /// <summary>Gets the frame rate mode (Constant or Variable).</summary>
  public FrameRateMode FrameRateModeValue {
    get {
      var mode = this.GetStringOrDefault("frame rate mode")?.ToLowerInvariant();
      return mode switch {
        "cfr" or "constant" => FrameRateMode.Constant,
        "vfr" or "variable" => FrameRateMode.Variable,
        _ => FrameRateMode.Unknown
      };
    }
  }
  
  /// <summary>Gets the original frame rate mode before any conversion.</summary>
  public string OriginalFrameRateMode => this.GetStringOrDefault("framerate_mode_original");
  
  /// <summary>Gets the frame rate numerator.</summary>
  public int FrameRateNumerator => this.GetIntOrDefault("framerate_num");
  
  /// <summary>Gets the frame rate denominator.</summary>
  public int FrameRateDenominator => this.GetIntOrDefault("framerate_den");

  #endregion

  #region Format Details
  
  /// <summary>Gets the format profile (e.g., "High@L3" for AVC).</summary>
  public string FormatProfile => this.GetStringOrDefault("format profile");
  
  /// <summary>Gets the format settings description.</summary>
  public string FormatSettings => this.GetStringOrDefault("format settings");
  
  /// <summary>Gets whether CABAC is used (AVC specific).</summary>
  public bool UsesCabac => this.GetBoolOrDefault("format settings, cabac");
  
  /// <summary>Gets the number of reference frames.</summary>
  public int ReferenceFrames => this.GetSomeIntOrDefault("format settings, reference frames");
  
  /// <summary>Gets the internet media type.</summary>
  public string InternetMediaType => this.GetStringOrDefault("internet media type");

  #endregion

  #region Color Information

  /// <summary>Gets the color space (e.g., "YUV").</summary>
  public string ColorSpace => this.GetStringOrDefault("color space");
  
  /// <summary>Gets the chroma subsampling (e.g., "4:2:0").</summary>
  public string ChromaSubsampling => this.GetStringOrDefault("chroma subsampling");
  
  /// <summary>Gets the bit depth in bits.</summary>
  public int BitDepth => this.GetSomeIntOrDefault("bit depth");
  
  /// <summary>Gets the color range (Limited or Full).</summary>
  public string ColorRange => this.GetStringOrDefault("color range");
  
  /// <summary>Gets whether color description is present in the stream.</summary>
  public bool HasColorDescription => this.GetBoolOrDefault("colour_description_present");
  
  /// <summary>Gets the matrix coefficients (e.g., "BT.709").</summary>
  public string MatrixCoefficients => this.GetStringOrDefault("matrix coefficients");

  #endregion

  #region Scan Type

  /// <summary>Gets the scan type (Progressive or Interlaced).</summary>
  public ScanType ScanTypeValue {
    get {
      var type = this.GetStringOrDefault("scan type")?.ToLowerInvariant();
      return type switch {
        "progressive" => ScanType.Progressive,
        "interlaced" => ScanType.Interlaced,
        _ => ScanType.Unknown
      };
    }
  }

  #endregion

  #region Encoding Information

  /// <summary>Gets the bits per pixel per frame.</summary>
  public double BitsPerPixelFrame => this.GetDoubleOrDefault("bits/(pixel*frame)");
  
  /// <summary>Gets the writing/encoding library.</summary>
  public string WritingLibrary => this.GetStringOrDefault("writing library");
  
  /// <summary>Gets the encoding library name.</summary>
  public string EncodingLibraryName => this.GetStringOrDefault("encoded_library_name");
  
  /// <summary>Gets the encoding library version.</summary>
  public string EncodingLibraryVersion => this.GetStringOrDefault("encoded_library_version");
  
  /// <summary>Gets the full encoding settings string.</summary>
  public string EncodingSettings => this.GetStringOrDefault("encoding settings");

  #endregion

  #region Stereoscopic 3D

  public StereoscopicMode StereoscopicMode {
    get {
      if (!this.IsStereoscopic)
        return StereoscopicMode.None;
      /*
      Stereo-3D video mode (
        0: mono,
        1: side by side (left eye is first),
        2: top-bottom (right eye is first),
        3: top-bottom (left eye is first),
        4: checkboard (right is first),
        5: checkboard (left is first),
        6: row interleaved (right is first),
        7: row interleaved (left is first),
        8: column interleaved (right is first),
        9: column interleaved (left is first),
        10: anaglyph (cyan/red),
        11: side by side (right eye is first),
        12: anaglyph (green/magenta),
        13 both eyes laced in one Block (left eye is first),
        14 both eyes laced in one Block (right eye is first)
      ).
      */
      var type = (this.GetStringOrDefault("MultiView_Layout") ?? string.Empty).ToLowerInvariant();
      if (type.Contains("side"))
        return type.Contains("left")
          ? StereoscopicMode.SideBySideLeftFirst
          : type.Contains("right") ? StereoscopicMode.SideBySideRightFirst : StereoscopicMode.Unknown;

      if (type.Contains("top-bottom"))
        return type.Contains("left")
          ? StereoscopicMode.HorizontalOverUnderLeftFirst
          : type.Contains("right") ? StereoscopicMode.HorizontalOverUnderRightFirst : StereoscopicMode.Unknown;

      if (type.Contains("checkboard"))
        return type.Contains("left")
          ? StereoscopicMode.CheckerboardLeftFirst
          : type.Contains("right") ? StereoscopicMode.CheckerboardRightFirst : StereoscopicMode.Unknown;

      if (type.Contains("anaglyph"))
        return type.Contains("cyan")
          ? StereoscopicMode.AnaglyphCyanRed
          : type.Contains("green") ? StereoscopicMode.AnaglyphGreenMagenta : StereoscopicMode.Unknown;

      if (type.Contains("interleaved")) {
        if (type.Contains("row"))
          return type.Contains("left")
            ? StereoscopicMode.RowInterleavedLeftFirst
            : type.Contains("right") ? StereoscopicMode.RowInterleavedRightFirst : StereoscopicMode.Unknown;

        if (type.Contains("column"))
          return type.Contains("left")
            ? StereoscopicMode.ColumnInterleavedLeftFirst
            : type.Contains("right") ? StereoscopicMode.ColumnInterleavedRightFirst : StereoscopicMode.Unknown;

        return StereoscopicMode.Unknown;
      }

      return StereoscopicMode.Unknown;
    }
  }

  public bool IsStereoscopic => this.GetIntOrDefault("MultiView_Count") == 2;

  #endregion
}