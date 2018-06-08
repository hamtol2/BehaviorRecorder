using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechRenderrer : MonoBehaviour, Renderrer {

	bool isPlaying = false;
	AndroidJavaClass ttsPlugin;
	List<string> _speeches = new List<string> ();

	public void Init () {
	}

	public void Play (string name) {
		#if UNITY_EDITOR
		#elif UNITY_ANDROID
		ttsPlugin.CallStatic ("speak", name);
		#endif
	}

	public void Stop () {
		#if UNITY_EDITOR
		#elif UNITY_ANDROID
		ttsPlugin.CallStatic ("stopTTS");
		#endif
	}

	public bool IsRunning () {
		#if UNITY_EDITOR
		return false;
		#elif UNITY_ANDROID
		return ttsPlugin.CallStatic<bool> ("isSpeaking");
		#else
		return false;
		#endif
	}

	void Start () {
		#if UNITY_EDITOR
		#elif UNITY_ANDROID
		AndroidJavaClass androidPlugins = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject mainActivity = androidPlugins.GetStatic<AndroidJavaObject>("currentActivity");

		ttsPlugin = new AndroidJavaClass ("kr.ac.hansung.eai.TTSPlugin");
		ttsPlugin.CallStatic ("initTTS", mainActivity);
		#endif
	}

	private static SpeechRenderrer _instance = null;
	public static SpeechRenderrer Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(SpeechRenderrer)) as SpeechRenderrer;
			}
			return _instance;
		}
	}
}
