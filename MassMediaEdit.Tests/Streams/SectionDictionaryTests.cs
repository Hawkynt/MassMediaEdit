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
}
