using Godot;
using System;

public class HUD : CanvasLayer
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var textureButtonHome = ResourceLoader.Load<Texture>("res://Sprites/menu/MathRPG_menu_option_4.png");
		var pauseButton = (Button)GetNode("PauseButton");
		pauseButton.Icon = textureButtonHome;
	}

	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	//  public override void _Process(float delta)
	//  {
	//      
	//  }
	private void OnPauseButtonPressed()
	{
		var pauseButton = (Button)GetNode("PauseButton");
		if (GetTree().Paused)
		{
			GetTree().Paused = false;
			var textureButtonContinue = ResourceLoader.Load<Texture>("res://Sprites/menu/MathRPG_menu_option_1.png");
			pauseButton.Icon = textureButtonContinue;
		}
		else
		{
			GetTree().Paused = true;
			var textureButtonHome = ResourceLoader.Load<Texture>("res://Sprites/menu/MathRPG_menu_option_4.png");
			pauseButton.Icon = textureButtonHome;
		}
	}
}



