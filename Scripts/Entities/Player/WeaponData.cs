// WeaponData.cs
using Godot;

[GlobalClass]
public partial class WeaponData : Resource
{
	[Export] public string WeaponName { get; set; }
	[Export] public float AttackBonus { get; set; }
	[Export] public float DefenseBonus { get; set; }
	[Export] public float HealthBonus { get; set; }
	[Export(PropertyHint.File, "*.tscn,*.glb,*.gltf")] public string ModelPath { get; set; } // Path to the weapon's 3D model
	[Export] public string EquippedBoneName { get; set; } = "WeaponAttach"; // Default bone to attach to
	// Add other properties like attack animation name if different per weapon
}
