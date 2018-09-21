using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.Recorder
{
	public class QuizManager : Singleton<QuizManager>
	{
        public string quizTitle;
        public int quizNumber;
        public ContentState quizState = ContentState.IceBreaking;
        public AnswerState answerState = AnswerState.Wait;
        public ModelType robotModelType = ModelType.ExpressionRobot;
    }
}