using MemoFramework;
using MemoFramework.Extension;
using MemoFramework.GameState;

namespace GGJ2026
{
    public class TapTapLauncher : MFLauncher
    {
        public override void InitGameStatesFsm(GameStateComponent gameStateComponent)
        {
            gameStateComponent.PushGameState(EGameState.Splash.ToString(), new SplashState());
            gameStateComponent.PushGameState(EGameState.Menu.ToString(), new MenuState());
            gameStateComponent.PushGameState(EGameState.Game.ToString(), new GameState());
            gameStateComponent.PushGameState(EGameState.Test.ToString(), new GameTestState());
            gameStateComponent.PushGameState(EGameState.End.ToString(), new EndState());
            
            gameStateComponent.SetAsStartState(EGameState.Splash.ToString());
        }
    }
}