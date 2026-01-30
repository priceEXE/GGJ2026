using MemoFramework.Extension;
using UnityEngine.SceneManagement;

namespace TapTap2025
{
    public class EndState : GameState
    {
        protected override void OnStateEnter()
        {
            base.OnStateEnter();
            SceneManager.LoadScene(SceneConstants.Menu);
            if (MF.Cutscene.IsPlaying)
            {
                MF.Cutscene.FadeCutScene(GlobalContants.CutSceneFadeDuration);
            } 
            MF.Event.Subscribe<OnRequireEnterGame>(RequireEnterGame);
        }

        private void RequireEnterGame(object o, OnRequireEnterGame obj)
        {
            MF.Cutscene.EnterCutScene(GlobalContants.CutSceneEnterDuration, () =>
            {
                GameStateComponent.RequestStateChange(EGameState.Game.ToString());
            });
            
        }

        protected override void OnStateExit()
        {
            base.OnStateExit();
            MF.Event.Unsubscribe<OnRequireEnterGame>(RequireEnterGame);
        }
    }
}