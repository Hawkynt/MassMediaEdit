using NUnit.Framework;

namespace MassMediaEdit.Tests;

/// <summary>
/// Unit tests for the <see cref="EnumerableExtensions"/> class from MainForm.cs.
/// </summary>
[TestFixture]
public sealed class EnumerableExtensionsTests {
  #region OneOrDefault Tests

  [Test]
  public void OneOrDefault_WithNullEnumerable_ReturnsDefault() {
    int[]? items = null;
    
    var result = items.OneOrDefault();
    
    Assert.That(result, Is.EqualTo(default(int)));
  }

  [Test]
  public void OneOrDefault_WithNullEnumerable_ReturnsCustomDefault() {
    string[]? items = null;
    
    var result = items.OneOrDefault("custom");
    
    Assert.That(result, Is.EqualTo("custom"));
  }

  [Test]
  public void OneOrDefault_WithEmptyEnumerable_ReturnsDefault() {
    var items = System.Array.Empty<int>();
    
    var result = items.OneOrDefault();
    
    Assert.That(result, Is.EqualTo(default(int)));
  }

  [Test]
  public void OneOrDefault_WithSingleElement_ReturnsThatElement() {
    var items = new[] { 42 };
    
    var result = items.OneOrDefault();
    
    Assert.That(result, Is.EqualTo(42));
  }

  [Test]
  public void OneOrDefault_WithMultipleElements_ReturnsDefault() {
    var items = new[] { 1, 2, 3 };
    
    var result = items.OneOrDefault();
    
    Assert.That(result, Is.EqualTo(default(int)));
  }

  [Test]
  public void OneOrDefault_WithMultipleElements_ReturnsCustomDefault() {
    var items = new[] { 1, 2, 3 };
    
    var result = items.OneOrDefault(99);
    
    Assert.That(result, Is.EqualTo(99));
  }

  [Test]
  public void OneOrDefault_WithReferenceType_SingleElement_ReturnsThatElement() {
    var items = new[] { "hello" };
    
    var result = items.OneOrDefault();
    
    Assert.That(result, Is.EqualTo("hello"));
  }

  [Test]
  public void OneOrDefault_WithReferenceType_MultipleElements_ReturnsNull() {
    var items = new[] { "hello", "world" };
    
    var result = items.OneOrDefault();
    
    Assert.That(result, Is.Null);
  }

  #endregion
}
