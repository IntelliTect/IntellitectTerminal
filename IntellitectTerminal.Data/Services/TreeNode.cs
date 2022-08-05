using System.Collections.ObjectModel;

namespace IntellitectTerminal.Data.Services;

public class TreeNode<T>
{
    private readonly T name;
    private readonly List<TreeNode<T>> children = new();
    public TreeNode<T>? Parent { get; private set; }
    public bool isFile { get; set; }

    public TreeNode(T value)
    {
        name = value;
        isFile = false;
    }

    public TreeNode(T value, bool isFile)
    {
        name = value;
        this.isFile = isFile;
    }

    public TreeNode<T> this[int i]
    {
        get { return children[i]; }
    }


    public T Value { get { return name; } }

    public ReadOnlyCollection<TreeNode<T>> Children
    {
        get { return children.AsReadOnly(); }
    }

    public TreeNode<T> AddChild(T value)
    {
        var node = new TreeNode<T>(value) { Parent = this };
        children.Add(node);
        return node;
    }

    public TreeNode<T> AddChild(T value, bool isFile)
    {
        var node = new TreeNode<T>(value, isFile) { Parent = this };
        children.Add(node);
        return node;
    }

    public TreeNode<T>[] AddChildren(params T[] values)
    {
        return values.Select(AddChild).ToArray();
    }

    public bool RemoveChild(TreeNode<T> node)
    {
        return children.Remove(node);
    }

    public void Traverse(Action<T> action)
    {
        action(Value);
        foreach (var child in children)
            child.Traverse(action);
    }

    public IEnumerable<T> Flatten()
    {
        return new[] { Value }.Concat(children.SelectMany(x => x.Flatten()));
    }
    public TreeNode<T> InsertChild(TreeNode<T> parent, T value) 
    { 
        var node = new TreeNode<T>(value) { Parent = parent }; 
        parent.children.Add(node); 
        return node; 
    }
}
