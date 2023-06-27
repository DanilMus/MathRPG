using Godot;

namespace MathRPG.Attack
{
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
        
            // It does not appear in the center of it's parent without these settings. It is either because the font is shit or font size is really small
            Label.MarginLeft = 1f;
            Label.MarginTop = 1f;
        }
    }
}
