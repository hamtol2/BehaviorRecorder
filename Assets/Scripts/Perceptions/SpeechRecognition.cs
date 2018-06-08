using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechRecognition : MonoBehaviour {

	AndroidJavaClass sttPlugin;
	IEnumerator coroutine;

	public void Enable () {
		#if UNITY_EDITOR
		#elif UNITY_ANDROID
		sttPlugin.CallStatic ("enableSTT");
		#endif
	}

	public void Disable () {
		#if UNITY_EDITOR
		#elif UNITY_ANDROID
		sttPlugin.CallStatic ("disableSTT");
		#endif
	}

	public string GetResult () {
		#if UNITY_EDITOR
		return "";
		#elif UNITY_ANDROID
		return sttPlugin.CallStatic<string> ("getRecognizedWord");
		#else
		return "";
		#endif
	}

	public void OnRecognized (string words) {

	}

	// Use this for initialization
	void Start () {
		#if UNITY_EDITOR
		#elif UNITY_ANDROID
		AndroidJavaClass androidPlugins = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject mainActivity = androidPlugins.GetStatic<AndroidJavaObject>("currentActivity");
		sttPlugin = new AndroidJavaClass ("kr.ac.hansung.eai.STTPlugin");
		sttPlugin.CallStatic ("initSTT", mainActivity);
		sttPlugin.CallStatic ("enableSTT");

		coroutine = Process(0.5f);
		StartCoroutine(coroutine);
		#endif
	}
	
	// Update is called once per frame
	void Update () {
	}

	// every 0.1 seconds perform
	private IEnumerator Process(float waitTime)
	{
		Debug.Log ("Arbitor::Process");
		while (true)
		{
			yield return new WaitForSeconds(waitTime);

			// try {
			// 	var recognizedWord = GetResult();
			// 	if (recognizedWord != null && recognizedWord.Length > 0) {
			// 		Program.Instance.Parse(recognizedWord);
			// 	}
			// } catch (Exception ex) {
			// 	Debug.Log(ex.ToString());
			// }
		}
	}


	private static SpeechRecognition _instance = null;
	public static SpeechRecognition Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(SpeechRecognition)) as SpeechRecognition;
			}
			return _instance;
		}
	}
}
