using Godot;

public class PlayerCollision
{
	private CharacterBody3D player;
	private float pushForce = 5.0f;

	public PlayerCollision(CharacterBody3D player)
	{
		this.player = player;
	}

	public void HandleCollision(Vector3 velocity, double delta)
	{
		var collision = player.MoveAndCollide(velocity * (float)delta);
		if (collision?.GetCollider() is RigidBody3D rigidBody)
		{
			Vector3 pushDir = velocity.Normalized();
			pushDir.Y = 0;
			rigidBody.ApplyCentralImpulse(pushDir * pushForce);
		}
	}
}
