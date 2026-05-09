using Godot;
using System;

public partial class NPCInteraction : Area3D
{
	private NPC npc;
	private bool playerInRange = false;
	
	public override void _Ready()
	{
		npc = GetParent<NPC>();
	}
	
	private void OnBodyEntered(Node3D body)
	{
		if (body.Name == "Player")
		{
			playerInRange = true;
			npc.ShowInteractionPrompt();
		}			
	}

	private void OnBodyExited(Node3D body)
	{
		if (body.Name == "Player")
		{
			playerInRange = false;
			npc.HideInteractionPrompt();
		}	
	}
	
	public override void _Process(double delta)
	{
		if (playerInRange && Input.IsActionJustPressed("interact"))
		{
			npc.StartConversation();
		}
	}
}
