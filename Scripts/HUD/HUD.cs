using Godot;
using System;
using System.Diagnostics;

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
		var helpButton = (Button)GetNode("HelpButton");
		helpButton.Connect("pressed", this, "OnHelpButtonPressed");
		HideButtonsLabels(true);
	}

	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	//  public override void _Process(float delta)
	//  {
	//      
	//  }
	private void OnPauseButtonPressed()
	{
		var pauseButton = (Button)GetNode("PauseButton");
		var helpButton = (Button)GetNode("HelpButton");
		if (GetTree().Paused)
		{
			GetTree().Paused = false;
			var textureButtonContinue = ResourceLoader.Load<Texture>("res://Sprites/menu/MathRPG_menu_option_1.png");
			pauseButton.Icon = textureButtonContinue;
			HideButtonsLabels(true);
		}
		else
		{
			GetTree().Paused = true;
			var textureButtonHome = ResourceLoader.Load<Texture>("res://Sprites/menu/MathRPG_menu_option_4.png");
			pauseButton.Icon = textureButtonHome;
			HideButtonsLabels(false);
		}
	}

	public void HideButtonsLabels(bool toHide)
	{
		var helpButton = (Button)GetNode("HelpButton");
		var pauseLabel = (Label)GetNode("PauseLabel");
		var colorRect = (ColorRect)GetNode("ColorRect");
		if (toHide)
		{
			helpButton.Hide();
			pauseLabel.Hide();
			colorRect.Hide();

		}
		else
		{
			helpButton.Show();
			pauseLabel.Show();
			colorRect.Show();
		}
	}

	private void OnHelpButtonPressed()
	{
		string path = ProjectSettings.GlobalizePath("res://Help/") + "/test.chm";
		Process.Start(path);
	}
}









