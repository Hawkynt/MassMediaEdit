using System;
using System.Diagnostics;
using System.IO;

namespace Classes {
  static internal class MkvMerge {
    private enum ReturnCode {
      Success = 0,
      Warning = 1,
      Error = 2
    }

    public static FileInfo MkvMergeExecutable { get; set; }

    // TODO: report progress
    public static void ConvertToMkv(FileInfo sourceFile, FileInfo targetFile) => _Execute($"--output \"{targetFile.FullName}\" \"{sourceFile.FullName}\"");

    private static void _Execute(string arguments) {

      var executable = MkvMergeExecutable;
      if (executable == null || !executable.Exists)
        throw new NotSupportedException($"Please set path to MKVMerge first using {nameof(MkvMergeExecutable)} property.");


      using (var process = new Process { StartInfo = new ProcessStartInfo(executable.FullName, arguments) { WindowStyle = ProcessWindowStyle.Hidden } }) {
        process.Start();
        process.WaitForExit();
        var result = (ReturnCode)process.ExitCode;
        if (result == ReturnCode.Success || result == ReturnCode.Warning)
          return;

        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();

        throw new Exception($"Something went wrong during MKVPropEdit. Arguments: {arguments}") {
          Data = {
            { nameof(arguments), arguments },
            { nameof(output),output },
            { nameof(error),error }
          }
        };
      }
    }
  }
}
