namespace MassMediaEdit.Constants;

/// <summary>
/// Constants for loading indicator keys.
/// </summary>
public static class IndicatorKeys {
  public const string Loading = nameof(Loading);
  public const string Committing = nameof(Committing);
  public const string Converting = nameof(Converting);
}

/// <summary>
/// Constants for background task tags.
/// </summary>
public static class TaskTags {
  public const string DragDrop = nameof(DragDrop);
  public const string Commit = nameof(Commit);
  public const string Convert = nameof(Convert);
}

/// <summary>
/// Constants for file extensions.
/// </summary>
public static class FileExtensions {
  public const string Mkv = ".mkv";
  public const string Nfo = ".nfo";
}

/// <summary>
/// Constants for rename mask placeholders.
/// </summary>
public static class RenamePlaceholders {
  public const string Filename = "{filename}";
  public const string Extension = "{extension}";
  public const string Title = "{title}";
  public const string VideoName = "{video:name}";
}

/// <summary>
/// Constants for ISO 639-2 language codes.
/// </summary>
public static class LanguageCodes {
  public const string German = "deu";
  public const string English = "eng";
  public const string Japanese = "jpn";
  public const string Spanish = "spa";
  public const string French = "fra";
  public const string Russian = "rus";
}

/// <summary>
/// Constants for two-letter language codes.
/// </summary>
public static class LanguageCodesShort {
  public const string German = "de";
  public const string English = "en";
  public const string Japanese = "ja";
  public const string Spanish = "es";
  public const string French = "fr";
  public const string Russian = "ru";
}