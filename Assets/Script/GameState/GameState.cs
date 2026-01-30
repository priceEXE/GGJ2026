using MemoFramework.Extension;
using MemoFramework.GameState;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace GGJ2026
{
    public class GameState : GameStateBase
    {
        protected override void OnStateEnter()
        {
            base.OnStateEnter();
            RegisterEvents();
            InitScene();
        }

        protected override void OnStateExit()
        {
            base.OnStateExit();
            UnregisterEvents();
        }

        private void InitScene()
        {
            SceneManager.LoadScene(SceneConstants.Game);
            if (MF.Cutscene.IsPlaying)
            {
                MF.Cutscene.FadeCutScene(GlobalContants.CutSceneFadeDuration);
            }
            
        }

        private void RegisterEvents()
        {
            MF.Event.Subscribe<OnRequireEnterMenu>(RequireEnterMenu);
            MF.Event.Subscribe<OnRequireGameOver>(RequireGameOver);
            MF.Event.Subscribe<OnRequireEnterEnd>(RequireEnterEnd);
        }

        private void UnregisterEvents()
        {
            MF.Event.Unsubscribe<OnRequireEnterMenu>(RequireEnterMenu);
            MF.Event.Unsubscribe<OnRequireGameOver>(RequireGameOver);
            MF.Event.Unsubscribe<OnRequireEnterEnd>(RequireEnterEnd);
        }

        private void RequireEnterMenu(object sender, OnRequireEnterMenu e)
        {
            MF.Cutscene.EnterCutScene(GlobalContants.CutSceneEnterDuration, () =>
            {
                GameStateComponent.RequestStateChange(EGameState.Menu.ToString());
            });
        }

        private void RequireGameOver(object sender, OnRequireGameOver e)
        {
            // Player Die Animation
            MF.Cutscene.EnterCutScene(GlobalContants.CutSceneEnterDuration, () =>
            {
                GameStateComponent.RequestStateChange(EGameState.Menu.ToString());
            });
        }

        private void RequireEnterEnd(object sender, OnRequireEnterEnd e)
        {
            MF.Cutscene.EnterCutScene(GlobalContants.CutSceneEnterDuration, () =>
            {
                GameStateComponent.RequestStateChange(EGameState.End.ToString());
            });
        }
    }
}