using System;
using System.Threading.Tasks;

namespace MassMediaEdit.Abstractions;

/// <summary>
/// Abstraction for background task execution, enabling testability of async operations.
/// </summary>
public interface IBackgroundTaskRunner {
  /// <summary>
  /// Runs a task in the background with progress indication.
  /// </summary>
  /// <param name="tag">A unique tag identifying this task category.</param>
  /// <param name="task">The task to execute.</param>
  /// <param name="onStart">Callback when the task category starts (no tasks were running).</param>
  /// <param name="onComplete">Callback when all tasks in this category complete.</param>
  /// <returns>A task representing the background operation.</returns>
  Task RunAsync(string tag, Action task, Action? onStart = null, Action? onComplete = null);
}
