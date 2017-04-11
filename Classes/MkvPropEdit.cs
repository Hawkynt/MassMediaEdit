using System;
using System.Diagnostics;
using System.IO;

namespace Classes {
  static internal class MkvPropEdit {
    public static FileInfo MkvPropEditExecutable { get; set; }

    public static void SetTitle(FileInfo file, string title)
      => _Execute(
        title == null
        ? $"\"{file.FullName}\" --edit info --delete \"title\""
        : $"\"{file.FullName}\" --edit info --set \"title={title}\""
      );

    public static void SetVideo0Name(FileInfo file, string name)
      => _Execute(
        (name == null)
        ? $"\"{file.FullName}\" --edit track:v1 --delete \"name\""
        : $"\"{file.FullName}\" --edit track:v1 --set \"name={name}\""
      );

    public static void SetVideo0StereoscopicMode(FileInfo file, int value)
      => _Execute(
        (value == 0)
        ? $"\"{file.FullName}\" --edit track:v1 --delete \"stereo-mode\""
        : $"\"{file.FullName}\" --edit track:v1 --set \"stereo-mode={value}\""
      );

    private static void _Execute(string arguments) {

      var executable = MkvPropEditExecutable;
      if (executable == null || !executable.Exists)
        throw new NotSupportedException($"Please set path to MKVPropEdit first using {nameof(MkvPropEditExecutable)} property.");


      using (var process = new Process { StartInfo = new ProcessStartInfo(executable.FullName, arguments) { WindowStyle = ProcessWindowStyle.Hidden } }) {
        process.Start();
        process.WaitForExit();
        if (process.ExitCode != 0)
          throw new Exception($"Something went wrong during MKVPropEdit. Arguments: {arguments}") { Data = { { nameof(arguments), arguments } } };
      }
    }


  }
}