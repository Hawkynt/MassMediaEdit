using System;
using System.ComponentModel;

namespace Libraries;

[AttributeUsage(AttributeTargets.Field)]
internal sealed class FieldDisplayNameAttribute(string value) : DisplayNameAttribute(value);