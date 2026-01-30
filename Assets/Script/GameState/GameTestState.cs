using MemoFramework.Extension;
using UnityEngine.SceneManagement;

namespace GGJ2026
{
    public class GameTestState : GameState
    {
        protected override void OnStateEnter()
        {
            base.OnStateEnter();
            InitScene();
        }
        private void InitScene()
        {
            SceneManager.LoadScene(SceneConstants.TestScene);
            if (MF.Cutscene.IsPlaying)
            {
                MF.Cutscene.FadeCutScene(GlobalContants.CutSceneFadeDuration);
            }
        }

        protected override void OnStateExit()
        {
            base.OnStateExit();
        }
    }
}