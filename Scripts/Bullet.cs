using Godot;
using System;

public partial class Bullet : Area3D
{
	[Export] public float Speed = 20.0f;
	[Export] public float Lifetime = 4.0f; // Max time bullet lives (seconds)

	private double _timeAlive = 0.0;
	private bool _hasHit = false;
	
	public override void _Ready()
	{
		// Play fireball sound when bullet is spawned
		var fireballSound = GetNode<AudioStreamPlayer3D>("FireballSound");
		if (fireballSound != null)
		{
			fireballSound.Play();
		}
	}

	// Called every physics frame. Use for movement.
	public override void _PhysicsProcess(double delta)
	{
		// Godot convention: -Z is forward for nodes.
		Vector3 forwardDirection = -GlobalTransform.Basis.Z;
		GlobalPosition += forwardDirection * Speed * (float)delta;

		_timeAlive += delta;
		if (_timeAlive >= Lifetime)
		{
			QueueFree(); // Destroy bullet if lifetime expires
		}
	}

	private async void _on_body_entered(Node3D body)
	{
		if (_hasHit) return;
		_hasHit = true;

		if (body.IsInGroup("targets"))
		{
			GameManager gm = GetTree().Root.GetNode<GameManager>("/root/Node3D");

			// Coba ambil AnimationPlayer dari target
			AnimationPlayer animPlayer = body.GetNodeOrNull<AnimationPlayer>("Character/AnimationPlayer");

			if (body is RigidBody3D targetRb)
			{
				targetRb.Freeze = false;
				targetRb.Sleeping = false;

				//Vector3 bulletDirection = GlobalTransform.Basis.Z.Normalized();
				//targetRb.ApplyCentralImpulse(-bulletDirection * 5f);

				// Mainkan animasi Death_A jika ada
				animPlayer?.Play("Death_A");

				var timer = GetTree().CreateTimer(1); // tunggu animasi selesai
				await ToSignal(timer, "timeout");

				if (GodotObject.IsInstanceValid(body))
				{
					//gm?.AddScore(10);
					body.QueueFree();
				}
			}
		}

		// Hapus bullet setelah mengenai sesuatu
		QueueFree();
	}
}
