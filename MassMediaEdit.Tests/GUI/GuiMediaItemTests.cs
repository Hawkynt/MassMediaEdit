using System;
using System.IO;
using Models;
using NUnit.Framework;

namespace MassMediaEdit.Tests.GUI;

/// <summary>
/// Unit tests for the <see cref="GuiMediaItem"/> class.
/// </summary>
[TestFixture]
public sealed class GuiMediaItemTests {
  #region LanguageType Tests

  [TestCase(GuiMediaItem.LanguageType.None)]
  [TestCase(GuiMediaItem.LanguageType.Other)]
  [TestCase(GuiMediaItem.LanguageType.German)]
  [TestCase(GuiMediaItem.LanguageType.English)]
  [TestCase(GuiMediaItem.LanguageType.Spanish)]
  [TestCase(GuiMediaItem.LanguageType.Japanese)]
  [TestCase(GuiMediaItem.LanguageType.French)]
  [TestCase(GuiMediaItem.LanguageType.Russian)]
  public void LanguageType_AllValuesAreDefined(GuiMediaItem.LanguageType language) {
    // Verify all enum values are defined
    Assert.That(Enum.IsDefined(typeof(GuiMediaItem.LanguageType), language), Is.True);
  }

  [Test]
  public void LanguageType_HasExpectedCount() {
    var values = Enum.GetValues(typeof(GuiMediaItem.LanguageType));
    Assert.That(values.Length, Is.EqualTo(8)); // None, Other, German, English, Spanish, Japanese, French, Russian
  }

  #endregion

  #region IsWriteableMediaType Tests

  [Test]
  public void IsWriteableMediaType_WithMkvFile_ReturnsTrue() {
    var file = new FileInfo("test.mkv");
    
    var result = GuiMediaItem.IsWriteableMediaType(file);
    
    Assert.That(result, Is.True);
  }

  [TestCase("test.mp4")]
  [TestCase("test.avi")]
  [TestCase("test.mov")]
  [TestCase("test.wmv")]
  public void IsWriteableMediaType_WithNonMkvFile_ReturnsFalse(string filename) {
    var file = new FileInfo(filename);
    
    var result = GuiMediaItem.IsWriteableMediaType(file);
    
    Assert.That(result, Is.False);
  }

  [Test]
  public void IsWriteableMediaType_WithUppercaseMkv_ReturnsTrue() {
    var file = new FileInfo("test.MKV");
    
    var result = GuiMediaItem.IsWriteableMediaType(file);
    
    Assert.That(result, Is.True);
  }

  #endregion
}
