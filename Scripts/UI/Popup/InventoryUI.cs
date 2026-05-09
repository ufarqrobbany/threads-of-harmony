// Scripts/UI/InventoryUI.cs
using Godot;
using System;
using Godot.Collections; // Required for Godot.Collections.Array

public partial class InventoryUI : Control
{
	//[Export] public PackedScene WeaponItemScene; // Assign a packed scene for a single weapon item (see step 3)
//
	//private VBoxContainer weaponListContainer;
	//private Label equippedWeaponNameLabel;
	//private Label equippedWeaponAttackLabel;
	//private Label equippedWeaponDefenseLabel;
	//private Label equippedWeaponHealthLabel;
	//private TextureButton closeButton;
//
	//private Player player; // Reference to the Player script
	//private Inventory playerInventory; // Reference to the Player's Inventory component
//
	//public override void _Ready()
	//{
		//weaponListContainer = GetNode<VBoxContainer>("Panel/ScrollContainer/WeaponListContainer");
		//equippedWeaponNameLabel = GetNode<Label>("Panel/EquippedWeaponStats/WeaponNameLabel");
		//equippedWeaponAttackLabel = GetNode<Label>("Panel/EquippedWeaponStats/AttackLabel");
		//equippedWeaponDefenseLabel = GetNode<Label>("Panel/EquippedWeaponStats/DefenseLabel");
		//equippedWeaponHealthLabel = GetNode<Label>("Panel/EquippedWeaponStats/HealthLabel");
		//closeButton = GetNode<TextureButton>("Panel/CloseButton");
//
		//closeButton.Pressed += HideInventory;
//
		//Visible = false; // Start hidden
//
		//// Get Player and Inventory references (adjust path as needed)
		//player = GetNode<Player>("/root/Game/Entities/Player");
		//if (player != null)
		//{
			//playerInventory = player.GetNode<Inventory>("Inventory"); // Assuming Inventory is a child of Player
			//if (playerInventory != null)
			//{
				//playerInventory.InventoryChanged += UpdateWeaponList;
				//playerInventory.WeaponEquipped += UpdateEquippedWeaponDisplay;
			//}
			//else
			//{
				//GD.PrintErr("Inventory node not found as child of Player!");
			//}
		//}
		//else
		//{
			//GD.PrintErr("Player node not found at /root/Game/Entities/Player!");
		//}
	//}
//
	//public override void _Input(InputEvent @event)
	//{
		//if (@event.IsActionJustPressed("toggle_inventory")) // Define "toggle_inventory" in Project Settings -> Input Map
		//{
			//if (Visible)
			//{
				//HideInventory();
			//}
			//else
			//{
				//ShowInventory();
			//}
		//}
	//}
//
	//public void ShowInventory()
	//{
		//Visible = true;
		//Input.MouseMode = Input.MouseModeEnum.Visible;
		//GetTree().Paused = true; // Pause the game when inventory is open
		//UpdateWeaponList(); // Refresh the list when opening
		//UpdateEquippedWeaponDisplay(playerInventory.GetEquippedWeapon());
	//}
//
	//public void HideInventory()
	//{
		//Visible = false;
		//Input.MouseMode = Input.MouseModeEnum.Captured;
		//GetTree().Paused = false; // Unpause the game
	//}
//
	//private void UpdateWeaponList()
	//{
		//// Clear existing weapon entries
		//foreach (Node child in weaponListContainer.GetChildren())
		//{
			//child.QueueFree();
		//}
//
		//// Add new entries for each weapon in inventory
		//Array<WeaponData> weapons = playerInventory.GetAllWeapons();
		//foreach (WeaponData weapon in weapons)
		//{
			//WeaponItemUI weaponItem = WeaponItemScene.Instantiate<WeaponItemUI>();
			//weaponListContainer.AddChild(weaponItem);
			//weaponItem.SetWeaponData(weapon);
			//weaponItem.Connect("EquipButtonPressed", Callable.From((WeaponData data) => playerInventory.EquipWeapon(data)));
		//}
	//}
//
	//private void UpdateEquippedWeaponDisplay(WeaponData equippedWeapon)
	//{
		//if (equippedWeapon != null)
		//{
			//equippedWeaponNameLabel.Text = $"Equipped: {equippedWeapon.WeaponName}";
			//equippedWeaponAttackLabel.Text = $"Attack: {equippedWeapon.AttackBonus}";
			//equippedWeaponDefenseLabel.Text = $"Defense: {equippedWeapon.DefenseBonus}";
			//equippedWeaponHealthLabel.Text = $"Health: {equippedWeapon.HealthBonus}";
		//}
		//else
		//{
			//equippedWeaponNameLabel.Text = "Equipped: None";
			//equippedWeaponAttackLabel.Text = "Attack: --";
			//equippedWeaponDefenseLabel.Text = "Defense: --";
			//equippedWeaponHealthLabel.Text = "Health: --";
		//}
	//}
}
