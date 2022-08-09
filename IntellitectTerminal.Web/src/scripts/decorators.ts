import { Terminal } from "xterm";
import { Command, Key, KeydownEvent } from "./utils";

// Provides a decorator that maps any method with the @KeyListener(Key.KEY) syntax to
// a list of keyListeners that are iterated through every key press
export abstract class KeyListener {

    // All of the key listener functions
    private static keyListeners: Map<Key, (event: KeydownEvent) => void> = new Map()

    // Bind a key to a function call
    static on = (key: Key) => (
        target: Object, 
        propertyKey: string,
        descriptor: TypedPropertyDescriptor<any>): any =>
        KeyListener.keyListeners.set(
            key,
            descriptor.value as (event: KeydownEvent) => void
        );

    /// Initiate the key listeners onto the terminal instance
    static engage(term: Terminal) {
        const _default = KeyListener.keyListeners.get(Key.DEFAULT)!;
        term.onKey((event) => {
            let k = event.key;

            // Ignore arrow keys (for now)
            if (k == Key.ARROW_BOTTOM || k == Key.ARROW_UP || k == Key.ARROW_LEFT || k == Key.ARROW_RIGHT) {
                return;
            }

            let action = KeyListener.keyListeners.get(event.key as Key);
            if (action != undefined) {
                action(event);
                return;
            }
            _default(event);
        })
    }
}

// Provides a decorator that maps any method with the @CommandProcessor(Command.COMMAND) syntax
// to an action
export abstract class CommandProcessor {

    // All of the commands
    private static commands: Map<Command, (args: string[]) => void> = new Map();

    // Bind a command to a function call
    static on = (command: Command) => (
        target: Object, 
        propertyKey: string,
        descriptor: TypedPropertyDescriptor<any>): any =>
        CommandProcessor.commands.set(command, descriptor.value as (args: string[]) => void);

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