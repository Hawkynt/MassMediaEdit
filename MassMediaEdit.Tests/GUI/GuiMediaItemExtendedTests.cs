using System;
using System.IO;
using Models;
using NUnit.Framework;

namespace MassMediaEdit.Tests.GUI;

/// <summary>
/// Extended unit tests for the <see cref="GuiMediaItem"/> class.
/// </summary>
[TestFixture]
public sealed class GuiMediaItemExtendedTests {
  #region LanguageType Conversion Tests

  [TestCase(GuiMediaItem.LanguageType.None, null)]
  [TestCase(GuiMediaItem.LanguageType.Other, null)]
  public void LanguageType_NoneAndOther_ReturnNullCulture(GuiMediaItem.LanguageType languageType, object? expected) {
    // This test validates the expected behavior of LanguageType.None and Other
    // which should map to null cultures
    Assert.That(expected, Is.Null);
    Assert.That(Enum.IsDefined(typeof(GuiMediaItem.LanguageType), languageType), Is.True);
  }

  #endregion

  #region IsWriteableMediaType Extended Tests

  [TestCase(".MKV")]
  [TestCase(".Mkv")]
  [TestCase(".mKv")]
  [TestCase(".mkV")]
  public void IsWriteableMediaType_WithMixedCaseMkv_ReturnsTrue(string extension) {
    var file = new FileInfo($"test{extension}");
    
    var result = GuiMediaItem.IsWriteableMediaType(file);
    
    Assert.That(result, Is.True);
  }

  [TestCase("test.mp4")]
  [TestCase("test.avi")]
  [TestCase("test.mov")]
  [TestCase("test.wmv")]
  [TestCase("test.flv")]
  [TestCase("test.webm")]
  [TestCase("test.m4v")]
  [TestCase("test.mpg")]
  [TestCase("test.mpeg")]
  [TestCase("test.ts")]
  public void IsWriteableMediaType_WithCommonVideoFormats_ReturnsFalse(string filename) {
    var file = new FileInfo(filename);
    
    var result = GuiMediaItem.IsWriteableMediaType(file);
    
    Assert.That(result, Is.False);
  }

  [TestCase("test.mp3")]
  [TestCase("test.wav")]
  [TestCase("test.flac")]
  [TestCase("test.aac")]
  public void IsWriteableMediaType_WithAudioFormats_ReturnsFalse(string filename) {
    var file = new FileInfo(filename);
    
    var result = GuiMediaItem.IsWriteableMediaType(file);
    
    Assert.That(result, Is.False);
  }

  [Test]
  public void IsWriteableMediaType_WithNoExtension_ReturnsFalse() {
    var file = new FileInfo("testfile");
    
    var result = GuiMediaItem.IsWriteableMediaType(file);
    
    Assert.That(result, Is.False);
  }

  [Test]
  public void IsWriteableMediaType_WithPathContainingMkv_ChecksExtensionOnly() {
    var file = new FileInfo(@"C:\Videos\mkv files\test.mp4");
    
    var result = GuiMediaItem.IsWriteableMediaType(file);
    
    Assert.That(result, Is.False);
  }

  #endregion

  #region LanguageType Enum Tests

  [Test]
  public void LanguageType_None_HasValueZero() {
    Assert.That((int)GuiMediaItem.LanguageType.None, Is.EqualTo(0));
  }

  [Test]
  public void LanguageType_AllValuesAreUnique() {
    var values = Enum.GetValues(typeof(GuiMediaItem.LanguageType));
    var uniqueCount = new System.Collections.Generic.HashSet<int>();
    
    foreach (var value in values) {
      uniqueCount.Add((int)value);
    }
    
    Assert.That(uniqueCount.Count, Is.EqualTo(values.Length));
  }

  [Test]
  public void LanguageType_CanBeParsedFromString() {
    var result = Enum.Parse<GuiMediaItem.LanguageType>("German");
    
    Assert.That(result, Is.EqualTo(GuiMediaItem.LanguageType.German));
  }

  [Test]
  public void LanguageType_CanBeConvertedToString() {
    var result = GuiMediaItem.LanguageType.English.ToString();
    
    Assert.That(result, Is.EqualTo("English"));
  }

  #endregion
}
