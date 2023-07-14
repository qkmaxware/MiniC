using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Qkmaxware.Languages.C.Terminal.Commands;

public abstract class BaseCommand {
    
    public abstract void Execute();

    public StatusCode TryExecute() {
        try {
            Execute();
            return StatusCode.Ok;
        } catch (Exception e) {
            if (e is IPrettyPrint printer) {
                Console.Write(printer.PrettyPrint());
            } else {
                Console.WriteLine(e);
            }
            return StatusCode.Err;
        }
    }

}