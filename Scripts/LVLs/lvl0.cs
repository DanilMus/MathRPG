using Godot;
using MathRPG.Entities.Heroes;

namespace MathRPG.LVL
{
    public class lvl0 : Main
    {
        FirstFriend friend; // для друга
        int cutsceneNum = 0; // номер текущей катсцены
        string[] cutscenesNames; // хранение названий катсцен
        

        public override void _Ready() // Первая инициализация сцены
        {
            base.InitializeVariables();
            cutscenesNames = cutscenes.GetAnimationList();

            friend = entities.GetNode<FirstFriend>("FirstFriend");
            friend.Position = pathFinder.GetClosestPosition(friend.Position); // Прикрепление позиции игрока к сетке
            
            // friend = GetNode<AnimatedSprite>("FirstFriend");
            // friend.Position = pathFinder.GetClosestPosition(friend.Position);
            // friend.Connect("MousePressed", this, nameof(PlayScene));
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);
        }

        public void PlayScene()
        {
            if (cutsceneNum < cutscenesNames.Length && !cutscenes.IsPlaying())
                cutscenes.Play(cutscenesNames[ cutsceneNum++ ]);
        }
    }
}