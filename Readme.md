# Multimesh Plugin
- this is a C# plugin -> you need to use Godot with C# support!
- place these files in `res://addons/multimesh_plugin/`
- activate the plugin in Project Settings > Plugins
- Select a Node3D
- Press the button "Generate Multimesh from Children"
- ![image](https://github.com/user-attachments/assets/7dd27eec-6b48-428b-a57c-89d8aab6aecd)
- A Dialog will appear, telling you which meshes were found in the children
- ![image](https://github.com/user-attachments/assets/f2342fd2-ba2b-438b-9bbe-900e760e4520)
- Select what you want to generate and press OK.
- Your selected Node3D will have 2 new Children: "Generated Multimeshes" and "Generated Collisions". The source meshes are **not** deleted!
- ![image](https://github.com/user-attachments/assets/fa6b8149-5295-4251-a308-501ec583d8dc)
