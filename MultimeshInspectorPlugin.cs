#if TOOLS
using System.Collections.Generic;
using Game.extensions;
using Godot;

namespace MultimeshPlugin;


public partial class MultimeshInspectorPlugin : EditorInspectorPlugin
{
    public override bool _CanHandle(GodotObject obj)
    {
        return obj is Node3D;
    }

    public override void _ParseBegin(GodotObject obj)
    {
	    var controlScene = GD.Load<PackedScene>("res://addons/multimesh_plugin/GenerateButton.tscn");
	    var control = controlScene.Instantiate<ScanForMeshes>();
	    control.RootNode = obj as Node3D;
	    
        AddCustomControl(control);
    }

    private void CreateMultimeshFromChildren(Node parent)
    {
	    foreach (var child in parent.GetChildren(includeInternal: true))
	    {
		    if (child.HasMeta(nameof(MultimeshInspectorPlugin)))
		    {
			    child.QueueFree();
		    }
	    }

	    var meshTypes = new Dictionary<Mesh, List<MeshInstance3D>>();

    	var stack = new Stack<Node>(parent.GetChildren());
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

	    var editedSceneRoot = parent.GetTree().GetEditedSceneRoot();
	    
	    var generatedNode = new Node3D { Name = "GeneratedByMultimeshPlugin" };
	    generatedNode.SetMeta(nameof(MultimeshInspectorPlugin), true);
	    parent.AddChild(generatedNode);
	    generatedNode.Owner = editedSceneRoot;
	    
	    var multimeshNode = new Node3D { Name = "Multimeshes" };
	    generatedNode.AddChild(multimeshNode);
	    multimeshNode.Owner = editedSceneRoot;
	    
	    var collisionNode = new Node3D { Name = "Collisions" };
	    generatedNode.AddChild(collisionNode);
	    collisionNode.Owner = editedSceneRoot;
	    
    	foreach (var (mesh, instances) in meshTypes)
    	{
    		var multiMesh = new MultiMesh
    		{
    			Mesh = mesh,
    			TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
    			InstanceCount = instances.Count,
    		};
		    var triMesh = mesh.CreateTrimeshShape();

    		foreach (var (instance, index) in instances.Enumerate())
		    {
			    var collisionShape = new CollisionShape3D { Shape = triMesh };
			    var staticBody = new StaticBody3D { Name = $"StaticBody {index}" };
			    
			    staticBody.AddChild(collisionShape);
			    collisionShape.Owner = editedSceneRoot;
			    
			    collisionNode.AddChild(staticBody);
			    staticBody.Owner = editedSceneRoot;
			    
    			multiMesh.SetInstanceTransform(index, instance.GlobalTransform);
    		}

    		var multiMeshInstance = new MultiMeshInstance3D { Multimesh = multiMesh };
		    multimeshNode.AddChild(multiMeshInstance);
		    multiMeshInstance.Owner = editedSceneRoot;
	    }
    }
}
#endif
