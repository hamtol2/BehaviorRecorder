using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.Recorder
{
    [CreateAssetMenu(menuName = "ScriptableObject/Quiz/Quiz Config")]
	public class QuizConfig : ScriptableObject
	{
        public string quizTitle;
        public ContentState quizState = ContentState.IceBreaking;
        public AnswerState answerState = AnswerState.Wait;
        public ModelType robotModelType = ModelType.ExpressionRobot;
    }
}