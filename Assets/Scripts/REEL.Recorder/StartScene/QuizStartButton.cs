using UnityEngine;
using UnityEngine.SceneManagement;

namespace REEL.Recorder
{
    public class QuizStartButton : MonoBehaviour
    {
        [SerializeField] private string quizSceneName;

        public void OnGazeComplete()
        {
            SceneManager.LoadScene(quizSceneName);
        }
    }
}