using Godot;
using System;

/// <summary>
/// В данный класс необходимо будет в дальнешем добавить привязку к перменной - сведениями о ХП
/// (пока такой переменной не существует). Вместо этого для демонстрации работы, добавлены поля класса
/// и изменен метод Process(он изменяет хп от 0 до максимума и наоборот)
/// После добавления привязки демонстрацию нужно удалить (это все поля данного класса и функция _Ready)
/// </summary>
public class HealthBar : TextureProgress
{	
  private int HPFake = 2;                     //Фейковое ХП(для демонстрации работы метода UpdateHealthBar)
  private int HPFakeMax = 500;					//Фейковое максимальное ХП(для демонстрации работы метода UpdateHealthBar)
  private bool isHPFakeIncreasing = true;     //Переменная, обеспечивающая переход текущего хп от 0 до максимального


  public override void _Ready()
  {
    //Вот здесь нужно добавить получение сведений о хп персонажа из другого источника
  }
  
  
  /// <summary>
  /// Функция обновляет полоску ХП в соответствии с введёнными в неё значениями
  /// </summary>
  /// <curHP>Текущее хп</curHP>
  /// <maxHP>Максимальное ХП</maxHP>
  public void UpdateHealthBar(int curHP,int maxHP)
  {
    this.Value=(int)((double)curHP/maxHP*100);                 //Обновить рисунок полоски
    ((Label)GetNode("HealthLabel")).Text=$"{curHP}/{maxHP}";   //Обновить текст полоски
  }


  
   public override void _Process(float delta)
   {
    UpdateHealthBar(HPFake,HPFakeMax);     //Обновление полоски ХП в соответствии с фейк-значениями
    
    //Изменение фейк-значений таким образом, чтобы текущее хп изменялось плавно от 0 до максимума и наоборот
         if(isHPFakeIncreasing)
    {
      HPFake+=1;
      if(HPFake>=HPFakeMax)
      {
        isHPFakeIncreasing=false;
      }
    }
    else
    {
      HPFake-=1;
      if(HPFake<=0)
      {
        isHPFakeIncreasing=true;
      }
    }
  }
}
