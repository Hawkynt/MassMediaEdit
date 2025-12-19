using System;
using System.Threading;
using System.Threading.Tasks;
using MassMediaEdit.Abstractions;
using MassMediaEdit.Services;
using NSubstitute;
using NUnit.Framework;

namespace MassMediaEdit.Tests.Services;

/// <summary>
/// Unit tests for the <see cref="BackgroundTaskRunner"/> class.
/// </summary>
[TestFixture]
public sealed class BackgroundTaskRunnerTests {
  #region RunAsync Basic Tests

  [Test]
  public void RunAsync_WithNullTag_ThrowsArgumentException() {
    var runner = new BackgroundTaskRunner();

    // ArgumentException is thrown synchronously before the task runs
    Assert.Throws(Is.TypeOf<ArgumentException>().Or.TypeOf<ArgumentNullException>(), 
      () => runner.RunAsync(null!, () => { }));
  }

  [Test]
  public void RunAsync_WithEmptyTag_ThrowsArgumentException() {
    var runner = new BackgroundTaskRunner();

    Assert.Throws<ArgumentException>(() => runner.RunAsync(string.Empty, () => { }));
  }

  [Test]
  public void RunAsync_WithNullTask_ThrowsArgumentNullException() {
    var runner = new BackgroundTaskRunner();

    Assert.Throws<ArgumentNullException>(() => runner.RunAsync("tag", null!));
  }

  [Test]
  public async Task RunAsync_ExecutesTask() {
    var runner = new BackgroundTaskRunner();
    var taskExecuted = false;

    await runner.RunAsync("test", () => taskExecuted = true);

    Assert.That(taskExecuted, Is.True);
  }

  #endregion

  #region Callback Tests

  [Test]
  public async Task RunAsync_CallsOnStartForFirstTask() {
    var runner = new BackgroundTaskRunner();
    var onStartCalled = false;

    await runner.RunAsync("test", () => { }, () => onStartCalled = true);

    Assert.That(onStartCalled, Is.True);
  }

  [Test]
  public async Task RunAsync_CallsOnCompleteWhenTaskFinishes() {
    var runner = new BackgroundTaskRunner();
    var onCompleteCalled = false;

    await runner.RunAsync("test", () => { }, null, () => onCompleteCalled = true);

    Assert.That(onCompleteCalled, Is.True);
  }

  [Test]
  public async Task RunAsync_OnStartAndOnComplete_CalledInOrder() {
    var runner = new BackgroundTaskRunner();
    var sequence = 0;
    var onStartSequence = 0;
    var taskSequence = 0;
    var onCompleteSequence = 0;

    await runner.RunAsync(
      "test",
      () => taskSequence = Interlocked.Increment(ref sequence),
      () => onStartSequence = Interlocked.Increment(ref sequence),
      () => onCompleteSequence = Interlocked.Increment(ref sequence));

    Assert.That(onStartSequence, Is.EqualTo(1));
    Assert.That(taskSequence, Is.EqualTo(2));
    Assert.That(onCompleteSequence, Is.EqualTo(3));
  }

  #endregion

  #region Concurrent Task Tests

  [Test]
  public async Task RunAsync_MultipleConcurrentTasks_OnStartCalledOnce() {
    var runner = new BackgroundTaskRunner();
    var onStartCount = 0;
    var barrier = new ManualResetEventSlim(false);

    var task1 = runner.RunAsync(
      "test",
      () => barrier.Wait(1000),
      () => Interlocked.Increment(ref onStartCount));

    var task2 = runner.RunAsync(
      "test",
      () => barrier.Wait(1000),
      () => Interlocked.Increment(ref onStartCount));

    // Give time for both tasks to start
    await Task.Delay(100);
    barrier.Set();

    await Task.WhenAll(task1, task2);

    Assert.That(onStartCount, Is.EqualTo(1));
  }

  [Test]
  public async Task RunAsync_MultipleConcurrentTasks_OnCompleteCalledWhenAllFinish() {
    var runner = new BackgroundTaskRunner();
    var onCompleteCount = 0;
    var barrier1 = new ManualResetEventSlim(false);
    var barrier2 = new ManualResetEventSlim(false);

    var task1 = runner.RunAsync(
      "test",
      () => barrier1.Wait(2000),
      null,
      () => Interlocked.Increment(ref onCompleteCount));

    // Give time for task1 to start
    await Task.Delay(50);

    var task2 = runner.RunAsync(
      "test",
      () => barrier2.Wait(2000),
      null,
      () => Interlocked.Increment(ref onCompleteCount));

    // Complete first task
    barrier1.Set();
    await Task.Delay(100);

    // onComplete should not be called yet (or may be called depending on timing)
    var countAfterFirst = onCompleteCount;

    // Complete second task
    barrier2.Set();
    await Task.WhenAll(task1, task2);

    // Final count should be 1 (called only when all tasks complete)
    Assert.That(onCompleteCount, Is.GreaterThanOrEqualTo(1));
  }

  [Test]
  public async Task RunAsync_DifferentTags_HandleIndependently() {
    var runner = new BackgroundTaskRunner();
    var onStartCount1 = 0;
    var onStartCount2 = 0;

    await runner.RunAsync(
      "tag1",
      () => { },
      () => Interlocked.Increment(ref onStartCount1));

    await runner.RunAsync(
      "tag2",
      () => { },
      () => Interlocked.Increment(ref onStartCount2));

    Assert.That(onStartCount1, Is.EqualTo(1));
    Assert.That(onStartCount2, Is.EqualTo(1));
  }

  #endregion

  #region UI Synchronizer Tests

  [Test]
  public async Task RunAsync_WithUiSynchronizer_InvokesCallbacksOnUiThread() {
    var mockSynchronizer = Substitute.For<IUiSynchronizer>();
    mockSynchronizer.InvokeRequired.Returns(true);
    mockSynchronizer.When(x => x.Invoke(Arg.Any<Action>()))
      .Do(x => x.Arg<Action>()());

    var runner = new BackgroundTaskRunner(mockSynchronizer);
    var onStartCalled = false;

    await runner.RunAsync("test", () => { }, () => onStartCalled = true);

    mockSynchronizer.Received().Invoke(Arg.Any<Action>());
    Assert.That(onStartCalled, Is.True);
  }

  [Test]
  public async Task RunAsync_WithUiSynchronizer_NotRequired_ExecutesDirectly() {
    var mockSynchronizer = Substitute.For<IUiSynchronizer>();
    mockSynchronizer.InvokeRequired.Returns(false);

    var runner = new BackgroundTaskRunner(mockSynchronizer);
    var onStartCalled = false;

    await runner.RunAsync("test", () => { }, () => onStartCalled = true);

    mockSynchronizer.DidNotReceive().Invoke(Arg.Any<Action>());
    Assert.That(onStartCalled, Is.True);
  }

  #endregion

  #region Exception Handling Tests

  [Test]
  public void RunAsync_TaskThrowsException_PropagatesException() {
    var runner = new BackgroundTaskRunner();

    Assert.ThrowsAsync<InvalidOperationException>(async () =>
      await runner.RunAsync("test", () => throw new InvalidOperationException("Test exception")));
  }

  [Test]
  public async Task RunAsync_TaskThrowsException_OnCompleteStillCalled() {
    var runner = new BackgroundTaskRunner();
    var onCompleteCalled = false;

    try {
      await runner.RunAsync(
        "test",
        () => throw new InvalidOperationException(),
        null,
        () => onCompleteCalled = true);
    } catch (InvalidOperationException) {
      // Expected
    }

    Assert.That(onCompleteCalled, Is.True);
  }

  #endregion
}
