using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Classes;
using NUnit.Framework;

namespace MassMediaEdit.Tests;

/// <summary>
/// Unit tests for the <see cref="MkvPropEdit"/> class.
/// </summary>
[TestFixture]
public sealed class MkvPropEditTests {
  private FileInfo? _originalExecutable;

  [SetUp]
  public void SetUp() => this._originalExecutable = MkvPropEdit.MkvPropEditExecutable;

  [TearDown]
  public void TearDown() => MkvPropEdit.MkvPropEditExecutable = this._originalExecutable!;

  #region Executable Configuration Tests

  [Test]
  public void SetTitle_WhenExecutableNotSet_ThrowsNotSupportedException() {
    // Arrange
    MkvPropEdit.MkvPropEditExecutable = null!;
    var file = new FileInfo("test.mkv");

    // Act & Assert
    var ex = Assert.Throws<NotSupportedException>(() => MkvPropEdit.SetTitle(file, "Test Title"));
    Assert.That(ex!.Message, Does.Contain(nameof(MkvPropEdit.MkvPropEditExecutable)));
  }

  [Test]
  public void SetTitle_WhenExecutableDoesNotExist_ThrowsNotSupportedException() {
    // Arrange
    MkvPropEdit.MkvPropEditExecutable = new FileInfo("nonexistent_mkvpropedit.exe");
    var file = new FileInfo("test.mkv");

    // Act & Assert
    var ex = Assert.Throws<NotSupportedException>(() => MkvPropEdit.SetTitle(file, "Test Title"));
    Assert.That(ex!.Message, Does.Contain(nameof(MkvPropEdit.MkvPropEditExecutable)));
  }

  #endregion

  #region SetTitle Tests

  [Test]
  public void SetTitle_WithNullTitle_CallsDeleteOperation() {
    // Arrange
    var (mockExecutable, capturedArgs) = CreateMockExecutable();
    MkvPropEdit.MkvPropEditExecutable = mockExecutable;
    var file = new FileInfo("test.mkv");

    // Act
    MkvPropEdit.SetTitle(file, null!);

    // Assert
    Assert.That(capturedArgs(), Does.Contain("--delete"));
    Assert.That(capturedArgs(), Does.Contain("title"));
  }

  [Test]
  public void SetTitle_WithEmptyTitle_CallsDeleteOperation() {
    // Arrange
    var (mockExecutable, capturedArgs) = CreateMockExecutable();
    MkvPropEdit.MkvPropEditExecutable = mockExecutable;
    var file = new FileInfo("test.mkv");

    // Act
    MkvPropEdit.SetTitle(file, "");

    // Assert
    Assert.That(capturedArgs(), Does.Contain("--delete"));
    Assert.That(capturedArgs(), Does.Contain("title"));
  }

  [Test]
  public void SetTitle_WithWhitespaceTitle_CallsDeleteOperation() {
    // Arrange
    var (mockExecutable, capturedArgs) = CreateMockExecutable();
    MkvPropEdit.MkvPropEditExecutable = mockExecutable;
    var file = new FileInfo("test.mkv");

    // Act
    MkvPropEdit.SetTitle(file, "   ");

    // Assert
    Assert.That(capturedArgs(), Does.Contain("--delete"));
    Assert.That(capturedArgs(), Does.Contain("title"));
  }

  [Test]
  public void SetTitle_WithValidTitle_CallsSetOperation() {
    // Arrange
    var (mockExecutable, capturedArgs) = CreateMockExecutable();
    MkvPropEdit.MkvPropEditExecutable = mockExecutable;
    var file = new FileInfo("test.mkv");

    // Act
    MkvPropEdit.SetTitle(file, "My Movie Title");

    // Assert
    Assert.That(capturedArgs(), Does.Contain("--set"));
    Assert.That(capturedArgs(), Does.Contain("title=My Movie Title"));
  }

  #endregion

  #region SetVideoName Tests

  [Test]
  public void SetVideoName_WithNullName_CallsDeleteOperation() {
    // Arrange
    var (mockExecutable, capturedArgs) = CreateMockExecutable();
    MkvPropEdit.MkvPropEditExecutable = mockExecutable;
    var file = new FileInfo("test.mkv");

    // Act
    MkvPropEdit.SetVideoName(file, null!);

    // Assert
    Assert.That(capturedArgs(), Does.Contain("track:v1"));
    Assert.That(capturedArgs(), Does.Contain("--delete"));
    Assert.That(capturedArgs(), Does.Contain("name"));
  }

  [Test]
  public void SetVideoName_WithValidName_CallsSetOperation() {
    // Arrange
    var (mockExecutable, capturedArgs) = CreateMockExecutable();
    MkvPropEdit.MkvPropEditExecutable = mockExecutable;
    var file = new FileInfo("test.mkv");

    // Act
    MkvPropEdit.SetVideoName(file, "Main Video");

    // Assert
    Assert.That(capturedArgs(), Does.Contain("track:v1"));
    Assert.That(capturedArgs(), Does.Contain("--set"));
    Assert.That(capturedArgs(), Does.Contain("name=Main Video"));
  }

  [Test]
  public void SetVideoName_WithStreamIndex_UsesCorrectTrackSelector() {
    // Arrange
    var (mockExecutable, capturedArgs) = CreateMockExecutable();
    MkvPropEdit.MkvPropEditExecutable = mockExecutable;
    var file = new FileInfo("test.mkv");

    // Act
    MkvPropEdit.SetVideoName(file, "Secondary Video", 1);

    // Assert
    Assert.That(capturedArgs(), Does.Contain("track:v2"));
  }

  [TestCase((byte)0, "track:v1")]
  [TestCase((byte)1, "track:v2")]
  [TestCase((byte)5, "track:v6")]
  public void SetVideoName_WithVariousStreamIndices_UsesCorrectTrackSelector(byte streamIndex, string expectedTrack) {
    // Arrange
    var (mockExecutable, capturedArgs) = CreateMockExecutable();
    MkvPropEdit.MkvPropEditExecutable = mockExecutable;
    var file = new FileInfo("test.mkv");

    // Act
    MkvPropEdit.SetVideoName(file, "Test", streamIndex);

    // Assert
    Assert.That(capturedArgs(), Does.Contain(expectedTrack));
  }

  #endregion

  #region SetVideoStereoscopicMode Tests

  [Test]
  public void SetVideoStereoscopicMode_WithZeroValue_CallsDeleteOperation() {
    // Arrange
    var (mockExecutable, capturedArgs) = CreateMockExecutable();
    MkvPropEdit.MkvPropEditExecutable = mockExecutable;
    var file = new FileInfo("test.mkv");

    // Act
    MkvPropEdit.SetVideoStereoscopicMode(file, 0);

    // Assert
    Assert.That(capturedArgs(), Does.Contain("--delete"));
    Assert.That(capturedArgs(), Does.Contain("stereo-mode"));
  }

  [Test]
  public void SetVideoStereoscopicMode_WithNonZeroValue_CallsSetOperation() {
    // Arrange
    var (mockExecutable, capturedArgs) = CreateMockExecutable();
    MkvPropEdit.MkvPropEditExecutable = mockExecutable;
    var file = new FileInfo("test.mkv");

    // Act
    MkvPropEdit.SetVideoStereoscopicMode(file, 1);

    // Assert
    Assert.That(capturedArgs(), Does.Contain("--set"));
    Assert.That(capturedArgs(), Does.Contain("stereo-mode=1"));
  }

  [TestCase(1, "stereo-mode=1")]
  [TestCase(2, "stereo-mode=2")]
  [TestCase(3, "stereo-mode=3")]
  [TestCase(11, "stereo-mode=11")]
  public void SetVideoStereoscopicMode_WithVariousValues_SetsCorrectMode(int mode, string expectedArg) {
    // Arrange
    var (mockExecutable, capturedArgs) = CreateMockExecutable();
    MkvPropEdit.MkvPropEditExecutable = mockExecutable;
    var file = new FileInfo("test.mkv");

    // Act
    MkvPropEdit.SetVideoStereoscopicMode(file, mode);

    // Assert
    Assert.That(capturedArgs(), Does.Contain(expectedArg));
  }

  #endregion

  #region SetAudioLanguage Tests

  [Test]
  public void SetAudioLanguage_WithNullCulture_CallsDeleteOperation() {
    // Arrange
    var (mockExecutable, capturedArgs) = CreateMockExecutable();
    MkvPropEdit.MkvPropEditExecutable = mockExecutable;
    var file = new FileInfo("test.mkv");

    // Act
    MkvPropEdit.SetAudioLanguage(file, null);

    // Assert
    Assert.That(capturedArgs(), Does.Contain("track:a1"));
    Assert.That(capturedArgs(), Does.Contain("--delete"));
    Assert.That(capturedArgs(), Does.Contain("language"));
  }

  [Test]
  public void SetAudioLanguage_WithValidCulture_CallsSetOperationWithIso639Code() {
    // Arrange
    var (mockExecutable, capturedArgs) = CreateMockExecutable();
    MkvPropEdit.MkvPropEditExecutable = mockExecutable;
    var file = new FileInfo("test.mkv");
    var english = new CultureInfo("en-US");

    // Act
    MkvPropEdit.SetAudioLanguage(file, english);

    // Assert
    Assert.That(capturedArgs(), Does.Contain("--set"));
    Assert.That(capturedArgs(), Does.Contain("language=eng"));
  }

  [Test]
  public void SetAudioLanguage_WithGermanCulture_UsesThreeLetterCode() {
    // Arrange
    var (mockExecutable, capturedArgs) = CreateMockExecutable();
    MkvPropEdit.MkvPropEditExecutable = mockExecutable;
    var file = new FileInfo("test.mkv");
    var german = new CultureInfo("de-DE");

    // Act
    MkvPropEdit.SetAudioLanguage(file, german);

    // Assert
    Assert.That(capturedArgs(), Does.Contain("language=deu"));
  }

  [TestCase((byte)0, "track:a1")]
  [TestCase((byte)1, "track:a2")]
  [TestCase((byte)3, "track:a4")]
  public void SetAudioLanguage_WithVariousStreamIndices_UsesCorrectTrackSelector(byte streamIndex, string expectedTrack) {
    // Arrange
    var (mockExecutable, capturedArgs) = CreateMockExecutable();
    MkvPropEdit.MkvPropEditExecutable = mockExecutable;
    var file = new FileInfo("test.mkv");

    // Act
    MkvPropEdit.SetAudioLanguage(file, new CultureInfo("en"), streamIndex);

    // Assert
    Assert.That(capturedArgs(), Does.Contain(expectedTrack));
  }

  #endregion

  #region SetAudioDefault Tests

  [Test]
  public void SetAudioDefault_WhenTrue_SetsFlagToOne() {
    // Arrange
    var (mockExecutable, capturedArgs) = CreateMockExecutable();
    MkvPropEdit.MkvPropEditExecutable = mockExecutable;
    var file = new FileInfo("test.mkv");

    // Act
    MkvPropEdit.SetAudioDefault(file, 0, true);

    // Assert
    Assert.That(capturedArgs(), Does.Contain("flag-default=1"));
  }

  [Test]
  public void SetAudioDefault_WhenFalse_SetsFlagToZero() {
    // Arrange
    var (mockExecutable, capturedArgs) = CreateMockExecutable();
    MkvPropEdit.MkvPropEditExecutable = mockExecutable;
    var file = new FileInfo("test.mkv");

    // Act
    MkvPropEdit.SetAudioDefault(file, 0, false);

    // Assert
    Assert.That(capturedArgs(), Does.Contain("flag-default=0"));
  }

  [TestCase((byte)0, "track:a1")]
  [TestCase((byte)1, "track:a2")]
  [TestCase((byte)2, "track:a3")]
  public void SetAudioDefault_WithVariousStreamIndices_UsesCorrectTrackSelector(byte streamIndex, string expectedTrack) {
    // Arrange
    var (mockExecutable, capturedArgs) = CreateMockExecutable();
    MkvPropEdit.MkvPropEditExecutable = mockExecutable;
    var file = new FileInfo("test.mkv");

    // Act
    MkvPropEdit.SetAudioDefault(file, streamIndex, true);

    // Assert
    Assert.That(capturedArgs(), Does.Contain(expectedTrack));
  }

  #endregion

  #region Helper Methods

  /// <summary>
  /// Creates a mock executable that captures arguments passed to it.
  /// Uses a simple batch/shell script that returns success.
  /// </summary>
  private static (FileInfo executable, Func<string> getCapturedArgs) CreateMockExecutable() {
    var tempDir = Path.Combine(Path.GetTempPath(), $"MkvPropEditTests_{Guid.NewGuid():N}");
    Directory.CreateDirectory(tempDir);

    var argsFile = Path.Combine(tempDir, "args.txt");

    FileInfo executable;
    if (OperatingSystem.IsWindows()) {
      var batPath = Path.Combine(tempDir, "mock_mkvpropedit.bat");
      File.WriteAllText(batPath, $"@echo off\r\necho %* > \"{argsFile}\"\r\nexit /b 0");
      executable = new FileInfo(batPath);
    } else {
      var shPath = Path.Combine(tempDir, "mock_mkvpropedit.sh");
      File.WriteAllText(shPath, $"#!/bin/bash\necho \"$@\" > \"{argsFile}\"\nexit 0");
      using var process = Process.Start("chmod", $"+x \"{shPath}\"");
      process?.WaitForExit();
      executable = new FileInfo(shPath);
    }

    return (executable, () => File.Exists(argsFile) ? File.ReadAllText(argsFile) : string.Empty);
  }

  #endregion
}
