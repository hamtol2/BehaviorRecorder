using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;


public class Arbitor : MonoBehaviour {

	public Toggle automaticExpression;

	List<String> _items = new List<String> ();
	bool _isSpeaking;

	private IEnumerator coroutine;

	public void Insert (String item) {
		_items.Add(item);
	}

	void Start () {
		SpeechRenderrer.Instance.Init();

		coroutine = Process(0.1f);
		StartCoroutine(coroutine);
	}


	// every 0.1 seconds perform
	private IEnumerator Process(float waitTime)
	{
		Debug.Log ("Arbitor::Process");
		while (true)
		{
			yield return new WaitForSeconds(waitTime);

			if (_items.Count > 0) {
				String reply = _items[0];
				Debug.Log ("Arbitor::Input " + reply);

				Regex rx = new Regex("(<[^>]+>)");
				MatchCollection matches = rx.Matches(reply);
				// Debug.Log("Match found " + matches.Count);
				if (matches.Count > 0) {
					foreach (Match match in matches)
					{
						GroupCollection groupCollection = match.Groups;
						String command = groupCollection[1].ToString();
						reply = reply.Replace(command, "");
						// Debug.Log("command: " + command);
						command = command.Substring(1).Substring(0,command.Length-2);
						// Debug.Log("command: " + command);

						int index = command.IndexOf("=");
						if (index > 0) {
							String tag = command.Substring(0, index);
							command = command.Substring(index+1);

							switch (tag) {
								case "sm":
									String[] detail = Regex.Split(command, ":");
									if (detail.Length > 0) {
										switch (detail[0]) {
											case "facial":
												Debug.Log("Sub command facial with " + detail[1]);
												REEL.Animation.FacialRenderrer.Instance.Play(detail[1]);
												break;
											case "motion":
												Debug.Log("Sub command motion with " + detail[1]);
												BluetoothManager.Instance.Send(detail[1]);
												break;
											case "mobility":
												Debug.Log("Sub command mobility with " + detail[1]);
												BluetoothManager.Instance.Send(detail[1]);
												break;
											default:
												break;
										}
									}
									break;
								default:
									Debug.Log("No matched command with " + tag);
									break;
							}
						}
					}
				}
				else if (automaticExpression.isOn) {
					switch (UnityEngine.Random.Range(0, 5)) {
						case 0:
							REEL.Animation.FacialRenderrer.Instance.Play("normal");
							BluetoothManager.Instance.Send("normal");
							break;
						case 1:
							REEL.Animation.FacialRenderrer.Instance.Play("normal");
							BluetoothManager.Instance.Send("hi");
							break;
						case 2:
							REEL.Animation.FacialRenderrer.Instance.Play("normal");
							BluetoothManager.Instance.Send("hello");
							break;
						case 3:
							REEL.Animation.FacialRenderrer.Instance.Play("surprised");
							BluetoothManager.Instance.Send("hi");
							break;
						case 4:
							REEL.Animation.FacialRenderrer.Instance.Play("surprised");
							BluetoothManager.Instance.Send("angry");
							break;
					}
				}

				if (!SpeechRenderrer.Instance.IsRunning()) {
					SpeechRecognition.Instance.Disable();
					SpeechRenderrer.Instance.Play(reply);
					_items.RemoveAt(0);
				}
			}

			if (_isSpeaking && !SpeechRenderrer.Instance.IsRunning()) {
				Debug.Log ("Speaking finished");
				SpeechRecognition.Instance.Enable();
			}
			_isSpeaking = SpeechRenderrer.Instance.IsRunning();

		}
	}

	private static Arbitor _instance = null;
	public static Arbitor Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(Arbitor)) as Arbitor;
			}
			return _instance;
		}
	}
}
