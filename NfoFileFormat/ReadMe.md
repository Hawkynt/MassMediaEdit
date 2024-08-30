# Hawkynt.NfoFileFormat

![NuGet](https://img.shields.io/nuget/v/Hawkynt.NfoFileFormat?style=flat-square)
![License](https://img.shields.io/github/license/Hawkynt/MassMediaEdit?style=flat-square)

## Overview

**Hawkynt.NfoFileFormat** is a library that provides classes for handling NFO media metadata files. These files are commonly used in conjunction with media management software like Kodi, TinyMediaManager, and Jellyfin. This library enables developers to easily parse, manipulate, and generate NFO files, facilitating better media organization and metadata management.

## Features

- **NFO File Parsing**: Parse existing NFO files to extract media metadata.
- **Metadata Manipulation**: Modify the contents of NFO files programmatically.
- **NFO File Generation**: Create new NFO files with customized metadata.
- **NFO File Updating**: Update existing NFO files with modified metadata.

## Installation

You can install the library via NuGet Package Manager:

```bash
dotnet add package Hawkynt.NfoFileFormat
```

Or use the NuGet Package Manager Console in Visual Studio:

```bash
Install-Package Hawkynt.NfoFileFormat
```

## Usage

Here’s a simple example of how to use the **Hawkynt.NfoFileFormat** library:

```csharp
using Hawkynt.NfoFileFormat;

class Program
{
    static void Main()
    {
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

## Project Structure

This repository is part of the MassMediaEdit project. The **Hawkynt.NfoFileFormat** library is contained in the `NfoFileFormat` directory.

## Contributing

Contributions are welcome! Please refer to the [contribution guidelines](https://github.com/Hawkynt/MassMediaEdit/blob/master/CONTRIBUTING.md) before submitting a pull request.

## License

This project is licensed under the LGPL-3.0-or-later License. See the [LICENSE](https://github.com/Hawkynt/MassMediaEdit/blob/master/LICENSE) file for more details.

## Support

If you encounter any issues or have questions, please open an issue on the [GitHub repository](https://github.com/Hawkynt/MassMediaEdit/issues).

---

For more information, visit the [project page](https://github.com/Hawkynt/MassMediaEdit).