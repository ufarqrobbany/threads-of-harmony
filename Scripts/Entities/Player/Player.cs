using Godot;
using System;
using System.Collections.Generic; // Added for List

public partial class Player : CharacterBody3D
{
	[Export] public NodePath CameraPath { get; set; }
	
	private PlayerCameraController camera;
	private PlayerAnimation animation;
	private PlayerMovement movement;
	private PlayerJump jump;
	private PlayerHealth health;
	private PlayerStamina stamina;
	private PlayerAttack attack;
	private PlayerCollision collision;
	private Inventory inventory; // Add Inventory instance
	
	private bool canMove = true;
	
	private HealthUI healthUI;
	private StaminaUI staminaUI;

	// New: Reference to the PlayerModel and current equipped weapon node
	private Node3D playerModel;
	private Node3D currentEquippedWeaponNode;

	public override void _Ready()
	{
		camera = GetNodeOrNull<PlayerCameraController>(CameraPath);
		
		animation = new PlayerAnimation(GetNode<AnimationPlayer>("PlayerModel/AnimationPlayer"));
		health = new PlayerHealth(animation);
		stamina = new PlayerStamina();
		jump = new PlayerJump(this, animation);
		attack = new PlayerAttack(this, animation, health);
		movement = new PlayerMovement(this, camera, jump, stamina, animation);
		collision = new PlayerCollision(this);

		inventory = new Inventory(); // Initialize inventory
		AddChild(inventory); // Add inventory as a child node if you want it in the scene tree
		inventory.WeaponEquipped += OnWeaponEquipped; // Subscribe to weapon equipped event
		
		health.OnPlayerDied += HandlePlayerDied;
		attack.OnAttackStateChanged += HandleAttackStateChanged;
				
		healthUI = GetNode<HealthUI>("/root/Game/UI/HUD/HealthUI");
		staminaUI = GetNode<StaminaUI>("/root/Game/UI/HUD/StaminaUI");

		playerModel = GetNode<Node3D>("PlayerModel"); // Get the PlayerModel node

		// Example: Add a starting weapon or for testing
		// Make sure you have a WeaponData.cs and a corresponding .tres file (e.g., res://Weapons/Sword.tres)
		WeaponData startingWeapon = GD.Load<WeaponData>("res://Assets/Weapons/Kujang.tres"); 
		if (startingWeapon != null)
		{
			inventory.AddWeapon(startingWeapon);
			inventory.EquipWeapon(startingWeapon);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;
		
		animation.Update((float)delta);
		
		jump.ApplyGravity(ref velocity, delta);
		
		if (canMove)
		{
			movement.HandleMovement(ref velocity, delta, stamina.IsSprinting());
			jump.HandleJump(ref velocity);
		} else {
			velocity.X = 0;
			velocity.Z = 0;
		}
		
		collision.HandleCollision(velocity, delta);

		Velocity = velocity;
		MoveAndSlide();
	}

	public override void _Process(double delta)
	{
		stamina.HandleStamina(delta, movement.IsMoving());
		attack.Update(delta);
		
		float currentStamina = stamina.GetCurrentStamina();
		float maxStamina = stamina.GetMaxStamina();
		
		staminaUI?.UpdateStamina(currentStamina, maxStamina);
		
		if (stamina.IsSprinting() || currentStamina < maxStamina)
			staminaUI?.ShowStamina();
			
		// Simulasi input untuk health
		if (Input.IsActionJustPressed("debug_take_damage"))
		{
			TakeDamage(10f);
		}
		else if (Input.IsActionJustPressed("debug_heal"))
		{
			health.Heal(5f);
		}
		
		attack.HandleAttack();

		healthUI.UpdateHealth(health.CurrentHealth, health.MaxHealth);

		// Debug input for equipping weapons
		// Make sure you have another WeaponData .tres file (e.g., res://Weapons/Axe.tres)
		if (Input.IsActionJustPressed("debug_equip_weapon_A"))
		{
			WeaponData weaponA = GD.Load<WeaponData>("res://Assets/Weapons/Kujang.tres"); 
			if (weaponA != null)
			{
				inventory.AddWeapon(weaponA);
				inventory.EquipWeapon(weaponA);
			}
		}
		else if (Input.IsActionJustPressed("debug_unequip_weapon"))
		{
			inventory.EquipWeapon(null);
		}
	}
	
	public void DisableControl()
	{
		movement?.SetEnabled(false);
		jump?.SetEnabled(false);		
		camera?.SetEnabled(false);
		attack.IsEnabled = false;
	}
	
	private void HandleAttackStateChanged(bool attacking)
	{
		canMove = !attacking;
		movement?.SetEnabled(!attacking);
		jump?.SetEnabled(!attacking);
	}
	
	private void HandlePlayerDied()
	{
		DisableControl();
		GetNode<GameManager>("/root/Game").GameOver();
	}
	
	public void TakeDamage(float amount)
	{
		health.TakeDamage(amount);
	}

	private void OnWeaponEquipped(WeaponData newWeapon)
	{
		health.UpdateStatsFromWeapon(newWeapon);
		attack.UpdateStatsFromWeapon(newWeapon);
		EquipWeaponVisual(newWeapon); // Call new method to handle visual equipping
	}

	private void EquipWeaponVisual(WeaponData weapon)
	{
		// Hapus Node3D perantara (weaponSocket) sebelumnya jika ada
		if (currentEquippedWeaponNode != null && Godot.GodotObject.IsInstanceValid(currentEquippedWeaponNode))
		{
			currentEquippedWeaponNode.QueueFree();
			currentEquippedWeaponNode = null;
		}

		if (weapon != null && !string.IsNullOrEmpty(weapon.ModelPath))
		{
			var weaponScene = GD.Load<PackedScene>(weapon.ModelPath);
			if (weaponScene != null)
			{
				Node3D newWeaponInstance = weaponScene.Instantiate<Node3D>();
				
				Node boneAttachment = playerModel.FindChild(weapon.EquippedBoneName, recursive: true);
				
				if (boneAttachment != null && boneAttachment is BoneAttachment3D)
				{
					// --- BARU: Tambahkan Node3D perantara (WeaponSocket) di sini ---
					Node3D weaponSocket = new Node3D();
					weaponSocket.Name = "WeaponSocket_" + weapon.WeaponName; // Beri nama unik untuk debugging
					boneAttachment.AddChild(weaponSocket);

					// Posisikan, rotasi, dan skala weapon di dalam weaponSocket ini
					// Anda mungkin perlu menyesuaikan nilai-nilai ini tergantung model weapon dan bone attachment Anda
					weaponSocket.Scale = new Vector3(1f, 1f, 1f); // Atur skala awal weaponSocket
					// Misalnya, jika weapon Anda awalnya kecil, coba nilai seperti:
					// weaponSocket.Scale = new Vector3(10f, 10f, 10f); 

					// Sesuaikan rotasi jika weapon menghadap arah yang salah setelah menempel
					// weaponSocket.RotationDegrees = new Vector3(0, 90, 0); // Contoh rotasi 90 derajat di sumbu Y

					// Sesuaikan posisi jika weapon perlu offset dari titik attach tulang
					// weaponSocket.Position = new Vector3(0.1f, -0.2f, 0.05f); 

					// Tambahkan instance weapon ke dalam weaponSocket
					weaponSocket.AddChild(newWeaponInstance);
					
					// currentEquippedWeaponNode sekarang akan merujuk ke weaponSocket
					currentEquippedWeaponNode = weaponSocket; 
					GD.Print($"Equipped visual weapon: {weapon.WeaponName} to bone: {weapon.EquippedBoneName}");
				}
				else
				{
					GD.PrintErr($"Could not find BoneAttachment '{weapon.EquippedBoneName}' on PlayerModel or it's not a BoneAttachment3D.");
					newWeaponInstance.QueueFree();
				}
			}
			else
			{
				GD.PrintErr($"Failed to load weapon model from path: {weapon.ModelPath}");
			}
		}
		else
		{
			GD.Print("No weapon or model path specified, unequipping visual weapon.");
		}
	}
}
