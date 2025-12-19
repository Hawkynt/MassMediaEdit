using System;
using System.Collections.Generic;
using MassMediaEdit.Abstractions;
using MassMediaEdit.Presenters;
using Models;
using NSubstitute;
using NUnit.Framework;

namespace MassMediaEdit.Tests.Presenters;

/// <summary>
/// Unit tests for the <see cref="MainPresenter"/> class.
/// </summary>
[TestFixture]
public sealed class MainPresenterTests {
  private IBackgroundTaskRunner _mockBackgroundTaskRunner = null!;
  private IUiSynchronizer _mockUiSynchronizer = null!;
  private IMainView _mockView = null!;
  private MainPresenter _presenter = null!;

  [SetUp]
  public void SetUp() {
    this._mockBackgroundTaskRunner = Substitute.For<IBackgroundTaskRunner>();
    this._mockUiSynchronizer = Substitute.For<IUiSynchronizer>();
    this._mockView = Substitute.For<IMainView>();

    // Default synchronizer behavior - execute immediately
    this._mockUiSynchronizer.InvokeRequired.Returns(false);
    this._mockUiSynchronizer.When(x => x.Invoke(Arg.Any<Action>()))
      .Do(x => x.Arg<Action>()());
    this._mockUiSynchronizer.When(x => x.BeginInvoke(Arg.Any<Action>()))
      .Do(x => x.Arg<Action>()());

    // Default background task runner - execute immediately
    this._mockBackgroundTaskRunner.RunAsync(
        Arg.Any<string>(),
        Arg.Any<Action>(),
        Arg.Any<Action?>(),
        Arg.Any<Action?>())
      .Returns(callInfo => {
        callInfo.Arg<Action?>()?.Invoke(); // onStart
        callInfo.ArgAt<Action>(1)(); // task
        return System.Threading.Tasks.Task.CompletedTask;
      });

    this._presenter = new MainPresenter(this._mockBackgroundTaskRunner, this._mockUiSynchronizer);
  }

  #region Constructor Tests

  [Test]
  public void Constructor_WithNullBackgroundTaskRunner_ThrowsArgumentNullException() {
    Assert.Throws<ArgumentNullException>(() => new MainPresenter(null!, this._mockUiSynchronizer));
  }

  [Test]
  public void Constructor_WithNullUiSynchronizer_ThrowsArgumentNullException() {
    Assert.Throws<ArgumentNullException>(() => new MainPresenter(this._mockBackgroundTaskRunner, null!));
  }

  #endregion

  #region Initialize Tests

  [Test]
  public void Initialize_WithNullView_ThrowsArgumentNullException() {
    Assert.Throws<ArgumentNullException>(() => this._presenter.Initialize(null!));
  }

  [Test]
  public void Initialize_SetsPresenterOnView() {
    this._presenter.Initialize(this._mockView);

    Assert.That(this._mockView.Presenter, Is.SameAs(this._presenter));
  }

  #endregion

  #region ClearAll Tests

  [Test]
  public void ClearAll_CallsClearItemsOnView() {
    this._presenter.Initialize(this._mockView);

    this._presenter.ClearAll();

    this._mockView.Received(1).ClearItems();
  }

  #endregion

  #region RemoveItems Tests

  [Test]
  public void RemoveItems_WithEmptyCollection_DoesNotCallView() {
    this._presenter.Initialize(this._mockView);

    this._presenter.RemoveItems([]);

    this._mockView.DidNotReceive().RemoveItems(Arg.Any<IEnumerable<GuiMediaItem>>());
  }

  #endregion

  #region RenameFiles Tests

  [Test]
  public void RenameFiles_WithNullMask_ThrowsArgumentException() {
    this._presenter.Initialize(this._mockView);

    Assert.Throws(Is.TypeOf<ArgumentException>().Or.TypeOf<ArgumentNullException>(), 
      () => this._presenter.RenameFiles([], null!));
  }

  [Test]
  public void RenameFiles_WithEmptyMask_ThrowsArgumentException() {
    this._presenter.Initialize(this._mockView);

    Assert.Throws<ArgumentException>(() => this._presenter.RenameFiles([], string.Empty));
  }

  #endregion

  #region OnFilesDropped Tests

  [Test]
  public void OnFilesDropped_WithNullPaths_DoesNothing() {
    this._presenter.Initialize(this._mockView);

    // Should not throw
    Assert.DoesNotThrow(() => this._presenter.OnFilesDropped(null!));
  }

  [Test]
  public void OnFilesDropped_WithEmptyPaths_DoesNothing() {
    this._presenter.Initialize(this._mockView);

    // Should not throw
    Assert.DoesNotThrow(() => this._presenter.OnFilesDropped([]));
  }

  #endregion
}
