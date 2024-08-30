# MassMediaEdit

![.NET Version](https://img.shields.io/badge/.NET-8.0-blue)
![License](https://img.shields.io/github/license/Hawkynt/MassMediaEdit?style=flat-square)

## Overview

**MassMediaEdit** is a Windows Forms application designed for batch editing and managing media files. This tool provides various functionalities to streamline the organization and manipulation of media files, including support for different media formats and metadata editing.

## Features

- **Batch Media File Editing**: Quickly and efficiently edit multiple media files.
- **Metadata Management**: Modify and update media file metadata using integrated tools.
- **Support for Multiple Formats**: Works with various media formats including MP4, MKV, and more.
- **Integration with External Tools**: Utilizes tools like [MediaInfo](https://github.com/MediaArea/MediaInfo), [MKVToolNix](https://github.com/Kissaki/MKVToolNix), and [GPAC](https://github.com/gpac/gpac) to perform advanced media operations.

## Installation

To install and run **MassMediaEdit**, ensure that you have the following prerequisites:

- [.NET 8.0 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Windows OS](https://www.microsoft.com/en-us/software-download/windows10)

### Building from Source

1. Clone the repository:

   ```bash
   git clone https://github.com/Hawkynt/MassMediaEdit.git
   ```

2. Navigate to the project directory:

   ```bash
   cd MassMediaEdit
   ```

3. Build the project using the .NET CLI:

   ```bash
   dotnet build
   ```

4. Run the application:

   ```bash
   dotnet run --project MassMediaEdit
   ```

## Usage

### Starting the Application

After launching the application, you can begin editing media files by dragging them into the main window or using the file selection dialog. The application provides various tools to manipulate file metadata, rename files, and perform batch operations.

### Key Features

- **Batch Rename**: Rename files and folders in bulk according to customizable patterns.
- **Metadata Editing**: Update media metadata for better organization in media libraries.
- **Integration with External Tools**: Perform advanced media manipulations using integrated third-party tools.

### Dependencies

The application relies on several third-party tools and libraries:

- **GPAC**: For MP4 file operations.
- **MediaInfo**: To extract detailed information about media files.
- **MKVToolNix**: For MKV file operations.

These tools are included in the `Tools` directory and are configured to be copied to the output directory upon building the project.

## Project Structure

- **MainForm.cs**: The main entry point and UI logic for the application.
- **Libraries**: Custom utilities and helper classes used throughout the application.
- **Tools**: External tools and binaries used for media file operations.
- **Resources**: Images and icons used within the application's GUI.

## Contributing

Contributions are welcome! Please refer to the [contribution guidelines](https://github.com/Hawkynt/MassMediaEdit/blob/master/CONTRIBUTING.md) for more information on how to get started.

## License

This project is licensed under the LGPL-3.0-or-later License. See the [LICENSE](./LICENSE) file for more details.

## Support

If you encounter any issues or have questions, please open an issue on the [GitHub repository](https://github.com/Hawkynt/MassMediaEdit/issues).

---

For more information, visit the [project page](https://github.com/Hawkynt/MassMediaEdit).