# Hawkynt.NfoFileFormat

[![Build](https://github.com/Hawkynt/MassMediaEdit/actions/workflows/Build.yml/badge.svg)](https://github.com/Hawkynt/MassMediaEdit/actions/workflows/Build.yml)
[![Last Commit](https://img.shields.io/github/last-commit/Hawkynt/MassMediaEdit?branch=master)](https://github.com/Hawkynt/MassMediaEdit/commits/master/NfoFileFormat)
[![NuGet](https://img.shields.io/nuget/v/Hawkynt.NfoFileFormat)](https://www.nuget.org/packages/Hawkynt.NfoFileFormat/)
![License](https://img.shields.io/github/license/Hawkynt/MassMediaEdit)


## Overview

**NfoFileFormat** is a library that provides classes for handling NFO media metadata files. These files are commonly used in conjunction with media management software like Kodi, TinyMediaManager, and Jellyfin. This library enables developers to easily parse, manipulate, and generate NFO files, facilitating better media organization and metadata management.

## Features

- **NFO File Parsing**: Parse existing NFO files to extract media metadata.
- **Metadata Manipulation**: Modify the contents of NFO files programmatically.
- **NFO File Generation**: Create new NFO files with customized metadata.
- **NFO File Updating**: Update existing NFO files with modified metadata.

## Project Structure

This repository is part of the [MassMediaEdit](https://github.com/Hawkynt/MassMediaEdit) project. The **Hawkynt.NfoFileFormat** library is contained in the `NfoFileFormat` directory.

## Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) installed on your machine.

### Building the Project

To build the project, navigate to the project directory and run:

```bash
dotnet build
```

### Running Tests

If the project includes unit tests (typically in a `Tests` or similar directory), you can run them using:

```bash
dotnet test
```

### Using the Library

Here’s a simple example of how to use the **NfoFileFormat** library:

```csharp
using Hawkynt.NfoFileFormat;

class Program {
  static void Main() {
    var nfoFile = NfoFile.Load("path_to_nfo_file.nfo");

    // Access metadata
    Console.WriteLine(nfoFile.Title);
    Console.WriteLine(nfoFile.Year);

    // Modify metadata
    nfoFile.Title = "New Title";
    nfoFile.Save("path_to_modified_nfo_file.nfo");
  }
}
```

For more detailed examples and API documentation, please refer to the [project documentation](https://github.com/Hawkynt/MassMediaEdit/tree/master/NfoFileFormat).

## Contributing

Contributions are welcome! Please refer to the [contribution guidelines](https://github.com/Hawkynt/MassMediaEdit/blob/master/CONTRIBUTING.md) before submitting a pull request.

## License

This project is licensed under the LGPL-3.0-or-later License. See the [LICENSE](https://github.com/Hawkynt/MassMediaEdit/blob/master/LICENSE) file for more details.

## Support

If you encounter any issues or have questions, please open an issue on the [GitHub repository](https://github.com/Hawkynt/MassMediaEdit/issues).
