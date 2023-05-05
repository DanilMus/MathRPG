using Godot;
using System;



/// <summary>
/// Данный класс представляет собой малое зелье. При нажатии на его кнопку, излучается сигнал pressed, который вызывает
/// функции _on_SmallPotionButton_pressed() в узлах Main(назначение - отменить изменение пути, так как мы нажали не на поле, а на кнопку UI)
/// и SmallPotion(назначение - изменить количество зелий), а также сигнал pressedWithHealCount c параметром
/// целого типа, который вызывает функцию _on_SmallPotionButton_pressed(int heal) 
/// в узле Player(назначение - изменить параметры хп персонажа, обновить полоску хп). 
/// </summary>
public class SmallPotion : Button
{
    int potionsCount=5;
    int healFromPotion=5;
    [Signal]
    delegate void pressedWithHealCount(int heal);
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ((Label)GetNode("Label")).Text=potionsCount.ToString(); //Получаем актуальную информацию о количестве зелий
        //Подключаем сигналы, чтобы при нажатии на кнопку вызывались функции _on_SmallPotionButton_pressed в узлах Player,Main,SmallPotion
        this.Connect("pressed", this, "_on_SmallPotionButton_pressed");
        this.Connect("pressed", GetParent().GetParent().GetParent(), "_on_SmallPotionButton_pressed");
        this.Connect("pressedWithHealCount",GetParent().GetParent(),"_on_SmallPotionButton_pressed");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }


    public void _on_SmallPotionButton_pressed()
    {
       if(potionsCount>0)
       {
          potionsCount-=1;
          ((Label)GetNode("Label")).Text=potionsCount.ToString();
          EmitSignal("pressedWithHealCount",healFromPotion); //Излучаем сигнал для выполнения _on_SmallPotionButton_pressed в узле Player
          if(potionsCount==0)  // Делаем картинку зелья прозрачной, если больше нет зелий
          {
             ((TextureRect)GetNode("TextureRect")).Modulate=new Color(1,1,1,(float)0.5);
          }
       }
    }

}


