import { UserServiceViewModel, CommandServiceViewModel } from "@/viewmodels.g";
import { Terminal } from "xterm";

export class TreeNode {
    Name: string;
    isFile: boolean;
    Parent: TreeNode | null;
    Children: TreeNode[];
    constructor(name: string, isFile: boolean, parent: TreeNode | null, children: TreeNode[]) {
        this.Name = name;
        this.isFile = isFile;
        this.Parent = parent;
        this.Children = children;
    }
}

export interface API {
    command: CommandServiceViewModel;
    user: UserServiceViewModel;
}

export interface FileNode {
    Parent: string;
    Name: string;
    isFile: boolean;
    Children: FileNode[];
}

export interface KeydownEvent {
    key: string,
    domEvent: KeyboardEvent
}

export enum Command {
    HELP = "help",
    REQUEST = "request",
    LS = "ls",
    CD = "cd",
    CAT = "cat",
    CLEAR = "clear",
    PROGRESS = "progress",
    PWD = "pwd",
    SUBMIT = "submit",
    VERIFY = "verify"
}


export enum Key {
    BACKSPACE = "\x7F",
    ENTER = "\r",
    ARROW_LEFT = "\x1B[D",
    ARROW_RIGHT = "\x1B[C",
    ARROW_UP = "\x1B[A",
    ARROW_BOTTOM = "\x1B[B",
    CTRL_L = "\f",
    DEFAULT = ""
}

export function output(prefix: string, term: Terminal, msg: string) {
    term.writeln(`\r\n\x1b[34m${prefix}\x1b[0m: ${msg}`);
}

export function unknownArg(prefix: string, term: Terminal, unArg: string) {
    output(prefix, term, `Unknown argument '${unArg}'`);
}

export function validatePath(start: TreeNode, directions: string[], term: Terminal, arg: string): TreeNode {
    // Start on the dir we are on
    let traverse: TreeNode = start;
    directions.every((direction: string) => {

        // Navigate up one on ..
        if (direction == "..") {
            traverse = traverse.Parent!;
            return true;
        }

        // Find the node that matches the path
        let i = traverse.Children.find((child) => child.Name == direction);


        // If no node is found, the path is errornous. Return.
        if (i == undefined) {
            output(Command.CD, term, `Directory not found '${arg}'`);
            return;
        }
        if (i.isFile) {
            output(Command.CD, term, "Argument is a file and not a directory");
            return;
        }
        traverse = i;
        return true;
    })
    return traverse;
}

export function serializeFilesSystemToTree(node: FileNode | TreeNode) {
    let parent = new TreeNode(node.Name, node.isFile, null, []);
    node.Children.forEach((child) => {
        let childNode: TreeNode = serializeFilesSystemToTree(child);
        childNode.Parent = parent;
        parent.Children.push(childNode);
    });
    return parent;
}