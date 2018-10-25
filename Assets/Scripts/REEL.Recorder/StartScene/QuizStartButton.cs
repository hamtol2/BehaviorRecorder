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

            PlayerPrefs.SetInt(SurveyStart.countKey, GetFileCount());
            Invoke("LoadMainScene", 0.5f);
        }

        void LoadMainScene()
        {
            SceneManager.LoadScene(quizSceneName);
        }

        int GetFileCount()
        {
            if (PlayerPrefs.HasKey(SurveyStart.countKey)) return PlayerPrefs.GetInt(SurveyStart.countKey) + 1;
            else return 1;
        }
    }
}