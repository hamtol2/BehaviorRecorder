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
            SceneManager.LoadScene(quizSceneName);
        }
    }
}