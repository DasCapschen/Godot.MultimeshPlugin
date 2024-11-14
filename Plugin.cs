#if TOOLS
using Godot;

namespace MultimeshPlugin;

[Tool]
public partial class Plugin : EditorPlugin
{
    private MultimeshInspectorPlugin _plugin;

    public override void _EnterTree()
    {
        _plugin = new MultimeshInspectorPlugin();
        AddInspectorPlugin(_plugin);
    }

    public override void _ExitTree()
    {
        RemoveInspectorPlugin(_plugin);
    }
}
#endif
