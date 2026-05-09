using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class NarrationScene : CanvasLayer
{
	[Export] public Label NarrationLabel;
	[Export] public AnimationPlayer AnimPlayer;

	private List<string> lines = new List<string>
	{
		"Long ago, the world was woven by three unseen threads: Nature, Order, and Spirit. Together, they kept the fabric of existence in perfect harmony.",
		"But greed tore through the weave. Nature cried out in rage, order crumbled, and the spirits of the old began to fade. The balance was lost, and the world began to fracture.",
		"In the midst of this unraveling, a lone soul rises—not powerful, but determined; not perfect, but willing to act. You are the thread of hope.",
		"Step into the silent depths. Face echoes of the past, heed the call of forgotten spirits, and begin the journey to restore what was broken.",
		"Only by reweaving the threads can the world heal from its deepest wounds."
	};

	public override async void _Ready()
	{
		NarrationLabel.Text = "";
		
		await Task.Delay(2000); 

		foreach (string line in lines)
		{
			NarrationLabel.Text = line;
			AnimPlayer.Play("FadeIn");
			await ToSignal(AnimPlayer, "animation_finished");

			await Task.Delay(6000); 

			AnimPlayer.Play("FadeOut");
			await ToSignal(AnimPlayer, "animation_finished");
		}

		GetTree().ChangeSceneToFile("res://Scenes/World/Dungeon.tscn");
	}
}
