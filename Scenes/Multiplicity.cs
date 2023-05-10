using Godot;
using System;

public class Multiplicity : Control
{
    private Sprite _sprite;
    public Label Label;
    
    public override void _Ready()
    {
        InitializeVariables();
        ChangeSize();
    }
    
    private void InitializeVariables()
    {
        _sprite = GetNode<Sprite>("Sprite");
        Label = GetNode<Label>("Label");
    }
    
    private void ChangeSize()
    {
        RectSize = _sprite.Texture.GetSize();
        Label.RectSize = RectSize;
    }
}
