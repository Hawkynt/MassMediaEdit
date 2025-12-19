using System;
using System.IO;
using Classes;
using NUnit.Framework;

namespace MassMediaEdit.Tests;

/// <summary>
/// Unit tests for the <see cref="MediaFile"/> class.
/// </summary>
[TestFixture]
public sealed class MediaFileTests {
  private FileInfo? _originalExecutable;

  [SetUp]
  public void SetUp() => this._originalExecutable = MediaFile.MediaInfoExecutable;

  [TearDown]
  public void TearDown() => MediaFile.MediaInfoExecutable = this._originalExecutable!;

  #region Executable Configuration Tests

  [Test]
  public void FromFile_WhenExecutableNotSet_ThrowsNotSupportedException() {
    MediaFile.MediaInfoExecutable = null!;
    var file = new FileInfo("test.mkv");

    var mediaFile = MediaFile.FromFile(file);
    
    var ex = Assert.Throws<NotSupportedException>(() => _ = mediaFile.GeneralStream);
    Assert.That(ex!.Message, Does.Contain(nameof(MediaFile.MediaInfoExecutable)));
  }

  [Test]
  public void FromFile_WhenExecutableDoesNotExist_ThrowsNotSupportedException() {
    MediaFile.MediaInfoExecutable = new FileInfo("nonexistent_mediainfo.exe");
    var file = new FileInfo("test.mkv");

    var mediaFile = MediaFile.FromFile(file);
    
    var ex = Assert.Throws<NotSupportedException>(() => _ = mediaFile.GeneralStream);
    Assert.That(ex!.Message, Does.Contain(nameof(MediaFile.MediaInfoExecutable)));
  }

  #endregion

  #region FromFile Tests

  [Test]
  public void FromFile_ReturnsMediaFileInstance() {
    var file = new FileInfo("test.mkv");

    var result = MediaFile.FromFile(file);

    Assert.That(result, Is.Not.Null);
  }

  [Test]
  public void FromFile_SetsFileProperty() {
    var file = new FileInfo("test.mkv");

    var result = MediaFile.FromFile(file);

    Assert.That(result.File.Name, Is.EqualTo("test.mkv"));
  }

  #endregion

  #region Stream Property Tests

  [Test]
  public void AudioStreams_WhenExecutableNotSet_ThrowsNotSupportedExceptionOnEnumeration() {
    MediaFile.MediaInfoExecutable = null!;
    var file = new FileInfo("test.mkv");
    var mediaFile = MediaFile.FromFile(file);

    Assert.Throws<NotSupportedException>(() => {
      foreach (var _ in mediaFile.AudioStreams) { }
    });
  }

  [Test]
  public void VideoStreams_WhenExecutableNotSet_ThrowsNotSupportedExceptionOnEnumeration() {
    MediaFile.MediaInfoExecutable = null!;
    var file = new FileInfo("test.mkv");
    var mediaFile = MediaFile.FromFile(file);

    Assert.Throws<NotSupportedException>(() => {
      foreach (var _ in mediaFile.VideoStreams) { }
    });
  }

  #endregion
}
