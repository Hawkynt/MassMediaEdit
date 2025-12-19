using System;
using MassMediaEdit.Abstractions;
using Models;
using NUnit.Framework;

namespace MassMediaEdit.Tests.Abstractions;

/// <summary>
/// Unit tests for event argument classes in the Abstractions namespace.
/// </summary>
[TestFixture]
public sealed class EventArgsTests {
  #region FilesDroppedEventArgs Tests

  [Test]
  public void FilesDroppedEventArgs_Constructor_SetsPaths() {
    var paths = new[] { "/path/to/file1.mkv", "/path/to/file2.mkv" };

    var args = new FilesDroppedEventArgs(paths);

    Assert.That(args.Paths, Is.EqualTo(paths));
  }

  [Test]
  public void FilesDroppedEventArgs_WithNullPaths_SetsNull() {
    var args = new FilesDroppedEventArgs(null!);

    Assert.That(args.Paths, Is.Null);
  }

  [Test]
  public void FilesDroppedEventArgs_WithEmptyPaths_SetsEmptyArray() {
    var args = new FilesDroppedEventArgs([]);

    Assert.That(args.Paths, Is.Empty);
  }

  [Test]
  public void FilesDroppedEventArgs_InheritsFromEventArgs() {
    var args = new FilesDroppedEventArgs([]);

    Assert.That(args, Is.InstanceOf<EventArgs>());
  }

  [Test]
  public void FilesDroppedEventArgs_PathsAreReadOnly() {
    var originalPaths = new[] { "/path1.mkv", "/path2.mkv" };
    var args = new FilesDroppedEventArgs(originalPaths);

    // Paths should reference the original array
    Assert.That(args.Paths, Is.SameAs(originalPaths));
  }

  [Test]
  public void FilesDroppedEventArgs_WithSinglePath_WorksCorrectly() {
    var args = new FilesDroppedEventArgs(["/single/file.mkv"]);

    Assert.That(args.Paths, Has.Length.EqualTo(1));
    Assert.That(args.Paths[0], Is.EqualTo("/single/file.mkv"));
  }

  #endregion

  #region RenameRequestedEventArgs Tests

  [Test]
  public void RenameRequestedEventArgs_Constructor_SetsMask() {
    const string mask = "{title}.{extension}";

    var args = new RenameRequestedEventArgs(mask);

    Assert.That(args.Mask, Is.EqualTo(mask));
  }

  [Test]
  public void RenameRequestedEventArgs_WithNullMask_SetsNull() {
    var args = new RenameRequestedEventArgs(null!);

    Assert.That(args.Mask, Is.Null);
  }

  [Test]
  public void RenameRequestedEventArgs_WithEmptyMask_SetsEmpty() {
    var args = new RenameRequestedEventArgs(string.Empty);

    Assert.That(args.Mask, Is.Empty);
  }

  [Test]
  public void RenameRequestedEventArgs_InheritsFromEventArgs() {
    var args = new RenameRequestedEventArgs("mask");

    Assert.That(args, Is.InstanceOf<EventArgs>());
  }

  [Test]
  public void RenameRequestedEventArgs_WithWhitespaceMask_PreservesWhitespace() {
    const string mask = "  {title}  ";

    var args = new RenameRequestedEventArgs(mask);

    Assert.That(args.Mask, Is.EqualTo(mask));
  }

  [Test]
  public void RenameRequestedEventArgs_WithComplexMask_PreservesMask() {
    const string mask = "{filename} - {title} [{video:name}].{extension}";

    var args = new RenameRequestedEventArgs(mask);

    Assert.That(args.Mask, Is.EqualTo(mask));
  }

  #endregion

  #region AudioLanguageChangedEventArgs Tests

  [Test]
  public void AudioLanguageChangedEventArgs_Constructor_SetsTrackIndex() {
    var args = new AudioLanguageChangedEventArgs(1, GuiMediaItem.LanguageType.English);

    Assert.That(args.TrackIndex, Is.EqualTo(1));
  }

  [Test]
  public void AudioLanguageChangedEventArgs_Constructor_SetsLanguage() {
    var args = new AudioLanguageChangedEventArgs(0, GuiMediaItem.LanguageType.German);

    Assert.That(args.Language, Is.EqualTo(GuiMediaItem.LanguageType.German));
  }

  [Test]
  public void AudioLanguageChangedEventArgs_WithTrackIndex0_SetsCorrectValue() {
    var args = new AudioLanguageChangedEventArgs(0, GuiMediaItem.LanguageType.English);

    Assert.That(args.TrackIndex, Is.EqualTo(0));
  }

  [Test]
  public void AudioLanguageChangedEventArgs_InheritsFromEventArgs() {
    var args = new AudioLanguageChangedEventArgs(0, GuiMediaItem.LanguageType.None);

    Assert.That(args, Is.InstanceOf<EventArgs>());
  }

  [TestCase(GuiMediaItem.LanguageType.None)]
  [TestCase(GuiMediaItem.LanguageType.Other)]
  [TestCase(GuiMediaItem.LanguageType.German)]
  [TestCase(GuiMediaItem.LanguageType.English)]
  [TestCase(GuiMediaItem.LanguageType.Spanish)]
  [TestCase(GuiMediaItem.LanguageType.Japanese)]
  [TestCase(GuiMediaItem.LanguageType.French)]
  [TestCase(GuiMediaItem.LanguageType.Russian)]
  public void AudioLanguageChangedEventArgs_AcceptsAllLanguageTypes(GuiMediaItem.LanguageType language) {
    var args = new AudioLanguageChangedEventArgs(0, language);

    Assert.That(args.Language, Is.EqualTo(language));
  }

  [TestCase(0)]
  [TestCase(1)]
  [TestCase(2)]
  [TestCase(5)]
  [TestCase(10)]
  public void AudioLanguageChangedEventArgs_AcceptsVariousTrackIndices(int trackIndex) {
    var args = new AudioLanguageChangedEventArgs(trackIndex, GuiMediaItem.LanguageType.English);

    Assert.That(args.TrackIndex, Is.EqualTo(trackIndex));
  }

  [Test]
  public void AudioLanguageChangedEventArgs_TrackIndexAndLanguage_AreIndependent() {
    var args1 = new AudioLanguageChangedEventArgs(0, GuiMediaItem.LanguageType.German);
    var args2 = new AudioLanguageChangedEventArgs(1, GuiMediaItem.LanguageType.German);

    Assert.That(args1.TrackIndex, Is.Not.EqualTo(args2.TrackIndex));
    Assert.That(args1.Language, Is.EqualTo(args2.Language));
  }

  #endregion
}
