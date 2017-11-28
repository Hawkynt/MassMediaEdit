using System;
using System.ComponentModel;

namespace Libraries {

  [AttributeUsage(AttributeTargets.Field)]
  internal sealed class FieldDisplayNameAttribute : DisplayNameAttribute {
    public FieldDisplayNameAttribute(string value) : base(value) { }
  }
}
