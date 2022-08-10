import { Terminal } from "xterm";
import { Command, Key, KeydownEvent } from "./utils";

/** 
 *   These are decorators for easy interaction with the xterm onkey, an alternative to
 *   a hardcoded switchcase with a map, these dynamically create the switchcase.
 *   All of the methods that have a decorator above them are created at compilation,
 *   so they cannot access any instance variables. Thus, any variables must be static
 *   if you want to access them. 
*/

/**  
 *   Provides a decorator that maps any method with the @KeyListener(Key.KEY) syntax to
 *   a list of keyListeners that are iterated through every key press.
 *   @method on(Keys.DEFAULT) MUST exist with the default onKey algorithm.
*/
export abstract class KeyListener {

    // All of the key listener functions
    // This map is filled during compilation
    private static keyListeners: Map<Key, (event: KeydownEvent) => void> = new Map()

    /** @param key Maps to the method the decorator corresponds to */
    static on = (key: Key) => (
        target: Object,
        propertyKey: string,
        descriptor: TypedPropertyDescriptor<any>): any =>
        KeyListener.keyListeners.set(
            key,
            descriptor.value as (event: KeydownEvent) => void   // Descriptor is the weird TS way to get the method.
        );

    /** @param term Initiate the key listeners onto the terminal instance */
    static engage(term: Terminal) {

        // Default method
        const _default = KeyListener.keyListeners.get(Key.DEFAULT)!;

        // On any key pressed
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

/**
 * Provides a decorator that maps any method with the @CommandProcessor(Command.COMMAND) syntax to an action
 * @method process() must be ran on the Enter OnKey for commands to work correctly.
 */
export abstract class CommandProcessor {

    // Filled at run time
    private static commands: Map<Command, (args: string[]) => void> = new Map();

    /**
     * @param command Maps to the method the decorator corresponds to
     */
    static on = (command: Command) => (
        target: Object,
        propertyKey: string,
        descriptor: TypedPropertyDescriptor<any>): any =>
        CommandProcessor.commands.set(command, descriptor.value as (args: string[]) => void   // Descriptor is the weird TS way to get the method.
        );

    /**
     * 
     * @param command The command that is the key to the map
     * @param args Additional arguments to the command EX: -rf
     * @param _default Default method to run if no command is mapped
     * @returns 
     */
    static process(command: Command, args: string[], _default: (args: string[]) => void): void {
        let action = CommandProcessor.commands.get(command);
        if (action != undefined) {
            action(args);
            return;
        }
        _default(args);

    }
}