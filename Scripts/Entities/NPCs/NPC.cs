using Godot;
using System.Collections.Generic;

public partial class NPC : CharacterBody3D, ITalkable
{
	[Export] public string name { get; set; } = "NPC No.1";
	
	private InteractionPrompt interactionPrompt;
	
	public override void _Ready()
	{
		interactionPrompt = GetNode<InteractionPrompt>("/root/Game/UI/HUD/InteractionPrompt");
	}
		
	public void ShowInteractionPrompt()
	{
		 interactionPrompt.ShowPrompt($"[F]", $"Talk to {name}");
	}
	
	public void HideInteractionPrompt()
	{
		interactionPrompt.HidePrompt();
	}
	
	public void StartConversation()
	{
		// dialog sistem
	}
}
