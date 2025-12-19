using System;
using System.ComponentModel;
using MassMediaEdit.Abstractions;

namespace MassMediaEdit.Services;

/// <summary>
/// Implementation of <see cref="IUiSynchronizer"/> using <see cref="ISynchronizeInvoke"/>.
/// </summary>
public sealed class WinFormsUiSynchronizer : IUiSynchronizer {
  private readonly ISynchronizeInvoke _syncInvoke;

  /// <summary>
  /// Creates a new Windows Forms UI synchronizer.
  /// </summary>
  /// <param name="syncInvoke">The synchronization context (typically a Form or Control).</param>
  public WinFormsUiSynchronizer(ISynchronizeInvoke syncInvoke) 
    => this._syncInvoke = syncInvoke ?? throw new ArgumentNullException(nameof(syncInvoke));

  /// <inheritdoc/>
  public bool InvokeRequired => this._syncInvoke.InvokeRequired;

  /// <inheritdoc/>
  public void Invoke(Action action) {
    ArgumentNullException.ThrowIfNull(action);

    if (this._syncInvoke.InvokeRequired)
      this._syncInvoke.Invoke(action, null);
    else
      action();
  }

  /// <inheritdoc/>
  public void BeginInvoke(Action action) {
    ArgumentNullException.ThrowIfNull(action);

    if (this._syncInvoke.InvokeRequired)
      this._syncInvoke.BeginInvoke(action, null);
    else
      action();
  }
}
