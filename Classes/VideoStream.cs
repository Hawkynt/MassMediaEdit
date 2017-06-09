namespace Classes {

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

  public class VideoStream : MediaStream {


    internal VideoStream(SectionDictionary values) : base(values) {
    }


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
    public string Format => this.GetStringOrDefault("format");
    public int WidthInPixels => this.GetIntOrDefault("width");
    public int HeightInPixels => this.GetIntOrDefault("height");
    public double FramesPerSecond => this.GetDoubleOrDefault("frame rate");
  }
}
