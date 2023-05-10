using Godot;
using MathRPG.Scripts.Logic.Sets;

public class NumberNode : Control
{
    private Sprite _attackBlock;
    public Number Number;
    public Label Value;
    public bool IsTapped;
    
    [Signal]
    public delegate void Tapped(NumberNode node);
    
    public override void _Ready()
    {
        InitializeVariables();
        ConnectSignals();
    }

    private void InitializeVariables()
    {
        _attackBlock = GetNode<Sprite>("AttackBlock");
        Value = GetNode<Label>("AttackBlock/Value");
        IsTapped = false;
    }

    private void ConnectSignals()
    {
        Connect("gui_input", this, "_on_gui_input");
    }

    private void _on_gui_input(InputEvent @event)
    {
        if ( !(@event is InputEventMouseButton && @event.IsPressed()) ) return;

        EmitSignal(nameof(Tapped), this);
    }

    public void ChangeSize()
    {
        RectSize = _attackBlock.Texture.GetSize();
        Value.RectSize = RectSize;
    }
}
