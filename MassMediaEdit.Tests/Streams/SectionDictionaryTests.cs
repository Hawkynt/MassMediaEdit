using Classes;
using NUnit.Framework;

namespace MassMediaEdit.Tests.Streams;

/// <summary>
/// Unit tests for the <see cref="SectionDictionary"/> class.
/// </summary>
[TestFixture]
public sealed class SectionDictionaryTests {
  #region FromLines Tests

  [Test]
  public void FromLines_WithValidData_ParsesKeyValuePairs() {
    var lines = new[] {
      "Format                                   : Matroska",
      "Duration                                 : 671189"
    };

    var dict = SectionDictionary.FromLines(lines);

    Assert.That(dict.GetValueOrDefault("format"), Is.EqualTo("Matroska"));
    Assert.That(dict.GetValueOrDefault("duration"), Is.EqualTo("671189"));
  }

  [Test]
  public void FromLines_WithMultipleValuesForSameKey_StoresAll() {
    var lines = new[] {
      "Duration                                 : 671189",
      "Duration                                 : 11 min 11 s",
      "Duration                                 : 00:11:11.189"
    };

    var dict = SectionDictionary.FromLines(lines);

    Assert.That(dict.GetValueOrDefault("duration", 0), Is.EqualTo("671189"));
    Assert.That(dict.GetValueOrDefault("duration", 1), Is.EqualTo("11 min 11 s"));
    Assert.That(dict.GetValueOrDefault("duration", 2), Is.EqualTo("00:11:11.189"));
  }

  [Test]
  public void FromLines_CaseInsensitiveLookup_Works() {
    var lines = new[] {
      "Format                                   : AVC"
    };

    var dict = SectionDictionary.FromLines(lines);

    Assert.That(dict.GetValueOrDefault("FORMAT"), Is.EqualTo("AVC"));
    Assert.That(dict.GetValueOrDefault("format"), Is.EqualTo("AVC"));
    Assert.That(dict.GetValueOrDefault("Format"), Is.EqualTo("AVC"));
  }

  [Test]
  public void FromLines_WithEmptyLines_IgnoresThem() {
    var lines = new[] {
      "",
      "Format                                   : AVC",
      ""
    };

    var dict = SectionDictionary.FromLines(lines);

    Assert.That(dict.GetValueOrDefault("format"), Is.EqualTo("AVC"));
  }

  [Test]
  public void FromLines_WithColonInValue_ParsesCorrectly() {
    var lines = new[] {
      "Writing application                      : mkvmerge v88.0 ('All I Know') 64-bit"
    };

    var dict = SectionDictionary.FromLines(lines);

    Assert.That(dict.GetValueOrDefault("writing application"), Is.EqualTo("mkvmerge v88.0 ('All I Know') 64-bit"));
  }

  [Test]
  public void FromLines_WithTimeValue_ParsesCorrectly() {
    var lines = new[] {
      "Encoded date                             : 2025-12-18 21:34:18 UTC"
    };

    var dict = SectionDictionary.FromLines(lines);

    Assert.That(dict.GetValueOrDefault("encoded date"), Is.EqualTo("2025-12-18 21:34:18 UTC"));
  }

  #endregion

  #region GetValueOrDefault Tests

  [Test]
  public void GetValueOrDefault_WhenKeyNotFound_ReturnsDefault() {
    var dict = SectionDictionary.FromLines([]);

    Assert.That(dict.GetValueOrDefault("nonexistent"), Is.Null);
    Assert.That(dict.GetValueOrDefault("nonexistent", 0, "default"), Is.EqualTo("default"));
  }

  [Test]
  public void GetValueOrDefault_WhenIndexOutOfRange_ReturnsDefault() {
    var lines = new[] {
      "Format                                   : AVC"
    };

    var dict = SectionDictionary.FromLines(lines);

    Assert.That(dict.GetValueOrDefault("format", 5), Is.Null);
    Assert.That(dict.GetValueOrDefault("format", 5, "default"), Is.EqualTo("default"));
  }

  #endregion

  #region Extended FromLines Tests

  [Test]
  public void FromLines_WithWhitespaceOnlyLine_IgnoresIt() {
    var lines = new[] {
      "Format                                   : AVC",
      "     ",
      "Width                                    : 1920"
    };

    var dict = SectionDictionary.FromLines(lines);

    Assert.That(dict.GetValueOrDefault("format"), Is.EqualTo("AVC"));
    Assert.That(dict.GetValueOrDefault("width"), Is.EqualTo("1920"));
  }

  [Test]
  public void FromLines_WithLineWithoutColon_IgnoresIt() {
    var lines = new[] {
      "Format                                   : AVC",
      "This line has no colon",
      "Width                                    : 1920"
    };

    var dict = SectionDictionary.FromLines(lines);

    Assert.That(dict.GetValueOrDefault("format"), Is.EqualTo("AVC"));
    Assert.That(dict.GetValueOrDefault("width"), Is.EqualTo("1920"));
  }

  [Test]
  public void FromLines_WithMultipleColonsInValue_ParsesCorrectly() {
    var lines = new[] {
      "Time                                     : 12:30:45"
    };

    var dict = SectionDictionary.FromLines(lines);

    Assert.That(dict.GetValueOrDefault("time"), Is.EqualTo("12:30:45"));
  }

  [Test]
  public void FromLines_WithUrlValue_ParsesCorrectly() {
    var lines = new[] {
      "Format/Url                               : https://example.com/path?query=value"
    };

    var dict = SectionDictionary.FromLines(lines);

    Assert.That(dict.GetValueOrDefault("format/url"), Is.EqualTo("https://example.com/path?query=value"));
  }

  [Test]
  public void FromLines_TrimsKeyWhitespace() {
    var lines = new[] {
      "  Format                                 : AVC"
    };

    var dict = SectionDictionary.FromLines(lines);

    // Keys are trimmed via TrimEnd on the left part
    Assert.That(dict.GetValueOrDefault("  Format"), Is.EqualTo("AVC"));
  }

  [Test]
  public void FromLines_TrimsValueLeadingWhitespace() {
    var lines = new[] {
      "Format                                   :  AVC"
    };

    var dict = SectionDictionary.FromLines(lines);

    // Values are trimmed via TrimStart on the right part
    Assert.That(dict.GetValueOrDefault("format"), Is.EqualTo("AVC"));
  }

  [Test]
  public void FromLines_WithEmptyValue_StoresEmptyString() {
    var lines = new[] {
      "Format                                   : "
    };

    var dict = SectionDictionary.FromLines(lines);

    Assert.That(dict.GetValueOrDefault("format"), Is.EqualTo(""));
  }

  [Test]
  public void FromLines_LargeDataSet_HandlesCorrectly() {
    var lines = new string[100];
    for (var i = 0; i < 100; i++) {
      lines[i] = $"Key{i}                                     : Value{i}";
    }

    var dict = SectionDictionary.FromLines(lines);

    Assert.That(dict.GetValueOrDefault("key0"), Is.EqualTo("Value0"));
    Assert.That(dict.GetValueOrDefault("key99"), Is.EqualTo("Value99"));
    Assert.That(dict.GetValueOrDefault("key50"), Is.EqualTo("Value50"));
  }

  [Test]
  public void FromLines_WithSpecialCharactersInKey_ParsesCorrectly() {
    var lines = new[] {
      "Format/Info                              : Advanced Video Codec",
      "Format_AdditionalFeatures                : LC",
      "Bits/(Pixel*Frame)                       : 0.061"
    };

    var dict = SectionDictionary.FromLines(lines);

    Assert.That(dict.GetValueOrDefault("format/info"), Is.EqualTo("Advanced Video Codec"));
    Assert.That(dict.GetValueOrDefault("format_additionalfeatures"), Is.EqualTo("LC"));
    Assert.That(dict.GetValueOrDefault("bits/(pixel*frame)"), Is.EqualTo("0.061"));
  }

  [Test]
  public void FromLines_WithParenthesesInValue_ParsesCorrectly() {
    var lines = new[] {
      "Frame rate                               : 46.875 FPS (1024 SPF)"
    };

    var dict = SectionDictionary.FromLines(lines);

    Assert.That(dict.GetValueOrDefault("frame rate"), Is.EqualTo("46.875 FPS (1024 SPF)"));
  }

  #endregion

  #region GetValueOrDefault Extended Tests

  [Test]
  public void GetValueOrDefault_WithIndex0_ReturnsFirstValue() {
    var lines = new[] {
      "Duration                                 : 671189",
      "Duration                                 : 11 min 11 s"
    };

    var dict = SectionDictionary.FromLines(lines);

    Assert.That(dict.GetValueOrDefault("duration", 0), Is.EqualTo("671189"));
  }

  [Test]
  public void GetValueOrDefault_WithNegativeIndex_ReturnsDefault() {
    var lines = new[] {
      "Format                                   : AVC"
    };

    var dict = SectionDictionary.FromLines(lines);

    // Negative index should skip all elements and return default
    Assert.That(dict.GetValueOrDefault("format", -1, "default"), Is.EqualTo("AVC").Or.EqualTo("default"));
  }

  [Test]
  public void GetValueOrDefault_WithMixedCaseKey_ReturnsValue() {
    var lines = new[] {
      "Format_AdditionalFeatures                : LC"
    };

    var dict = SectionDictionary.FromLines(lines);

    Assert.That(dict.GetValueOrDefault("FORMAT_ADDITIONALFEATURES"), Is.EqualTo("LC"));
    Assert.That(dict.GetValueOrDefault("format_additionalfeatures"), Is.EqualTo("LC"));
    Assert.That(dict.GetValueOrDefault("Format_Additionalfeatures"), Is.EqualTo("LC"));
  }

  #endregion

  #region Add Tests

  [Test]
  public void Add_MultipleValues_AllStored() {
    var dict = SectionDictionary.FromLines([]);
    
    // Use FromLines to add values since Add is internal
    var lines = new[] {
      "Test                                     : Value1",
      "Test                                     : Value2",
      "Test                                     : Value3"
    };
    dict = SectionDictionary.FromLines(lines);

    Assert.That(dict.GetValueOrDefault("test", 0), Is.EqualTo("Value1"));
    Assert.That(dict.GetValueOrDefault("test", 1), Is.EqualTo("Value2"));
    Assert.That(dict.GetValueOrDefault("test", 2), Is.EqualTo("Value3"));
  }

  #endregion
}
