using Godot;
using System;

public partial class Enemy : CharacterBody3D
{
	[Export] public float VisionAngle = 90f;
	[Export] public float VisionDistance = 20f;
	[Export] public float MoveSpeed = 6f;
	[Export] public float AttackCooldown = 1.5f;
	[Export] public float Gravity = 9.8f;
	[Export] public float AttackDamage = 10f;
	[Export] public float MaxHealth = 50f;
	[Export] public float Defense = 1f;

	private float currentHealth;
	private float attackAnimTime = 0f;
	private float attackDamageMoment = 0.5f;
	private float attackTimer = 0f;
	private bool hasAppliedDamage = false;
	private bool isTaunting = false;
	private bool isChasing = false;
	private bool isAttacking = false;
	private bool playerInAttackArea = false;
	private bool isDead = false;
	
	private float tauntTimer = 0f;
	private const float tauntDuration = 1f;

	private Node3D target;
	private AnimationPlayer animationPlayer;
	private Area3D attackArea;

	private Control healthUI;
	private TextureProgressBar healthBar;
	
	private float displayedHealth;
	private const float healthLerpSpeed = 10f;

	private Player player;

	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("EnemyModel/AnimationPlayer");
		if (animationPlayer != null)
			animationPlayer.AnimationFinished += OnAnimationFinished;

		attackArea = GetNode<Area3D>("AttackArea");
		if (attackArea != null)
		{
			attackArea.BodyEntered += OnAttackAreaEntered;
			attackArea.BodyExited += OnAttackAreaExited;
		}

		healthUI = GetNode<Control>("MeshInstance3D/EnemyHealthViewport/EnemyHealthUI");
		healthBar = healthUI.GetNode<TextureProgressBar>("Node2D/HealthBar");

		currentHealth = MaxHealth;
		displayedHealth = currentHealth;

		UpdateHealthUI(instant: true);

		healthUI.Visible = false;

		player = GetNodeOrNull<Player>("/root/Game/Entities/Player");
	}

	private void UpdateHealthUI(bool instant = false)
	{
		if (healthBar == null && healthUI == null) return;
		
		healthBar.MaxValue = MaxHealth;

		if (instant)
		{
			displayedHealth = currentHealth;
			healthBar.Value = displayedHealth;
		}
		else
		{
			displayedHealth = Mathf.Lerp(displayedHealth, currentHealth, healthLerpSpeed * (float)GetProcessDeltaTime());
			healthBar.Value = displayedHealth;
		}
	}
	
	public void TakeDamage(float amount)
	{
		if (isDead) return;

		currentHealth = Mathf.Max(currentHealth - Mathf.Max(amount - Defense, 0), 0);
		if (currentHealth <= 0)
		{
			currentHealth = 0;
			Die();
		}
	}

	public void Die()
	{
		if (isDead) return;

		isDead = true;
		isAttacking = false;
		isChasing = false;
		isTaunting = false;

		animationPlayer?.Play("Die");
	}

	private void OnAnimationFinished(StringName animName)
	{
		if (animName == "Die")
		{
			var explosion = GetNode<Node3D>("Explosion");

			if (explosion != null)
			{
				this.GetNode<Node3D>("EnemyModel").Visible = false;
				explosion.GetNode<GpuParticles3D>("Debris").Emitting = true;
				explosion.GetNode<GpuParticles3D>("Smoke").Emitting = true;
				explosion.GetNode<GpuParticles3D>("Fire").Emitting = true;
			}

			healthUI?.QueueFree();

			GetTree().CreateTimer(1.0).Timeout += () => QueueFree();
		}
	}

	private void OnAttackAreaEntered(Node body)
	{
		if (body.Name == "Player" && body == target)
		{
			if(isDead == false)
			{
				playerInAttackArea = true;
				isAttacking = true;
				isChasing = false;
			}	
		}
	}

	private void OnAttackAreaExited(Node body)
	{
		if (body.Name == "Player" && body == target)
		{
			if(isDead == false)
			{
				playerInAttackArea = false;
				isAttacking = false;
				isChasing = true;
			}
		}
	}

	private bool IsPlayerVisible()
	{
		if (player == null)
			return false;

		Vector3 toPlayer = (player.GlobalTransform.Origin - GlobalTransform.Origin).Normalized();
		Vector3 forward = -GlobalTransform.Basis.Z;

		float dot = forward.Dot(toPlayer);
		float angle = Mathf.RadToDeg(Mathf.Acos(dot));
		float distance = GlobalTransform.Origin.DistanceTo(player.GlobalTransform.Origin);

		return angle < VisionAngle / 2 && distance <= VisionDistance;
	}
	
	public override void _Process(double delta)
	{
		var mesh = GetNode<Node3D>("MeshInstance3D");
		var cam = GetViewport().GetCamera3D();

		if (mesh != null && cam != null)
		{
			mesh.LookAt(cam.GlobalTransform.Origin, Vector3.Up);
			mesh.RotateY(Mathf.Pi);
			mesh.RotateZ(Mathf.Pi);
			mesh.RotateX(Mathf.Pi);
		}
		
		UpdateHealthUI();
	}


	public override void _PhysicsProcess(double delta)
	{
		float deltaTime = (float)delta;

		if (!isTaunting && !isChasing && !isAttacking && IsPlayerVisible())
		{
			if (isDead) return;

			target = player;
			isTaunting = true;
			tauntTimer = tauntDuration;

			LookAt(new Vector3(player.GlobalTransform.Origin.X, GlobalTransform.Origin.Y, player.GlobalTransform.Origin.Z), Vector3.Up);
			// Jangan mainkan animasi taunt jika tidak dibutuhkan
		}

		if (!IsOnFloor())
			Velocity = new Vector3(Velocity.X, Velocity.Y - Gravity * deltaTime, Velocity.Z);
		else
			Velocity = new Vector3(Velocity.X, 0, Velocity.Z);

		if (isAttacking && playerInAttackArea)
		{
			if(isDead == true) return;
			attackTimer -= deltaTime;

			if (attackTimer <= 0f)
			{
				if (animationPlayer?.CurrentAnimation != "Punch")
				{
					LookAt(new Vector3(player.GlobalTransform.Origin.X, GlobalTransform.Origin.Y, player.GlobalTransform.Origin.Z), Vector3.Up);
					animationPlayer?.Play("Punch");

					attackAnimTime = 0f;
					hasAppliedDamage = false;
				}
				attackTimer = AttackCooldown;
			}

			if (animationPlayer?.CurrentAnimation == "Punch")
			{
				attackAnimTime += deltaTime;

				if (!hasAppliedDamage && attackAnimTime >= attackDamageMoment)
				{
					player?.TakeDamage(AttackDamage);
					hasAppliedDamage = true;
				}
			}

			Velocity = new Vector3(0, Velocity.Y, 0);
		}
		else if (isChasing && target != null)
		{
			if(isDead == true) return;
			
			Vector3 direction = (target.GlobalTransform.Origin - GlobalTransform.Origin).Normalized();
			LookAt(GlobalTransform.Origin + new Vector3(direction.X, 0, direction.Z), Vector3.Up);
			Velocity = new Vector3(direction.X * MoveSpeed, Velocity.Y, direction.Z * MoveSpeed);

			if (animationPlayer?.CurrentAnimation != "Run")
				animationPlayer?.Play("Run");

			attackTimer = 0f;
		}
		else
		{
			if(isDead == true) return;
			
			Velocity = new Vector3(0, Velocity.Y, 0);
			if (animationPlayer?.CurrentAnimation == "Run")
				animationPlayer?.Stop();
		}

		if (healthUI != null)
			healthUI.Visible = isChasing || isTaunting || isAttacking;

		if (isTaunting)
		{
			tauntTimer -= deltaTime;
			if (tauntTimer <= 0f)
			{
				isTaunting = false;
				isChasing = true;
			}
		}

		MoveAndSlide();
	}
}
