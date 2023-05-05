using Godot;
using System;

/// <summary>
/// Данный класс представляет собой полоску здоровья персонажа. Её можно обновить при помощи функции UpdateHealthBar.
/// </summary>
public class HealthBar : TextureProgress
{
  
    

    public override void _Ready()
    {

    }


    /// <summary>
    /// Функция обновляет полоску ХП в соответствии с введёнными в неё значениями
    /// </summary>
    /// <curHP>Текущее хп</curHP>
    /// <maxHP>Максимальное ХП</maxHP>
    public void UpdateHealthBar(int curHP, int maxHP)
    {
        this.Value = (int)((double)curHP / maxHP * 100);                 //Обновить рисунок полоски
        ((Label)GetNode("HealthLabel")).Text = $"{curHP}/{maxHP}";   //Обновить текст полоски
    }



    public override void _Process(float delta)
    {
       
    }
}
