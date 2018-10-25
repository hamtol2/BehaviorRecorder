using UnityEngine;
using UnityEngine.SceneManagement;

namespace REEL.Recorder
{
    public class QuizStartButton : EyeSelectionBase
    {
        [SerializeField] private string quizSceneName;

        public override void OnGazeComplete()
        {
            base.OnGazeComplete();

            PlayerPrefs.SetInt(SurveyUtil.countKey, GetFileCount());
            Invoke("LoadMainScene", 0.5f);
        }

        void LoadMainScene()
        {
            SceneManager.LoadScene(quizSceneName);
        }

        int GetFileCount()
        {
            if (PlayerPrefs.HasKey(SurveyUtil.countKey)) return PlayerPrefs.GetInt(SurveyUtil.countKey) + 1;
            else return 1;
        }
    }
}