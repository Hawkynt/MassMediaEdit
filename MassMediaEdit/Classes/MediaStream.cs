using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Classes;

public class MediaStream {
  private readonly SectionDictionary _values;
  internal MediaStream(SectionDictionary values) {
    this._values = values;
  }

  #region Common Properties

  public bool IsDefault => this.GetBoolOrDefault("default");
  public bool IsForced => this.GetBoolOrDefault("forced");
  public long SizeInBytes => this.GetLongOrDefault("stream size");
  public TimeSpan Duration => TimeSpan.FromMilliseconds(this.GetDoubleOrDefault("duration"));
  public string Codec => this.GetStringOrDefault("codec", 1);
  public int BitRateInBitsPerSecond => this.GetSomeIntOrDefault("bit rate");
  public string Name => this.GetStringOrDefault("title");
  
  /// <summary>Gets the unique identifier for the stream.</summary>
  public string UniqueId => this.GetStringOrDefault("unique id");
  
  /// <summary>Gets the stream identifier.</summary>
  public int StreamIdentifier => this.GetIntOrDefault("stream identifier");
  
  /// <summary>Gets the stream order in the container.</summary>
  public int StreamOrder => this.GetIntOrDefault("streamorder");
  
  /// <summary>Gets the track ID.</summary>
  public int Id => this.GetIntOrDefault("id");
  
  /// <summary>Gets the codec ID.</summary>
  public string CodecId => this.GetStringOrDefault("codec id");
  
  /// <summary>Gets the format of the stream.</summary>
  public string Format => this.GetStringOrDefault("format");
  
  /// <summary>Gets the format info (description of the format).</summary>
  public string FormatInfo => this.GetStringOrDefault("format/info");
  
  /// <summary>Gets the commercial name of the format.</summary>
  public string CommercialName => this.GetStringOrDefault("commercial name");
  
  /// <summary>Gets the delay in milliseconds.</summary>
  public int DelayInMilliseconds => this.GetSomeIntOrDefault("delay");
  
  /// <summary>Gets the delay origin.</summary>
  public string DelayOrigin => this.GetStringOrDefault("delay, origin");
  
  /// <summary>Gets the proportion of this stream relative to total file size.</summary>
  public double ProportionOfStream => this.GetDoubleOrDefault("proportion of this stream");
  
  /// <summary>Gets the frame count.</summary>
  public long FrameCount => this.GetLongOrDefault("frame count");

  #endregion

  #region Language

  private static readonly Dictionary<string, string> _LANGUAGE_CONVERTERS = new(StringComparer.OrdinalIgnoreCase) {
    {"deutsch","de" },
    {"german","de" },
    {"englisch","en" },
    {"english","en" },
  };

  private static readonly ConcurrentDictionary<string, CultureInfo> _FOUND_CULTURES = new();

  public CultureInfo Language {
    get {
      var result = this.GetStringOrDefault("language");
      if (result == null)
        return null;

      try {
        var lookupValue = _LANGUAGE_CONVERTERS.GetValueOrDefault(result, result);
        var culture = _FOUND_CULTURES.GetOrAdd(lookupValue, CultureInfo.GetCultureInfoByIetfLanguageTag(lookupValue));
        return culture;
      } catch (Exception) {
        return null;
      }
    }
  }

  #endregion

  #region Helper Methods

  protected string GetStringOrDefault(string key, int index = 0, string defaultValue = default) => this._values.GetValueOrDefault(key, index, defaultValue);
  protected int GetIntOrDefault(string key, int index = 0, int defaultValue = default) => this.GetStringOrDefault(key, index)?.ParseIntOrNull() ?? defaultValue;

  private static readonly Regex _INT_FINDER = new("[0-9]+", RegexOptions.Compiled);
  private static readonly Regex _SIGNED_INT_FINDER = new("-?[0-9]+", RegexOptions.Compiled);
  
  protected int GetSomeIntOrDefault(string key, int index = 0, int defaultValue = default)
    => this.GetSomeIntOrNull(key, index) ?? defaultValue
  ;

  protected int? GetSomeIntOrNull(string key, int index = 0) {
    var value = this.GetStringOrDefault(key, index);
    if (value == null || value.Trim().Length < 1)
      return null;

    var match = _INT_FINDER.Match(value);
    return match.Success ? match.Value.ParseIntOrNull() : null;
  }

  /// <summary>
  /// Gets a signed integer from a value that may contain other text (like units).
  /// This method handles negative values.
  /// </summary>
  protected int GetSignedIntOrDefault(string key, int index = 0, int defaultValue = default)
    => this.GetSignedIntOrNull(key, index) ?? defaultValue;

  /// <summary>
  /// Gets a signed integer from a value that may contain other text (like units).
  /// This method handles negative values.
  /// </summary>
  protected int? GetSignedIntOrNull(string key, int index = 0) {
    var value = this.GetStringOrDefault(key, index);
    if (value == null || value.Trim().Length < 1)
      return null;

    var match = _SIGNED_INT_FINDER.Match(value);
    return match.Success ? match.Value.ParseIntOrNull() : null;
  }

  protected long GetLongOrDefault(string key, int index = 0, long defaultValue = default) => this.GetStringOrDefault(key, index)?.ParseLongOrNull() ?? defaultValue;
  protected long? GetLongOrNull(string key, int index = 0) => this.GetStringOrDefault(key, index)?.ParseLongOrNull();
  protected double GetDoubleOrDefault(string key, int index = 0, double defaultValue = default) => this.GetStringOrDefault(key, index)?.ParseDoubleOrNull(CultureInfo.InvariantCulture) ?? defaultValue;
  protected double? GetDoubleOrNull(string key, int index = 0) => this.GetStringOrDefault(key, index)?.ParseDoubleOrNull(CultureInfo.InvariantCulture);
  protected bool GetBoolOrDefault(string key, int index = 0, bool defaultValue = default) {
    var result = this.GetStringOrDefault(key, index);
    return result == null ? defaultValue : string.Equals(result, "yes", StringComparison.OrdinalIgnoreCase);
  }

  #endregion
}