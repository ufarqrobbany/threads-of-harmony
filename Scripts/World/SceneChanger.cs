// Scripts/World/SceneChanger.cs
using Godot;
using System;

public partial class SceneChanger : Area3D
{
	[Export(PropertyHint.File, "*.tscn")] // Memungkinkan Anda memilih file adegan dari editor
	public string TargetScenePath { get; set; } = ""; // Path ke adegan target yang akan dimuat

	[Export]
	public string PlayerNodeName { get; set; } = "Player"; // Nama node pemain yang akan memicu perubahan adegan

	public override void _Ready()
	{
		// Pastikan sinyal body_entered terhubung
		BodyEntered += OnBodyEntered;

		if (string.IsNullOrEmpty(TargetScenePath))
		{
			GD.PrintErr("SceneChanger: TargetScenePath tidak diatur. Tidak ada adegan yang akan dimuat.");
		}
	}

	private void OnBodyEntered(Node3D body)
	{
		// Periksa apakah tubuh yang masuk adalah pemain
		if (body.Name == "Player")
		{
			ChangeScene();
		}
	}

	private void ChangeScene()
	{
		if (!string.IsNullOrEmpty(TargetScenePath))
		{
			GetTree().ChangeSceneToFile(TargetScenePath);
		}
		else
		{
			GD.PrintErr("SceneChanger: TargetScenePath kosong, tidak dapat mengubah adegan.");
		}
	}
}
