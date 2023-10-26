using Godot;

public static class NodeExtensions
{
    public static T GetChildByType<T>(this Node node, bool recursive = true)
        where T : Node
    {
        var childCount = node.GetChildCount();

        for (var i = 0; i < childCount; i++)
        {
            var child = node.GetChild(i);
            if (child is T typedChild)
                return typedChild;

            if (recursive is false || child.GetChildCount() == 0) 
                continue;
            
            var recursiveResult = child.GetChildByType<T>();
            
            if (recursiveResult != null)
                return recursiveResult;
        }

        return null;
    }
    
    public static T GetParentByType<T>(this Node node)
        where T : Node
    {
        var parent = node.GetParent();
        switch (parent)
        {
            case null:
                return null;
            case T typedParent:
                return typedParent;
            default:
                return parent.GetParentByType<T>();
        }
    }
}
