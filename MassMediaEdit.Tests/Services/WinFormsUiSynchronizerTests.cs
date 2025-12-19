using System;
using System.ComponentModel;
using MassMediaEdit.Services;
using NSubstitute;
using NUnit.Framework;

namespace MassMediaEdit.Tests.Services;

/// <summary>
/// Unit tests for the <see cref="WinFormsUiSynchronizer"/> class.
/// </summary>
[TestFixture]
public sealed class WinFormsUiSynchronizerTests {
  #region Constructor Tests

  [Test]
  public void Constructor_WithNullForm_ThrowsArgumentNullException() {
    Assert.Throws<ArgumentNullException>(() => new WinFormsUiSynchronizer(null!));
  }

  [Test]
  public void Constructor_WithValidSyncInvoke_DoesNotThrow() {
    var mockSyncInvoke = Substitute.For<ISynchronizeInvoke>();
    
    Assert.DoesNotThrow(() => new WinFormsUiSynchronizer(mockSyncInvoke));
  }

  #endregion

  #region InvokeRequired Tests

  [Test]
  public void InvokeRequired_WhenSyncInvokeRequired_ReturnsTrue() {
    var mockSyncInvoke = Substitute.For<ISynchronizeInvoke>();
    mockSyncInvoke.InvokeRequired.Returns(true);
    var synchronizer = new WinFormsUiSynchronizer(mockSyncInvoke);

    Assert.That(synchronizer.InvokeRequired, Is.True);
  }

  [Test]
  public void InvokeRequired_WhenSyncInvokeNotRequired_ReturnsFalse() {
    var mockSyncInvoke = Substitute.For<ISynchronizeInvoke>();
    mockSyncInvoke.InvokeRequired.Returns(false);
    var synchronizer = new WinFormsUiSynchronizer(mockSyncInvoke);

    Assert.That(synchronizer.InvokeRequired, Is.False);
  }

  #endregion

  #region Invoke Tests

  [Test]
  public void Invoke_WithNullAction_ThrowsArgumentNullException() {
    var mockSyncInvoke = Substitute.For<ISynchronizeInvoke>();
    var synchronizer = new WinFormsUiSynchronizer(mockSyncInvoke);

    Assert.Throws<ArgumentNullException>(() => synchronizer.Invoke(null!));
  }

  [Test]
  public void Invoke_WhenInvokeRequired_CallsSyncInvokeInvoke() {
    var mockSyncInvoke = Substitute.For<ISynchronizeInvoke>();
    mockSyncInvoke.InvokeRequired.Returns(true);
    var synchronizer = new WinFormsUiSynchronizer(mockSyncInvoke);
    var actionCalled = false;

    synchronizer.Invoke(() => actionCalled = true);

    mockSyncInvoke.Received(1).Invoke(Arg.Any<Delegate>(), Arg.Any<object?[]?>());
  }

  [Test]
  public void Invoke_WhenInvokeNotRequired_CallsActionDirectly() {
    var mockSyncInvoke = Substitute.For<ISynchronizeInvoke>();
    mockSyncInvoke.InvokeRequired.Returns(false);
    var synchronizer = new WinFormsUiSynchronizer(mockSyncInvoke);
    var actionCalled = false;

    synchronizer.Invoke(() => actionCalled = true);

    Assert.That(actionCalled, Is.True);
    mockSyncInvoke.DidNotReceive().Invoke(Arg.Any<Delegate>(), Arg.Any<object?[]?>());
  }

  #endregion

  #region BeginInvoke Tests

  [Test]
  public void BeginInvoke_WithNullAction_ThrowsArgumentNullException() {
    var mockSyncInvoke = Substitute.For<ISynchronizeInvoke>();
    var synchronizer = new WinFormsUiSynchronizer(mockSyncInvoke);

    Assert.Throws<ArgumentNullException>(() => synchronizer.BeginInvoke(null!));
  }

  [Test]
  public void BeginInvoke_WhenInvokeRequired_CallsSyncInvokeBeginInvoke() {
    var mockSyncInvoke = Substitute.For<ISynchronizeInvoke>();
    mockSyncInvoke.InvokeRequired.Returns(true);
    var synchronizer = new WinFormsUiSynchronizer(mockSyncInvoke);

    synchronizer.BeginInvoke(() => { });

    mockSyncInvoke.Received(1).BeginInvoke(Arg.Any<Delegate>(), Arg.Any<object?[]?>());
  }

  [Test]
  public void BeginInvoke_WhenInvokeNotRequired_CallsActionDirectly() {
    var mockSyncInvoke = Substitute.For<ISynchronizeInvoke>();
    mockSyncInvoke.InvokeRequired.Returns(false);
    var synchronizer = new WinFormsUiSynchronizer(mockSyncInvoke);
    var actionCalled = false;

    synchronizer.BeginInvoke(() => actionCalled = true);

    Assert.That(actionCalled, Is.True);
    mockSyncInvoke.DidNotReceive().BeginInvoke(Arg.Any<Delegate>(), Arg.Any<object?[]?>());
  }

  #endregion
}
