// IntelliTerm Key Listener

import { Terminal } from "xterm";
import { IntelliTermKeyHelper } from "./intelliterm-helpers";
import { Command, Key } from "./utils";

// Provides a decorator that maps any method with the @KeyListener(Key.KEY) syntax to
// a list of keyListeners that are iterated through every key press
export abstract class KeyListener {

    // All of the key listener functions
    private static keyListeners: { 
        key: Key, 
        action: 
        (event: {key: string, domEvent: KeyboardEvent}) => void }[] = [];

    // Bind a key to a function call
    static on = (key: Key) => (target: Object, propertyKey: string,
        descriptor: TypedPropertyDescriptor<any>): any =>
        KeyListener.keyListeners.push({
            key: key,
            action: target as (event: {key: string, domEvent: KeyboardEvent}) => void
        });

    /// Initiate the key listeners onto the terminal instance
    static engage(term: Terminal, keyListener: IntelliTermKeyHelper) {
        term.onKey((e) => {

            // Manual switch case
            let actionCalled: boolean = KeyListener.keyListeners.every((keyListener) => {

                let k = keyListener.key;

                // Ignore arrow keys (for now)
                if (k == Key.ARROW_BOTTOM || k == Key.ARROW_UP || k == Key.ARROW_LEFT || k == Key.ARROW_RIGHT) {
                    return true;
                }

                if (keyListener.key == e.key) {
                    keyListener.action(e);

                    // Break
                    return true;
                }
                return false;
            })

            if (!actionCalled) {
                keyListener.default(e);
                return;
            }
        })
    }
}

// Provides a decorator that maps any method with the @CommandProcessor(Command.COMMAND) syntax
// to an action
export abstract class CommandProcessor {

    // All of the commands
    private static commands: Map<Command, (args: string[]) => void> = new Map();

    // Bind a command to a function call
    static on = (command: Command) => (target: Object, propertyKey: string,
        descriptor: TypedPropertyDescriptor<any>): any =>
        CommandProcessor.commands.set(command, target as (args: string[]) => void);

    // Map to the command to its action
    static process(command: Command, args: string[], _default: (args: string[]) => void): void {
        let action = CommandProcessor.commands.get(command);
        if (action != undefined) {
            action(args);
            return;
        }
        _default(args);

    }
}