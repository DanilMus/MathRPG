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
      HideButtonsLabels(true,false);
    }

    public void OnPauseButtonPressed()
    {
      var pauseButton = (TextureButton)GetNode("PauseButton");
      var animatedSpriteButton = (AnimatedSprite)pauseButton.GetNode("AnimatedSprite");
      if (GetTree().Paused)
      {
        GetTree().Paused = false;
        animatedSpriteButton.Animation = "continue";
        animatedSpriteButton.Play();
        HideButtonsLabels(true, false);
      }
      else
      {
        GetTree().Paused = true;
        animatedSpriteButton.Animation = "pause";
        animatedSpriteButton.Play();
        HideButtonsLabels(false, false);
      }
    }

    public void DeathMenuAppear()
    {
      HideButtonsLabels(false, true);
    }

    public void HideButtonsLabels(bool toHide, bool isDead)
    {
      var helpButton = (TextureButton)GetNode("HelpButton");
      var colorRect = (ColorRect)GetNode("ColorRect");
      var exitButton = (TextureButton)GetNode("ExitButton");
      var restartButton = (TextureButton)GetNode("RestartButton");
      var pauseButton = (TextureButton)GetNode("PauseButton");
      var deathAnimatedSprite = (AnimatedSprite)GetNode("DeathAnimatedSprite");
      
      if (toHide)
      {
        helpButton.Hide();
        colorRect.Hide();
        exitButton.Hide();
        restartButton.Hide();
        deathAnimatedSprite.Hide();
      }
      else
      {
        helpButton.Show();
        colorRect.Show();
        exitButton.Show();
        if (isDead)
        {
          pauseButton.Hide();
          helpButton.MarginTop = 186;
          helpButton.MarginBottom = 234;
          exitButton.MarginTop = 266;
          exitButton.MarginBottom = 314;
          restartButton.Show();
          deathAnimatedSprite.Show();
          deathAnimatedSprite.Play();
        }
        else
        {
          helpButton.MarginTop = 106;
          helpButton.MarginBottom = 154;
          exitButton.MarginTop = 216;
          exitButton.MarginBottom = 264;
        }
      }
    }

    public void OnRestartButtonPressed()
    {
      GetTree().Paused = false;
      GetTree().ReloadCurrentScene();
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



