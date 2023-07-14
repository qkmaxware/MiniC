﻿using CommandLine;
using Qkmaxware.Languages.C.Terminal.Commands;

namespace Qkmaxware.Languages.C.Terminal;

/// <summary>
/// Terminal status codes
/// </summary>
public enum StatusCode {
    Ok = 0, Err = 1
}

public class Program {

    public static int Main() {
        return new Parser(settings => {
                settings.AllowMultiInstance = true;
            })
            .ParseArguments<Build>(
                System.Environment.GetCommandLineArgs().Skip(1)
            )
            .MapResult(
                (Build build) => (int)build.TryExecute(),
                errs => 1
            );
    }

}