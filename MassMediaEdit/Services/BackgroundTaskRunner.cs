using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using MassMediaEdit.Abstractions;

namespace MassMediaEdit.Services;

/// <summary>
/// Implementation of <see cref="IBackgroundTaskRunner"/> that tracks concurrent tasks by tag.
/// </summary>
public sealed class BackgroundTaskRunner : IBackgroundTaskRunner {
  private readonly ConcurrentDictionary<string, int[]> _runningTasks = new();
  private readonly IUiSynchronizer? _uiSynchronizer;

  /// <summary>
  /// Creates a new background task runner.
  /// </summary>
  /// <param name="uiSynchronizer">Optional UI synchronizer for callbacks.</param>
  public BackgroundTaskRunner(IUiSynchronizer? uiSynchronizer = null) 
    => this._uiSynchronizer = uiSynchronizer;

  /// <inheritdoc/>
  public Task RunAsync(string tag, Action task, Action? onStart = null, Action? onComplete = null) {
    ArgumentException.ThrowIfNullOrEmpty(tag);
    ArgumentNullException.ThrowIfNull(task);

    return Task.Run(() => {
      var counter = this._runningTasks.GetOrAdd(tag, static _ => [0]);

      try {
        var isFirst = Interlocked.Increment(ref counter[0]) == 1;
        if (isFirst)
          this.InvokeOnUi(onStart);

        task();
      } finally {
        var isLast = Interlocked.Decrement(ref counter[0]) < 1;
        if (isLast)
          this.InvokeOnUi(onComplete);
      }
    });
  }

  private void InvokeOnUi(Action? action) {
    if (action is null)
      return;

    if (this._uiSynchronizer is { InvokeRequired: true } sync)
      sync.Invoke(action);
    else
      action();
  }
}
