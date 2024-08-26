using System;
using System.Collections.Generic;
using System.Linq;

namespace Classes;

internal class SectionDictionary {
  private readonly Dictionary<string, List<string>> _values = new(StringComparer.OrdinalIgnoreCase);

  public void Add(string key, string value) => this._values.GetOrAdd(key, () => new List<string>()).Add(value);
  public string GetValueOrDefault(string key, int index = 0, string defaultValue = default) => this._values.GetValueOrNull(key)?.Skip(index).FirstOrDefault() ?? defaultValue;
}