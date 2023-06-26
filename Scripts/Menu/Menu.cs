using Godot;
using System;
using System.Diagnostics;

namespace MathRPG.MenuHUD
{
	public class Menu : CanvasLayer
	{

		public override void _Ready()
		{
			var pauseButton = (TextureButton)GetNode("PauseButton");
			var animatedSpriteButton = (AnimatedSprite)pauseButton.GetNode("AnimatedSprite");
			var helpButton = (TextureButton)GetNode("HelpButton");
			HideButtonsLabels(true);
		}

		private void OnPauseButtonPressed()
		{
			var pauseButton = (TextureButton)GetNode("PauseButton");
			var animatedSpriteButton = (AnimatedSprite)pauseButton.GetNode("AnimatedSprite");
			if (GetTree().Paused)
			{
				GetTree().Paused = false;
				animatedSpriteButton.Animation = "continue";
				animatedSpriteButton.Play();
				HideButtonsLabels(true);
			}
			else
			{
				GetTree().Paused = true;
				animatedSpriteButton.Animation = "pause";
				animatedSpriteButton.Play();
				HideButtonsLabels(false);
			}
		}

		private void HideButtonsLabels(bool toHide)
		{
			var helpButton = (TextureButton)GetNode("HelpButton");
			var pauseLabel = (Label)GetNode("PauseLabel");
			var colorRect = (ColorRect)GetNode("ColorRect");
			var exitButton = (TextureButton)GetNode("ExitButton");
			if (toHide)
			{
				helpButton.Hide();
				pauseLabel.Hide();
				colorRect.Hide();
				exitButton.Hide();
			}
			else
			{
				helpButton.Show();
				pauseLabel.Show();
				colorRect.Show();
				exitButton.Show();
			}
		}

		private void OnHelpButtonPressed()
		{
			string executableDirectory = AppDomain.CurrentDomain.BaseDirectory;
			string path = executableDirectory + "/test.chm";
			Process.Start(path);
		}

		private void OnExitButtonPressed()
		{
			GetTree().Quit();
		}
	}
}