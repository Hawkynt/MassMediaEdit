using System;
using System.Collections.Generic;
using System.Linq;

namespace Classes;

internal class SectionDictionary {
  private readonly Dictionary<string, List<string>> _values = new(StringComparer.OrdinalIgnoreCase);

  public void Add(string key, string value) => this._values.GetOrAdd(key, () => new List<string>()).Add(value);
  public string GetValueOrDefault(string key, int index = 0, string defaultValue = default) => this._values.GetValueOrNull(key)?.Skip(index).FirstOrDefault() ?? defaultValue;
  
  /// <summary>
  /// Creates a SectionDictionary from raw MediaInfo output lines for a single section.
  /// Each line should be in the format "Key : Value".
  /// </summary>
  public static SectionDictionary FromLines(IEnumerable<string> lines) {
    var dict = new SectionDictionary();
    foreach (var line in lines) {
      var parts = line.Split([':'], 2);
      if (parts.Length == 2) {
        dict.Add(parts[0].TrimEnd(), parts[1].TrimStart());
      }
    }
    return dict;
  }
}