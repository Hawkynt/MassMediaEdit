using System;
using System.IO;
using Classes;
using NUnit.Framework;

namespace MassMediaEdit.Tests;

/// <summary>
/// Unit tests for the <see cref="MkvMerge"/> class.
/// </summary>
[TestFixture]
public sealed class MkvMergeTests {
  private FileInfo? _originalExecutable;

  [SetUp]
  public void SetUp() => this._originalExecutable = MkvMerge.MkvMergeExecutable;

  [TearDown]
  public void TearDown() => MkvMerge.MkvMergeExecutable = this._originalExecutable!;

  #region Executable Configuration Tests

  [Test]
  public void ConvertToMkv_WhenExecutableNotSet_ThrowsNotSupportedExceptionOrFileNotFound() {
    MkvMerge.MkvMergeExecutable = null!;
    var sourceFile = new FileInfo("test.mp4");
    var targetFile = new FileInfo("test.mkv");

    Assert.Throws(Is.TypeOf<NotSupportedException>().Or.TypeOf<FileNotFoundException>(), 
      () => MkvMerge.ConvertToMkv(sourceFile, targetFile));
  }

  [Test]
  public void ConvertToMkv_WhenExecutableDoesNotExist_ThrowsNotSupportedExceptionOrFileNotFound() {
    MkvMerge.MkvMergeExecutable = new FileInfo("nonexistent_mkvmerge.exe");
    var sourceFile = new FileInfo("test.mp4");
    var targetFile = new FileInfo("test.mkv");

    Assert.Throws(Is.TypeOf<NotSupportedException>().Or.TypeOf<FileNotFoundException>(), 
      () => MkvMerge.ConvertToMkv(sourceFile, targetFile));
  }

  #endregion

  #region Source File Validation Tests

  [Test]
  public void ConvertToMkv_WhenSourceFileDoesNotExist_ThrowsFileNotFoundException() {
    var tempDir = Path.Combine(Path.GetTempPath(), $"MkvMergeTests_{Guid.NewGuid():N}");
    Directory.CreateDirectory(tempDir);
    
    try {
      var mockExecutable = CreateMockExecutable(tempDir);
      MkvMerge.MkvMergeExecutable = mockExecutable;
      
      var sourceFile = new FileInfo(Path.Combine(tempDir, "nonexistent.mp4"));
      var targetFile = new FileInfo(Path.Combine(tempDir, "output.mkv"));

      var ex = Assert.Throws<FileNotFoundException>(() => MkvMerge.ConvertToMkv(sourceFile, targetFile));
      
      Assert.That(ex!.Message, Does.Contain(sourceFile.FullName));
    } finally {
      Directory.Delete(tempDir, true);
    }
  }

  #endregion

  #region Progress Reporter Tests

  [Test]
  public void ConvertToMkv_WithNullProgressReporter_DoesNotThrow() {
    var tempDir = Path.Combine(Path.GetTempPath(), $"MkvMergeTests_{Guid.NewGuid():N}");
    Directory.CreateDirectory(tempDir);
    
    try {
      var mockExecutable = CreateMockExecutable(tempDir);
      MkvMerge.MkvMergeExecutable = mockExecutable;
      
      var sourceFile = new FileInfo(Path.Combine(tempDir, "source.mp4"));
      File.WriteAllText(sourceFile.FullName, "dummy content");
      
      var targetFile = new FileInfo(Path.Combine(tempDir, "output.mkv"));

      // Should not throw NullReferenceException - other exceptions are acceptable
      Assert.DoesNotThrow(() => {
        try {
          MkvMerge.ConvertToMkv(sourceFile, targetFile, null);
        } catch (Exception ex) when (ex is not NullReferenceException) {
          // Expected - mock doesn't actually work
        }
      });
    } finally {
      Directory.Delete(tempDir, true);
    }
  }

  #endregion

  #region Helper Methods

  private static FileInfo CreateMockExecutable(string tempDir) {
    FileInfo executable;
    if (OperatingSystem.IsWindows()) {
      var batPath = Path.Combine(tempDir, "mock_mkvmerge.bat");
      File.WriteAllText(batPath, "@echo off\r\nexit /b 0");
      executable = new FileInfo(batPath);
    } else {
      var shPath = Path.Combine(tempDir, "mock_mkvmerge.sh");
      File.WriteAllText(shPath, "#!/bin/bash\nexit 0");
      executable = new FileInfo(shPath);
      System.Diagnostics.Process.Start("chmod", $"+x \"{shPath}\"")?.WaitForExit();
    }
    return executable;
  }

  #endregion
}
