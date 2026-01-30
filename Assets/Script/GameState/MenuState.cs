using MemoFramework.Extension;
using MemoFramework.GameState;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

namespace GGJ2026
{
    public class MenuState : GameStateBase
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
            MF.Event.Subscribe<OnRequireEnterTest>(RequireEnterTest);
        }

        protected override void OnStateExit()
        {
            base.OnStateExit();
            MF.Event.Unsubscribe<OnRequireEnterGame>(RequireEnterGame);
            MF.Event.Unsubscribe<OnRequireEnterTest>(RequireEnterTest);
        }

        private void RequireEnterGame(object sender, OnRequireEnterGame e)
        {
            MF.Cutscene.EnterCutScene(GlobalContants.CutSceneEnterDuration, () =>
            {
                GameStateComponent.RequestStateChange(EGameState.Game.ToString());
            });
        }

        private void RequireEnterTest(object sender, OnRequireEnterTest e)
        {
            GameStateComponent.RequestStateChange(EGameState.Test.ToString());
        }
        
        
    }
}