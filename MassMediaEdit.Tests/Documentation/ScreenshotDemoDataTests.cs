using System.IO;
using System.Linq;
using Classes;
using MassMediaEdit.Documentation;
using NUnit.Framework;

namespace MassMediaEdit.Tests.Documentation;

[TestFixture]
public sealed class ScreenshotDemoDataTests {
  [Test]
  public void DemoRowsExerciseTheImportantGridStates() {
    using var data = ScreenshotDemoData.Create();

    Assert.Multiple(() => {
      Assert.That(data.Items, Has.Count.EqualTo(3));
      Assert.That(data.Items.Count(item => item.IsMkvContainer), Is.EqualTo(2));
      Assert.That(data.Items.Count(item => item.IsMkvConversionEnabled), Is.EqualTo(1));
      Assert.That(data.Items.Any(item => item.HasNfo), Is.True);
      Assert.That(data.Items.Any(item => item.HasAudio1), Is.True);
      Assert.That(data.Items.Any(item => item.NeedsCommit), Is.True);
      Assert.That(data.Items.Any(item => item.Video0StereoscopicMode != StereoscopicMode.None), Is.True);
    });
  }

  [Test]
  public void DemoRowsUseRealTemporaryFilesAndCleanupRemovesThem() {
    var data = ScreenshotDemoData.Create();
    var files = data.Items.Select(item => item.MediaFile.File.FullName).ToArray();

    Assert.That(files.All(File.Exists), Is.True);

    data.Dispose();

    Assert.That(files.All(file => !File.Exists(file)), Is.True);
  }
}
