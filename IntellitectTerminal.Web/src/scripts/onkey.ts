import { Key, Command, unknownArg, TreeNode, validatePath, output, serializeFilesSystemToTree, KeydownEvent } from "./utils";
import { IntelliTerm } from "./intelliterm";
import { CommandProcessor, KeyListener } from "./decorators";

// All On Key events for the IntelliTerm are handled here
// via the KeyListener decorator. Singleton static class since decorators
// can't access instance params
export class ITKHelper {

    static term: IntelliTerm;

    // Integer index the cursor is currently at. ITKHelper is needed for back spaces.
    static cursorPosition: number;

    static init(term: IntelliTerm): ITKHelper {
        ITKHelper.term = term;
        ITKHelper.cursorPosition = term.hostname.length;
        return new ITKHelper();
    }

    // The stored string the user is typing
    static userInput: string = "";

    // On any key pressed that isnt already mapped
    @KeyListener.on(Key.DEFAULT)
    static default(event: KeydownEvent) {
        ITKHelper.term.write(event.key);
        ITKHelper.userInput += event.key;
        ITKHelper.cursorPosition++;
    }

    @KeyListener.on(Key.BACKSPACE)
    static backspacePressed(event: KeydownEvent) {
        // If the cursor is going to delete from our path... dont
        if (ITKHelper.cursorPosition == ITKHelper.term.hostname.length) { return; }

        // Write backspace
        ITKHelper.term.write("\b \b");
        ITKHelper.userInput = ITKHelper.userInput.substring(0, ITKHelper.userInput.length - 1);
        ITKHelper.cursorPosition--;
        return;
    }

    @KeyListener.on(Key.ENTER)
    static enterPressed(event: KeydownEvent) {
        ITKHelper.term.write(Key.ENTER);
        ITKHelper.userInput += Key.ENTER;

        // Newline
        ITKHelper.term.write("\n");

        // Do not allow a command that is blank to be ran.
        if (ITKHelper.userInput.trim() != "") {
            var cmd: string[] = ITKHelper.userInput.trim().split(" ");
            CommandProcessor.process(cmd[0] as Command, cmd.slice(1, cmd.length), () => output("intelliterm", ITKHelper.term, `Command not found '${cmd}'`));
        }

        ITKHelper.term.write(ITKHelper.term.hostname);
        ITKHelper.userInput = "";
        ITKHelper.cursorPosition = ITKHelper.term.hostname.length;
        return;
    }

    @KeyListener.on(Key.CTRL_L)
    static ctrlLPressed(event: KeydownEvent) {
        ITKHelper.term.clear();
    }
}