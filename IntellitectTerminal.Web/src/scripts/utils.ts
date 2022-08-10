import { UserServiceViewModel, CommandServiceViewModel } from "@/viewmodels.g";
import { Terminal } from "xterm";

export class TreeNode {
    Name: string;
    isFile: boolean;
    Parent: TreeNode | null;
    Children: TreeNode[];
    constructor(args: {
        name: string, isFile: boolean, parent: TreeNode | null, children: TreeNode[]
    }) {
        this.Name = args.name;
        this.isFile = args.isFile;
        this.Parent = args.parent;
        this.Children = args.children;
    }

    static fromRootNode(): TreeNode {
        return new TreeNode({
            name: "/",
            isFile: false,
            parent: null,
            children: []
        });
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

export function output(params: { prefix: string, term: Terminal, msg: string }) {
    params.term.writeln(`\x1b[34m${params.prefix}\x1b[0m: ${params.msg}`);
}

export function unknownArg(params: { prefix: string, term: Terminal, unArg: string }) {
    output({
        prefix: params.prefix,
        term: params.term,
        msg: params.unArg
    });
}

/**
 * Finds if the path given exists.
 * @param params.start Beginning TreeNode to search from
 * @returns a new path if valid, params.start if invalid
 */
export function validatePath(params: { start: TreeNode, directions: string[], term: Terminal, arg: string }): TreeNode {
    // Start on the dir we are on
    // Clone the object since we don't want to iterate on our actual path
    let traverse: TreeNode = structuredClone(params.start);
    let res = params.directions.every((direction: string) => {

        // Navigate up one on ..
        if (direction == "..") {
            traverse = traverse.Parent!;
            return true;
        }

        // Find the node that matches the path
        let i = traverse.Children.find((child) => child.Name == direction);

        // If no node is found, the path is errornous. Return.
        if (i == undefined) {
            output({
                prefix: Command.CD,
                term: params.term,
                msg: `Directory not found '${params.arg}'`
            });
            return false;;
        }
        if (i.isFile) {
            output({
                prefix: Command.CD,
                term: params.term,
                msg: `Argument is a file not a directory.'${params.arg}'`
            });
            return false;
        }
        traverse = i;
        return true;
    })
    return res ? traverse : params.start;
}

export function serializeFilesSystemToTree(node: FileNode | TreeNode) {
    let parent = new TreeNode({
        name: node.Name,
        isFile: node.isFile,
        parent: null,
        children: []
    });
    node.Children.forEach((child) => {
        let childNode: TreeNode = serializeFilesSystemToTree(child);
        childNode.Parent = parent;
        parent.Children.push(childNode);
    });
    return parent;
}