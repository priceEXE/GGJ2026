using MemoFramework.Extension;
using MemoFramework.GameState;

namespace TapTap2025
{
    public class SplashState : GameStateBase
    {
        protected override void OnStateEnter()
        {
            base.OnStateEnter();
            MF.Cutscene.EnterCutScene(GlobalContants.CutSceneEnterDuration, () =>
            {
                GameStateComponent.RequestStateChange(EGameState.Menu.ToString());
            });
        }

        protected override void OnStateExit()
        {
            base.OnStateExit();
            MF.Cutscene.FadeCutScene(GlobalContants.CutSceneFadeDuration);
        }
    }
}