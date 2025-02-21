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

  public bool IsDefault => this.GetBoolOrDefault("default");
  public long SizeInBytes => this.GetLongOrDefault("stream size");
  public TimeSpan Duration => TimeSpan.FromMilliseconds(this.GetDoubleOrDefault("duration"));
  public string Codec => this.GetStringOrDefault("codec", 1);
  public int BitRateInBitsPerSecond => this.GetSomeIntOrDefault("bit rate");
  public string Name => this.GetStringOrDefault("title");

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

  protected string GetStringOrDefault(string key, int index = 0, string defaultValue = default) => this._values.GetValueOrDefault(key, index, defaultValue);
  protected int GetIntOrDefault(string key, int index = 0, int defaultValue = default) => this.GetStringOrDefault(key, index)?.ParseIntOrNull() ?? defaultValue;

  private static readonly Regex _INT_FINDER = new("[0-9]+", RegexOptions.Compiled);
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

  protected long GetLongOrDefault(string key, int index = 0, long defaultValue = default) => this.GetStringOrDefault(key, index)?.ParseLongOrNull() ?? defaultValue;
  protected double GetDoubleOrDefault(string key, int index = 0, double defaultValue = default) => this.GetStringOrDefault(key, index)?.ParseDoubleOrNull(CultureInfo.InvariantCulture) ?? defaultValue;
  protected double? GetDoubleOrNull(string key, int index = 0) => this.GetStringOrDefault(key, index)?.ParseDoubleOrNull(CultureInfo.InvariantCulture);
  protected bool GetBoolOrDefault(string key, int index = 0, bool defaultValue = default) {
    var result = this.GetStringOrDefault(key, index);
    return result == null ? defaultValue : string.Equals(result, "yes", StringComparison.OrdinalIgnoreCase);
  }
}