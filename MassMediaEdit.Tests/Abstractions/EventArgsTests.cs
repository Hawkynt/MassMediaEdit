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

  #endregion
}
