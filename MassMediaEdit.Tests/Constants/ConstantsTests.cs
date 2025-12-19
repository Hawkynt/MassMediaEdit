using MassMediaEdit.Constants;
using NUnit.Framework;

namespace MassMediaEdit.Tests.Constants;

/// <summary>
/// Unit tests for constant classes.
/// </summary>
[TestFixture]
public sealed class ConstantsTests {
  #region IndicatorKeys Tests

  [Test]
  public void IndicatorKeys_Loading_HasExpectedValue() {
    Assert.That(IndicatorKeys.Loading, Is.EqualTo("Loading"));
  }

  [Test]
  public void IndicatorKeys_Committing_HasExpectedValue() {
    Assert.That(IndicatorKeys.Committing, Is.EqualTo("Committing"));
  }

  [Test]
  public void IndicatorKeys_Converting_HasExpectedValue() {
    Assert.That(IndicatorKeys.Converting, Is.EqualTo("Converting"));
  }

  #endregion

  #region TaskTags Tests

  [Test]
  public void TaskTags_DragDrop_HasExpectedValue() {
    Assert.That(TaskTags.DragDrop, Is.EqualTo("DragDrop"));
  }

  [Test]
  public void TaskTags_Commit_HasExpectedValue() {
    Assert.That(TaskTags.Commit, Is.EqualTo("Commit"));
  }

  [Test]
  public void TaskTags_Convert_HasExpectedValue() {
    Assert.That(TaskTags.Convert, Is.EqualTo("Convert"));
  }

  #endregion

  #region FileExtensions Tests

  [Test]
  public void FileExtensions_Mkv_HasExpectedValue() {
    Assert.That(FileExtensions.Mkv, Is.EqualTo(".mkv"));
  }

  [Test]
  public void FileExtensions_Nfo_HasExpectedValue() {
    Assert.That(FileExtensions.Nfo, Is.EqualTo(".nfo"));
  }

  #endregion

  #region RenamePlaceholders Tests

  [Test]
  public void RenamePlaceholders_Filename_HasExpectedValue() {
    Assert.That(RenamePlaceholders.Filename, Is.EqualTo("{filename}"));
  }

  [Test]
  public void RenamePlaceholders_Extension_HasExpectedValue() {
    Assert.That(RenamePlaceholders.Extension, Is.EqualTo("{extension}"));
  }

  [Test]
  public void RenamePlaceholders_Title_HasExpectedValue() {
    Assert.That(RenamePlaceholders.Title, Is.EqualTo("{title}"));
  }

  [Test]
  public void RenamePlaceholders_VideoName_HasExpectedValue() {
    Assert.That(RenamePlaceholders.VideoName, Is.EqualTo("{video:name}"));
  }

  #endregion

  #region LanguageCodes Tests

  [Test]
  public void LanguageCodes_German_HasCorrectIso639_2Code() {
    Assert.That(LanguageCodes.German, Is.EqualTo("deu"));
  }

  [Test]
  public void LanguageCodes_English_HasCorrectIso639_2Code() {
    Assert.That(LanguageCodes.English, Is.EqualTo("eng"));
  }

  [Test]
  public void LanguageCodes_Japanese_HasCorrectIso639_2Code() {
    Assert.That(LanguageCodes.Japanese, Is.EqualTo("jpn"));
  }

  [Test]
  public void LanguageCodes_Spanish_HasCorrectIso639_2Code() {
    Assert.That(LanguageCodes.Spanish, Is.EqualTo("spa"));
  }

  [Test]
  public void LanguageCodes_French_HasCorrectIso639_2Code() {
    Assert.That(LanguageCodes.French, Is.EqualTo("fra"));
  }

  [Test]
  public void LanguageCodes_Russian_HasCorrectIso639_2Code() {
    Assert.That(LanguageCodes.Russian, Is.EqualTo("rus"));
  }

  #endregion

  #region LanguageCodesShort Tests

  [Test]
  public void LanguageCodesShort_German_HasCorrectTwoLetterCode() {
    Assert.That(LanguageCodesShort.German, Is.EqualTo("de"));
  }

  [Test]
  public void LanguageCodesShort_English_HasCorrectTwoLetterCode() {
    Assert.That(LanguageCodesShort.English, Is.EqualTo("en"));
  }

  [Test]
  public void LanguageCodesShort_Japanese_HasCorrectTwoLetterCode() {
    Assert.That(LanguageCodesShort.Japanese, Is.EqualTo("ja"));
  }

  [Test]
  public void LanguageCodesShort_Spanish_HasCorrectTwoLetterCode() {
    Assert.That(LanguageCodesShort.Spanish, Is.EqualTo("es"));
  }

  [Test]
  public void LanguageCodesShort_French_HasCorrectTwoLetterCode() {
    Assert.That(LanguageCodesShort.French, Is.EqualTo("fr"));
  }

  [Test]
  public void LanguageCodesShort_Russian_HasCorrectTwoLetterCode() {
    Assert.That(LanguageCodesShort.Russian, Is.EqualTo("ru"));
  }

  #endregion
}
