#if TOOLS
using System.Collections.Generic;
using MultimeshPlugin.Extensions;
using Godot;

namespace MultimeshPlugin;

[Tool]
public partial class ScanForMeshes : Button
{
    public Node3D? RootNode { get; set; }
    
    public override void _Ready()
    {
        Pressed += PerformScan;
    }
    
    private void PerformScan()
    {
        if (RootNode is null)
        {
            GD.PrintErr("ScanForMeshes: RootNode is null! Aborting...");
            return;
        }

        var meshTypes = new Dictionary<Mesh, List<MeshInstance3D>>();

        var stack = new Stack<Node>(RootNode.GetChildren());
        while (stack.Count > 0)
        {
            var child = stack.Pop();
            if (child.HasMeta(nameof(MultimeshInspectorPlugin))) continue; // ignore generated nodes
		    
            stack.PushRange(child.GetChildren());
		    
            if (child is MeshInstance3D { Mesh: not null } meshInstance)
            {
                if (!meshTypes.ContainsKey(meshInstance.Mesh))
                {
                    meshTypes[meshInstance.Mesh] = new List<MeshInstance3D>();
                }

                meshTypes[meshInstance.Mesh].Add(meshInstance);
            }
        }

        var dialogScene = GD.Load<PackedScene>("res://addons/multimesh_plugin/ConfirmGenerationDialog.tscn");
        var dialog = dialogScene.Instantiate<ConfirmGenerationDialog>();
        GetTree().GetRoot().AddChild(dialog);
        dialog.PopupCentered();
        dialog.RootNode = RootNode;
        dialog.SetFoundMeshes(meshTypes);
    }
}
#endif
