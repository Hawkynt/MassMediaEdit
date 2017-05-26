using System;
using System.ComponentModel;

namespace Libraries {
  [AttributeUsage(AttributeTargets.Field)]
  class FieldDisplayNameAttribute : DisplayNameAttribute {
    public FieldDisplayNameAttribute(string value) : base(value) { }
  }
}
