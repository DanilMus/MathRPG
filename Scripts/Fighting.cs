using System.Collections.Generic;
using Godot;
using MathRPG.Scripts.Logic.Sets;

public class Fighting : Node2D
{
    private List<Number> _numbers;
    private NumericSet.Values _set;
    private Button _button;

    private Vector2 _startPosition;
    private float _xOffset;
    private float _yOffset;
    private List<NumberNode> _nodes;

    [Signal]
    public delegate void OnAnswer(float attackFactor);

    public override void _Ready()
    {
        InitializeVariables();
        MakeGui();
    }

    private void InitializeVariables()
    {
        (_set, _numbers) = Maker.CreateSetTask(false);

        _button = new Button();
        _nodes = new List<NumberNode>();

        _xOffset = GetViewportRect().Size.x / 10;
        _yOffset = GetViewportRect().Size.y / 25;
        
        _startPosition = new Vector2(_xOffset, (GetViewportRect().Size.y - GetViewportRect().Size.y / 3) - _yOffset * 3); // From here we start placing NumberNode objects
    }

    private void MakeGui()
    {
        LoadData();
        PlaceButton();
    }

    private void LoadData()
    {
        LoadAnswers();
        LoadQuestion();
    }

    // TODO: Change this shit to TextureButton
    private void PlaceButton()
    {
        NumberNode numberNode = GetNode<NumberNode>("NumberNode2");
        
        _button.RectSize = new Vector2(numberNode.RectSize.x, numberNode.RectSize.y / 1.5F);
        _button.Text = "Confirm";
        _button.RectPosition = new Vector2(GetViewportRect().Size.x / 2 - _button.RectSize.x / 2,
            numberNode.RectPosition.y + _yOffset * 3);
        _button.Connect("pressed", this, "_on_button_pressed");
        // _button.Icon = (Texture) ResourceLoader.Load("res://Sprites/Attack/MathRPG_attack_block.png");
        // _button.Flat = true;
        
        AddChild(_button);
    }

    private void _on_button_pressed()
    {
        List<NumberNode> rightAnswer = CollectRightAnswers();
        List<NumberNode> answer = CollectAnswers();

        List<NumberNode> result = new List<NumberNode>();
        foreach (NumberNode node in answer)
        {
            if (rightAnswer.Contains(node)) result.Add(node);
        }

        EmitSignal("OnAnswer", (float) result.Count / rightAnswer.Count); // Returns attack multiplier.
    }

    private List<NumberNode> CollectRightAnswers()
    {
        List<NumberNode> rightAnswers = new List<NumberNode>();

        foreach (NumberNode node in _nodes)
        {
            if (node.Number.GetSet() == _set) rightAnswers.Add(node);
        }

        return rightAnswers;
    }

    private List<NumberNode> CollectAnswers()
    {
        List<NumberNode> answers = new List<NumberNode>();

        foreach (NumberNode node in _nodes)
        {
            if (node.IsTapped) answers.Add(node);
        }

        return answers;
    }

    private void LoadAnswers()
    {
        PackedScene gridObject = (PackedScene) ResourceLoader.Load("res://Scenes/Number.tscn");

        for (int i = 0; i < _numbers.Count; i++)
        {
            Number number = _numbers[i];
            NumberNode obj = gridObject.Instance() as NumberNode;
            
            AddChild(obj);

            obj.Name = $"NumberNode{i}";
            obj.Number = number;
            obj.Value.Text = number.ToString();
            obj.ChangeSize(); // Be careful with this method
            obj.Connect("Tapped", this, "_on_NumberNode_tapped");

            PlaceObject(obj, i);
        }
        
        CollectNodes();
    }
    
    private void CollectNodes()
    {
        for (int i = 0; i < 6; i++)
        {
            NumberNode obj = GetNode<NumberNode>($"NumberNode{i}");
            _nodes.Add(obj);
        }
    }

    private void LoadQuestion()
    {
        PackedScene multiplicity = (PackedScene) ResourceLoader.Load("res://Scenes/Multiplicity.tscn");
        Multiplicity question = multiplicity.Instance() as Multiplicity;

        AddChild(question);

        question.RectPosition = new Vector2(GetViewportRect().Size.x / 2 - question.RectSize.x / 2,
            _startPosition.y - question.RectSize.y);
        
        question.Label.Text = NumericSet.Strings[ (int) _set];
        question.Label.Modulate = Colors.Black;
    }

    private void PlaceObject(NumberNode obj, int number)
    {
        NumberNode previousNode = null;
        
        switch (number)
        {
            /*
             * Objects look like this:
             * 0 3
             * 1 4
             * 2 5
             */
            case 0:
                obj.RectPosition = _startPosition;
                break;
            
            case 1:
            case 2:
                previousNode = GetNode<NumberNode>($"NumberNode{number - 1}");
                obj.RectPosition = new Vector2(_startPosition.x,
                    previousNode.RectPosition.y + previousNode.RectSize.y + _yOffset);
                break;
            
            case 3:
                previousNode = GetNode<NumberNode>($"NumberNode{number - 3}");
                obj.RectPosition = new Vector2(GetViewportRect().Size.x - _startPosition.x - previousNode.RectSize.x,
                                        previousNode.RectPosition.y);
                break;
            
            case 4:
            case 5:    
                previousNode = GetNode<NumberNode>($"NumberNode{number - 1}");
                obj.RectPosition = new Vector2(GetViewportRect().Size.x - _startPosition.x - previousNode.RectSize.x,
                    previousNode.RectPosition.y + previousNode.RectSize.y + _yOffset);
                break;
        }
    }

    public void _on_NumberNode_tapped(NumberNode node)
    {
        if (node.IsTapped)
        {
            node.Value.Modulate = Colors.Black;
            node.IsTapped = false;
        }
        else
        {
            node.Value.Modulate = Colors.Green;
            node.IsTapped = true;
        }
    }
}
