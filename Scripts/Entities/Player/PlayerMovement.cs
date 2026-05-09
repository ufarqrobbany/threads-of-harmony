using Godot;

public class PlayerMovement
{
	private CharacterBody3D player;
	private PlayerCameraController camera;
	private PlayerJump jump;
	private PlayerStamina stamina;
	private PlayerAnimation animation;
	
	private bool enabled = true;
	private float speed = 5.0f;
	private float sprintMultiplier = 1.8f;
	private float exhaustedMultiplier = 0.6f;
	private float rotationSpeed = 15.0f;
	
	private Vector3 lastDirection = Vector3.Forward;
	private Vector3 pendingDirection = Vector3.Zero;
	private float directionDelayTimer = 0f;
	private const float DirectionChangeThreshold = 0.01f;
		
	public PlayerMovement(CharacterBody3D player, PlayerCameraController camera, PlayerJump jump, PlayerStamina stamina, PlayerAnimation animation)
	{
		this.player = player;
		this.camera = camera;
		this.jump = jump;
		this.stamina = stamina;
		this.animation = animation;
	}
	
	public void HandleMovement(ref Vector3 velocity, double delta, bool isSprinting)
	{
		if (!enabled || (jump?.IsLanding ?? false))
		{
			StopMovement(ref velocity, delta);
			return;
		}
		
		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_backward", "move_forward");
		Vector3 moveDirection = Vector3.Zero;
		
		if (inputDir != Vector2.Zero)
		{
			moveDirection = GetDirection(inputDir);
			ApplyMovement(ref velocity, moveDirection, isSprinting);
			SmoothDirection(moveDirection, delta);
			
			if (stamina.GetCurrentStamina() <= 0f)
				animation?.Play("Walk");
			else if (isSprinting && stamina.GetCurrentStamina() > 0f)
				animation?.Play("Run");
			else
				animation?.Play("Walk");
		}
		else
		{
			StopMovement(ref velocity, delta);
			animation?.Play("Idle");
		}

		if (lastDirection.LengthSquared() > 0.01f)
			RotateSmoothly(lastDirection, delta);
	}

	private Vector3 GetDirection(Vector2 inputDir)
	{
		if (camera == null)
			return (player.Transform.Basis * new Vector3(inputDir.X, 0, -inputDir.Y)).Normalized();

		Vector3 forward = camera.GlobalTransform.Basis.Z;
		Vector3 right = camera.GlobalTransform.Basis.X;
		forward.Y = right.Y = 0;
		return (right.Normalized() * inputDir.X + forward.Normalized() * -inputDir.Y).Normalized();
	}
	
	private void StopMovement(ref Vector3 velocity, double delta)
	{
		velocity.X = Mathf.MoveToward(velocity.X, 0, speed * (float)delta * 10);
		velocity.Z = Mathf.MoveToward(velocity.Z, 0, speed * (float)delta * 10);
	}
	
	public bool IsMoving()
	{
		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_backward", "move_forward");
		return inputDir != Vector2.Zero;
	}

	private void ApplyMovement(ref Vector3 velocity, Vector3 dir, bool sprinting)
	{
		float currentStamina = stamina?.GetCurrentStamina() ?? 0f;
		float finalSpeed;
		
		if (currentStamina <= 0f)
		{
			finalSpeed = speed * exhaustedMultiplier;
		}
		else if (sprinting && currentStamina > 0f)
		{
			finalSpeed = speed * sprintMultiplier;
		}
		else
		{
			finalSpeed = speed;
		}

		velocity.X = dir.X * finalSpeed;
		velocity.Z = dir.Z * finalSpeed;
	}

	private void SmoothDirection(Vector3 dir, double delta)
	{
		if (lastDirection == Vector3.Zero || dir.Normalized().Dot(lastDirection.Normalized()) < 0.98f)
		{
			if (pendingDirection == Vector3.Zero || pendingDirection.Normalized().Dot(dir.Normalized()) < 0.98f)
			{
				pendingDirection = dir;
				directionDelayTimer = 0f;
			}
			else
			{
				directionDelayTimer += (float)delta;
				if (directionDelayTimer >= DirectionChangeThreshold)
				{
					lastDirection = pendingDirection;
					pendingDirection = Vector3.Zero;
					directionDelayTimer = 0f;
				}
			}
		}
		else
		{
			lastDirection = dir;
			pendingDirection = Vector3.Zero;
			directionDelayTimer = 0f;
		}
	}
	
	private void RotateSmoothly(Vector3 dir, double delta)
	{
		if (dir.LengthSquared() == 0)
			return;
			
		float targetYaw = Mathf.Atan2(dir.X, dir.Z) + Mathf.Pi;
		float currentYaw = player.Rotation.Y;
		float newYaw = Mathf.LerpAngle(currentYaw, targetYaw, (float)delta * rotationSpeed);
		player.Rotation = new Vector3(player.Rotation.X, newYaw, player.Rotation.Z);
	}
	
	public void SetEnabled(bool value)
	{
		enabled = value;
	}
}
