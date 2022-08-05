using IntellitectTerminal.Data.Models;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace IntellitectTerminal.Data.Services;

public class TreeNode<T>
{
    private readonly T name;
    private readonly List<TreeNode<T>> children = new();
    public TreeNode<T>? Parent { get; private set; }
    public bool isFile { get; set; }

    public TreeNode(T name)
    {
        this.name = name;
        isFile = false;
    }

    public TreeNode(T name, bool isFile)
    {
        this.name = name;
        this.isFile = isFile;
    }

    [JsonConstructor]
    public TreeNode(T name, bool isFile, List<TreeNode<T>> children, TreeNode<T>? parent)
    {
        this.name = name;
        this.isFile = isFile;
        this.children = children;
        this.Parent = parent;
    }

    public TreeNode<T> this[int i]
    {
        get { return children[i]; }
    }


    public T Name { get { return name; } }

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

    public IEnumerable<TreeNode<T>> Traverse(Func<TreeNode<T>, TreeNode<T>> action)
    {
        yield return action(this);
        foreach (var child in children)
            foreach (var child1 in child.Traverse(action))
                yield return child1;
    }

    public IEnumerable<T> Flatten()
    {
        return new[] { Name }.Concat(children.SelectMany(x => x.Flatten()));
    }
    public TreeNode<T> InsertChild(TreeNode<T> parent, T value)
    {
        var node = new TreeNode<T>(value) { Parent = parent };
        parent.children.Add(node);
        return node;
    }
    public static TreeNode<string> GetChild(TreeNode<string> foundUsersFileSystem, string dirOrFileName, bool isFile)
    {
        IEnumerable<TreeNode<string>> traverseResult = foundUsersFileSystem.Traverse(delegate (TreeNode<string> node)
        {
            return node.Children.FirstOrDefault(x => x.Name == dirOrFileName && x.isFile == isFile);
        }).Where(x=>x!=null);
        return traverseResult.First();
    }
    public static TreeNode<string> DeserializeFileSystem(User foundUser)
    {
        return JsonConvert.DeserializeObject<TreeNode<string>>(foundUser.FileSystem) ?? throw new InvalidOperationException("Deserializing User File System Failed");
    }
    public static TreeNode<string> DeserializeFileSystem(string fileStructure)
    {
        return JsonConvert.DeserializeObject<TreeNode<string>>(fileStructure) ?? throw new InvalidOperationException("Deserializing User File System Failed");
    }
    public static string SerializeFileSystem(TreeNode<string> fileStructure)
    {
        return JsonConvert.SerializeObject(fileStructure, new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });
    }
}
