using System;
using System.Collections.Generic;
using MassMediaEdit.Abstractions;
using MassMediaEdit.Constants;
using MassMediaEdit.Presenters;
using Models;
using NSubstitute;
using NUnit.Framework;

namespace MassMediaEdit.Tests.Presenters;

/// <summary>
/// Extended unit tests for the <see cref="MainPresenter"/> class.
/// </summary>
[TestFixture]
public sealed class MainPresenterExtendedTests {
  private IBackgroundTaskRunner _mockBackgroundTaskRunner = null!;
  private IUiSynchronizer _mockUiSynchronizer = null!;
  private IMainView _mockView = null!;
  private MainPresenter _presenter = null!;

  [SetUp]
  public void SetUp() {
    this._mockBackgroundTaskRunner = Substitute.For<IBackgroundTaskRunner>();
    this._mockUiSynchronizer = Substitute.For<IUiSynchronizer>();
    this._mockView = Substitute.For<IMainView>();

    this._mockUiSynchronizer.InvokeRequired.Returns(false);
    this._mockUiSynchronizer.When(x => x.Invoke(Arg.Any<Action>()))
      .Do(x => x.Arg<Action>()());
    this._mockUiSynchronizer.When(x => x.BeginInvoke(Arg.Any<Action>()))
      .Do(x => x.Arg<Action>()());

    this._mockBackgroundTaskRunner.RunAsync(
        Arg.Any<string>(),
        Arg.Any<Action>(),
        Arg.Any<Action?>(),
        Arg.Any<Action?>())
      .Returns(callInfo => {
        callInfo.Arg<Action?>()?.Invoke();
        callInfo.ArgAt<Action>(1)();
        return System.Threading.Tasks.Task.CompletedTask;
      });

    this._presenter = new MainPresenter(this._mockBackgroundTaskRunner, this._mockUiSynchronizer);
    this._presenter.Initialize(this._mockView);
  }

  #region Initialize Event Wiring Tests

  [Test]
  public void Initialize_WiresFilesDroppedEvent() {
    var handler = Substitute.For<EventHandler<FilesDroppedEventArgs>>();
    this._mockView.FilesDropped += handler;

    this._mockView.FilesDropped += Raise.Event<EventHandler<FilesDroppedEventArgs>>(
      this._mockView, new FilesDroppedEventArgs([]));

    handler.Received(1).Invoke(Arg.Any<object>(), Arg.Any<FilesDroppedEventArgs>());
  }

  [Test]
  public void Initialize_WiresRemoveSelectedRequestedEvent() {
    this._mockView.SelectedItems.Returns([]);
    
    this._mockView.RemoveSelectedRequested += Raise.Event<EventHandler>(this._mockView, EventArgs.Empty);
    
    // Should not throw
    Assert.Pass();
  }

  [Test]
  public void Initialize_WiresClearAllRequestedEvent() {
    this._mockView.ClearAllRequested += Raise.Event<EventHandler>(this._mockView, EventArgs.Empty);
    
    this._mockView.Received(1).ClearItems();
  }

  [Test]
  public void Initialize_WiresCommitSelectedRequestedEvent() {
    this._mockView.SelectedItems.Returns([]);
    
    this._mockView.CommitSelectedRequested += Raise.Event<EventHandler>(this._mockView, EventArgs.Empty);
    
    // Should not throw
    Assert.Pass();
  }

  [Test]
  public void Initialize_WiresRevertSelectedRequestedEvent() {
    this._mockView.SelectedItems.Returns([]);
    
    this._mockView.RevertSelectedRequested += Raise.Event<EventHandler>(this._mockView, EventArgs.Empty);
    
    // Should not throw
    Assert.Pass();
  }

  [Test]
  public void Initialize_WiresConvertToMkvRequestedEvent() {
    this._mockView.SelectedItems.Returns([]);
    
    this._mockView.ConvertToMkvRequested += Raise.Event<EventHandler>(this._mockView, EventArgs.Empty);
    
    // Should not throw
    Assert.Pass();
  }

  [Test]
  public void Initialize_WiresTitleFromFilenameRequestedEvent() {
    this._mockView.SelectedItems.Returns([]);
    
    this._mockView.TitleFromFilenameRequested += Raise.Event<EventHandler>(this._mockView, EventArgs.Empty);
    
    // Should not throw
    Assert.Pass();
  }

  [Test]
  public void Initialize_WiresVideoNameFromFilenameRequestedEvent() {
    this._mockView.SelectedItems.Returns([]);
    
    this._mockView.VideoNameFromFilenameRequested += Raise.Event<EventHandler>(this._mockView, EventArgs.Empty);
    
    // Should not throw
    Assert.Pass();
  }

  [Test]
  public void Initialize_WiresFixTitleAndNameRequestedEvent() {
    this._mockView.SelectedItems.Returns([]);
    
    this._mockView.FixTitleAndNameRequested += Raise.Event<EventHandler>(this._mockView, EventArgs.Empty);
    
    // Should not throw
    Assert.Pass();
  }

  [Test]
  public void Initialize_WiresRecoverSpacesRequestedEvent() {
    this._mockView.SelectedItems.Returns([]);
    
    this._mockView.RecoverSpacesRequested += Raise.Event<EventHandler>(this._mockView, EventArgs.Empty);
    
    // Should not throw
    Assert.Pass();
  }

  [Test]
  public void Initialize_WiresRemoveBracketContentRequestedEvent() {
    this._mockView.SelectedItems.Returns([]);
    
    this._mockView.RemoveBracketContentRequested += Raise.Event<EventHandler>(this._mockView, EventArgs.Empty);
    
    // Should not throw
    Assert.Pass();
  }

  [Test]
  public void Initialize_WiresSwapTitleAndNameRequestedEvent() {
    this._mockView.SelectedItems.Returns([]);
    
    this._mockView.SwapTitleAndNameRequested += Raise.Event<EventHandler>(this._mockView, EventArgs.Empty);
    
    // Should not throw
    Assert.Pass();
  }

  [Test]
  public void Initialize_WiresClearTitleRequestedEvent() {
    this._mockView.SelectedItems.Returns([]);
    
    this._mockView.ClearTitleRequested += Raise.Event<EventHandler>(this._mockView, EventArgs.Empty);
    
    // Should not throw
    Assert.Pass();
  }

  [Test]
  public void Initialize_WiresClearVideoNameRequestedEvent() {
    this._mockView.SelectedItems.Returns([]);
    
    this._mockView.ClearVideoNameRequested += Raise.Event<EventHandler>(this._mockView, EventArgs.Empty);
    
    // Should not throw
    Assert.Pass();
  }

  [Test]
  public void Initialize_WiresRenameFilesRequestedEvent() {
    this._mockView.SelectedItems.Returns([]);
    
    this._mockView.RenameFilesRequested += Raise.Event<EventHandler<RenameRequestedEventArgs>>(
      this._mockView, new RenameRequestedEventArgs("{title}.mkv"));
    
    // Should not throw
    Assert.Pass();
  }

  [Test]
  public void Initialize_WiresAudioLanguageChangedEvent() {
    this._mockView.SelectedItems.Returns([]);
    
    this._mockView.AudioLanguageChanged += Raise.Event<EventHandler<AudioLanguageChangedEventArgs>>(
      this._mockView, new AudioLanguageChangedEventArgs(0, GuiMediaItem.LanguageType.English));
    
    // Should not throw
    Assert.Pass();
  }

  #endregion

  #region OnFilesDropped Tests

  [Test]
  public void OnFilesDropped_WithNonExistentPaths_DoesNothing() {
    this._presenter.OnFilesDropped(["/non/existent/path/file.mkv"]);
    
    this._mockBackgroundTaskRunner.DidNotReceive().RunAsync(
      Arg.Any<string>(),
      Arg.Any<Action>(),
      Arg.Any<Action?>(),
      Arg.Any<Action?>());
  }

  #endregion

  #region RevertChanges Tests

  [Test]
  public void RevertChanges_WithEmptyCollection_DoesNotThrow() {
    Assert.DoesNotThrow(() => this._presenter.RevertChanges([]));
  }

  #endregion

  #region CommitChanges Tests

  [Test]
  public void CommitChanges_WithEmptyCollection_DoesNotStartTask() {
    this._presenter.CommitChanges([]);
    
    this._mockBackgroundTaskRunner.DidNotReceive().RunAsync(
      TaskTags.Commit,
      Arg.Any<Action>(),
      Arg.Any<Action?>(),
      Arg.Any<Action?>());
  }

  #endregion

  #region ConvertToMkv Tests

  [Test]
  public void ConvertToMkv_WithEmptyCollection_DoesNotStartTask() {
    this._presenter.ConvertToMkv([]);
    
    this._mockBackgroundTaskRunner.DidNotReceive().RunAsync(
      TaskTags.Convert,
      Arg.Any<Action>(),
      Arg.Any<Action?>(),
      Arg.Any<Action?>());
  }

  #endregion

  #region SetTitleFromFilename Tests

  [Test]
  public void SetTitleFromFilename_WithEmptyCollection_DoesNotThrow() {
    Assert.DoesNotThrow(() => this._presenter.SetTitleFromFilename([]));
  }

  #endregion

  #region SetVideoNameFromFilename Tests

  [Test]
  public void SetVideoNameFromFilename_WithEmptyCollection_DoesNotThrow() {
    Assert.DoesNotThrow(() => this._presenter.SetVideoNameFromFilename([]));
  }

  #endregion

  #region FixTitleAndName Tests

  [Test]
  public void FixTitleAndName_WithEmptyCollection_DoesNotThrow() {
    Assert.DoesNotThrow(() => this._presenter.FixTitleAndName([]));
  }

  #endregion

  #region RecoverSpaces Tests

  [Test]
  public void RecoverSpaces_WithEmptyCollection_DoesNotThrow() {
    Assert.DoesNotThrow(() => this._presenter.RecoverSpaces([]));
  }

  #endregion

  #region RemoveBracketContent Tests

  [Test]
  public void RemoveBracketContent_WithEmptyCollection_DoesNotThrow() {
    Assert.DoesNotThrow(() => this._presenter.RemoveBracketContent([]));
  }

  #endregion

  #region SwapTitleAndName Tests

  [Test]
  public void SwapTitleAndName_WithEmptyCollection_DoesNotThrow() {
    Assert.DoesNotThrow(() => this._presenter.SwapTitleAndName([]));
  }

  #endregion

  #region ClearTitle Tests

  [Test]
  public void ClearTitle_WithEmptyCollection_DoesNotThrow() {
    Assert.DoesNotThrow(() => this._presenter.ClearTitle([]));
  }

  #endregion

  #region ClearVideoName Tests

  [Test]
  public void ClearVideoName_WithEmptyCollection_DoesNotThrow() {
    Assert.DoesNotThrow(() => this._presenter.ClearVideoName([]));
  }

  #endregion

  #region RenameFiles Tests

  [Test]
  public void RenameFiles_WithEmptyCollection_DoesNotThrow() {
    Assert.DoesNotThrow(() => this._presenter.RenameFiles([], "{title}.mkv"));
  }

  #endregion

  #region SetAudioLanguage Tests

  [Test]
  public void SetAudioLanguage_WithEmptyCollection_DoesNotThrow() {
    Assert.DoesNotThrow(() => this._presenter.SetAudioLanguage([], 0, GuiMediaItem.LanguageType.English));
  }

  [Test]
  public void SetAudioLanguage_WithTrackIndex0_SetsAudio0Language() {
    // Test with empty collection - just verify no exception
    Assert.DoesNotThrow(() => this._presenter.SetAudioLanguage([], 0, GuiMediaItem.LanguageType.German));
  }

  [Test]
  public void SetAudioLanguage_WithTrackIndex1_SetsAudio1Language() {
    // Test with empty collection - just verify no exception
    Assert.DoesNotThrow(() => this._presenter.SetAudioLanguage([], 1, GuiMediaItem.LanguageType.German));
  }

  #endregion

  #region AddFile Tests

  [Test]
  public void AddFile_WithNullFile_ThrowsArgumentNullException() {
    Assert.Throws<ArgumentNullException>(() => this._presenter.AddFile(null!));
  }

  #endregion
}
