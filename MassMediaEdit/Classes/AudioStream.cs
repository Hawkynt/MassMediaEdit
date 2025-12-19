namespace Classes;

/// <summary>
/// Represents the compression mode of an audio stream.
/// </summary>
public enum CompressionMode {
  Unknown,
  Lossless,
  Lossy
}

public class AudioStream : MediaStream {
  internal AudioStream(SectionDictionary values) : base(values) {
  }

  #region Channel Information

  /// <summary>Gets the number of audio channels.</summary>
  public int Channels => this.GetSomeIntOrDefault("channel(s)");
  
  /// <summary>Gets the channel positions description (e.g., "Front: L R").</summary>
  public string ChannelPositions => this.GetStringOrDefault("channel positions");
  
  /// <summary>Gets the channel layout (e.g., "L R").</summary>
  public string ChannelLayout => this.GetStringOrDefault("channel layout");

  #endregion

  #region Sampling Information

  /// <summary>Gets the sampling rate in Hz.</summary>
  public int SamplingRate => this.GetSomeIntOrDefault("sampling rate");
  
  /// <summary>Gets the samples per frame.</summary>
  public int SamplesPerFrame => this.GetSomeIntOrDefault("samples per frame");
  
  /// <summary>Gets the total samples count.</summary>
  public long SamplesCount => this.GetLongOrDefault("samples count");
  
  /// <summary>Gets the audio frame rate.</summary>
  public double AudioFrameRate => this.GetDoubleOrDefault("frame rate");

  #endregion

  #region Format Details

  /// <summary>Gets additional format features (e.g., "LC" for AAC LC).</summary>
  public string FormatAdditionalFeatures => this.GetStringOrDefault("format_additionalfeatures");

  #endregion

  #region Compression

  /// <summary>Gets the compression mode (Lossy or Lossless).</summary>
  public CompressionMode CompressionModeValue {
    get {
      var mode = this.GetStringOrDefault("compression mode")?.ToLowerInvariant();
      return mode switch {
        "lossy" => CompressionMode.Lossy,
        "lossless" => CompressionMode.Lossless,
        _ => CompressionMode.Unknown
      };
    }
  }

  #endregion

  #region Delay/Sync

  /// <summary>Gets the delay relative to video in milliseconds (can be negative).</summary>
  public int DelayRelativeToVideoInMilliseconds => this.GetSignedIntOrDefault("delay relative to video");

  #endregion

  #region Replay Gain

  /// <summary>Gets the replay gain value.</summary>
  public double? ReplayGain => this.GetDoubleOrNull("replay gain");
  
  /// <summary>Gets the replay gain peak value.</summary>
  public double? ReplayPeak => this.GetDoubleOrNull("replay gain peak");

  #endregion
}
