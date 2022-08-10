import { Key, Command, unknownArg, TreeNode, validatePath, output, serializeFilesSystemToTree, KeydownEvent } from "./utils";
import { IntelliTerm } from "./intelliterm";
import { CommandProcessor, KeyListener } from "./decorators";

// All On Key events for the IntelliTerm are handled here
// via the KeyListener decorator. Singleton static class since decorators
// can't access instance params
export class OnKeys {

    static term: IntelliTerm;

    // Integer index the cursor is currently at. ITKHelper is needed for back spaces.
    static cursorPosition: number;

    static init(term: IntelliTerm): OnKeys {
        OnKeys.term = term;
        OnKeys.cursorPosition = term.hostname.length;
        return new OnKeys();
    }

    // The stored string the user is typing
    static userInput: string = "";

    // On any key pressed that isnt already mapped
    @KeyListener.on(Key.DEFAULT)
    default(event: KeydownEvent) {
        OnKeys.term.write(event.key);
        OnKeys.userInput += event.key;
        OnKeys.cursorPosition++;
    }

    @KeyListener.on(Key.BACKSPACE)
    backspacePressed(event: KeydownEvent) {
        // If the cursor is going to delete from our path... dont
        if (OnKeys.cursorPosition == OnKeys.term.hostname.length) { return; }

        // Write backspace
        OnKeys.term.write("\b \b");
        OnKeys.userInput = OnKeys.userInput.substring(0, OnKeys.userInput.length - 1);
        OnKeys.cursorPosition--;
        return;
    }

    @KeyListener.on(Key.ENTER)
    enterPressed(event: KeydownEvent) {
        OnKeys.term.write(Key.ENTER);
        OnKeys.userInput += Key.ENTER;

        // Newline
        OnKeys.term.write("\n");

        // Do not allow a command that is blank to be ran.
        if (OnKeys.userInput.trim() != "") {
            var cmd: string[] = OnKeys.userInput.trim().split(" ");
            CommandProcessor.process(cmd[0] as Command, cmd.slice(1, cmd.length), () => output("intelliterm", OnKeys.term, `Command not found '${cmd}'`));
        }

        // Reset and give input prompt
        OnKeys.term.write(OnKeys.term.hostname);
        OnKeys.userInput = "";
        OnKeys.cursorPosition = OnKeys.term.hostname.length;
        return;
    }

    @KeyListener.on(Key.CTRL_L)
    ctrlLPressed(event: KeydownEvent) {
        OnKeys.term.clear();
    }
}