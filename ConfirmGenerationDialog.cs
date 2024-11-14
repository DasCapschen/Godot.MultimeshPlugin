#if TOOLS
using Godot;
using System.Collections.Generic;
using System.Linq;
using Game.extensions;

namespace MultimeshPlugin;

[Tool]
public partial class ConfirmGenerationDialog : ConfirmationDialog
{
    public Node3D RootNode { get; set; }
    
    private Control _container;
    private Dictionary<Mesh, List<MeshInstance3D>> _foundMeshes;
    private Dictionary<Mesh, FoundMeshRow> _foundMeshSettings;

    private Node3D _generatedMultimeshRoot;
    private Node3D _generatedCollisionRoot;
    
    public override void _Ready()
    {
        _container = GetNode<Control>("VBoxContainer");
        GetOkButton().Pressed += PerformGeneration;
    }

    public void SetFoundMeshes(Dictionary<Mesh, List<MeshInstance3D>> foundMeshes)
    {
        _foundMeshes = foundMeshes;
        _foundMeshSettings = new Dictionary<Mesh, FoundMeshRow>();
        
        var foundMeshRowScene = GD.Load<PackedScene>("res://addons/multimesh_plugin/FoundMeshRow.tscn");
        
        foreach (var (mesh, count, index) in foundMeshes.Select((kvp, index) => (kvp.Key, kvp.Value.Count, index)))
        {
            var row = foundMeshRowScene.Instantiate<FoundMeshRow>();
            _container.AddChild(row);
            
            row.SetMesh(mesh);
            row.SetInstanceCount(count);

            _foundMeshSettings[mesh] = row;

            if (index < foundMeshes.Count - 1)
            {
                _container.AddChild(new Panel { CustomMinimumSize = new Vector2(0, 2) });
            }
        }
    }

    private void PerformGeneration()
    {
        if (RootNode is null)
        {
            GD.PrintErr("PerformGeneration: RootNode is null! Aborting...");
            return;
        }
        
        var editedSceneRoot = GetTree().GetEditedSceneRoot();
        
        _generatedMultimeshRoot = new Node3D { Name = "Generated Multimeshes" };
        RootNode.AddChild(_generatedMultimeshRoot);
        _generatedMultimeshRoot.Owner = editedSceneRoot;
        _generatedMultimeshRoot.SetMeta(nameof(MultimeshInspectorPlugin), true);

        _generatedCollisionRoot = new Node3D { Name = "Generated Collisions" };
        RootNode.AddChild(_generatedCollisionRoot);
        _generatedCollisionRoot.Owner = editedSceneRoot;
        _generatedCollisionRoot.SetMeta(nameof(MultimeshInspectorPlugin), true);
        
        foreach (var (mesh, settings) in _foundMeshSettings)
        {
            if (settings.GenerateMultimesh)
            {
                GenerateMultimesh(mesh, _foundMeshes[mesh]);
            }

            if (settings.GenerateCollision)
            {
                GenerateCollision(mesh, _foundMeshes[mesh]);
            }
        }
    }

    private void GenerateMultimesh(Mesh mesh, List<MeshInstance3D> instances)
    {
        var editedSceneRoot = GetTree().GetEditedSceneRoot();
        
        var multiMesh = new MultiMesh
        {
            Mesh = mesh,
            TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
            InstanceCount = instances.Count,
        };

        foreach (var (instance, index) in instances.Enumerate())
        {
            multiMesh.SetInstanceTransform(index, instance.GlobalTransform);
        }

        var multiMeshInstance = new MultiMeshInstance3D { Multimesh = multiMesh };
        _generatedMultimeshRoot.AddChild(multiMeshInstance);
        multiMeshInstance.Owner = editedSceneRoot;
    }

    private void GenerateCollision(Mesh mesh, List<MeshInstance3D> instances)
    {
        var editedSceneRoot = GetTree().GetEditedSceneRoot();
        var triMesh = mesh.CreateTrimeshShape();
        
        foreach (var (instance, index) in instances.Enumerate())
        {
            var collisionShape = new CollisionShape3D { Shape = triMesh };
            var staticBody = new StaticBody3D
            {
                Name = $"StaticBody {index}",
                GlobalTransform = instance.GlobalTransform,
            };
			
            _generatedCollisionRoot.AddChild(staticBody);
            staticBody.Owner = editedSceneRoot;
            
            staticBody.AddChild(collisionShape);
            collisionShape.Owner = editedSceneRoot;
        }
    }
}
#endif
