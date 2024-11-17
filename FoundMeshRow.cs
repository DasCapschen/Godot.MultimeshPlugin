#if TOOLS
using System;
using Godot;

namespace MultimeshPlugin;

[Tool]
public partial class FoundMeshRow : HBoxContainer
{
    public bool GenerateMultimesh => _generateMultimeshCheckbox.IsPressed();
    public bool GenerateCollision => _generateCollisionCheckbox.IsPressed();
    public bool GenerateNavObstacle => _generateNavigationObstacleCheckbox.IsPressed();

    private Label _instanceCountLabel;
    private CheckBox _generateCollisionCheckbox;
    private CheckBox _generateMultimeshCheckbox;
    private CheckBox _generateNavigationObstacleCheckbox;
    private TextureRect _meshPreview;
    
    public override void _Ready()
    {
        _instanceCountLabel = GetNode<Label>("VBoxContainer/InstanceCount");
        _generateCollisionCheckbox = GetNode<CheckBox>("VBoxContainer/GenerateCollisionsCheckbox");
        _generateMultimeshCheckbox = GetNode<CheckBox>("VBoxContainer/GenerateMultimeshCheckbox");
        _generateNavigationObstacleCheckbox = GetNode<CheckBox>("VBoxContainer/GenerateNavigationObstacleCheckbox");
        
        _meshPreview = GetNode<TextureRect>("MeshPreview");
    }

    public void SetMesh(Mesh mesh)
    {
        var previewer = EditorInterface.Singleton.GetResourcePreviewer();
        previewer.QueueEditedResourcePreview(mesh, this, MethodName.SetPreviewImage, new Variant());
    }

    public void SetPreviewImage(string path, Texture2D preview, Texture2D thumbnail, Variant userData)
    {
        _meshPreview.Texture = preview ?? thumbnail ?? throw new ArgumentNullException(nameof(thumbnail), "Resource Preview was not generated!!");
    }

    public void SetInstanceCount(int count)
    {
        _instanceCountLabel.Text = $"Found {count} instances of this mesh.";
    }
}
#endif
