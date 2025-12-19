using System;

namespace MassMediaEdit.Abstractions;

/// <summary>
/// Abstraction for UI thread synchronization, enabling testability of UI-bound operations.
/// </summary>
public interface IUiSynchronizer {
  /// <summary>
  /// Invokes an action on the UI thread.
  /// </summary>
  /// <param name="action">The action to invoke.</param>
  void Invoke(Action action);

  /// <summary>
  /// Asynchronously invokes an action on the UI thread without waiting for completion.
  /// </summary>
  /// <param name="action">The action to invoke.</param>
  void BeginInvoke(Action action);

  /// <summary>
  /// Gets whether the current thread requires invoking to access UI elements.
  /// </summary>
  bool InvokeRequired { get; }
}
