using Godot;
using System;

public class PathCell : Sprite
{
    AnimationPlayer animation;

    public override void _Ready() // Инициализация анимации
    {
        animation = GetNode<AnimationPlayer>("Animation");
        animation.Play("appear");
    }

    public void OnArea2DMouseEntered() // Когда наводишься мышкой
    {
        if (animation.CurrentAnimation == "appear")
            return;
        animation.Play("chosen");
    }

    public void OnArea2DMouseExited() // Когда убираешь мышку
    {
        if (animation.CurrentAnimation == "appear")
            return;
        animation.Play("back_to_normal");
    }
}
