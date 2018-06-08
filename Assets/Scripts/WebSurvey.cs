using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using UnityEngine.UI;

public class WebSurvey : MonoBehaviour {

	public REEL.Animation.RobotFacialRenderer robotFacialRenderer;

	private MqttClient mqttClient;
	string requested = "";
	string received_tts = "";
	string received_facial = "";
	string received_motion = "";

	public Text TTS;

	private void Awake()
	{
		if (!PlayerPrefs.HasKey("UUID"))
		{
			PlayerPrefs.SetString("UUID", System.Guid.NewGuid().ToString());
		}
	}

	// Use this for initialization
	void Start () {
		mqttClient = new MqttClient("localhost", 1883, false , null); 

		mqttClient.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

        string clientId = Guid.NewGuid().ToString();
        mqttClient.Connect(clientId);
        Debug.Log("MQTT IsConnected: " + mqttClient.IsConnected);
		mqttClient.Subscribe(
            new string[] { "/reply" },
            new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
		mqttClient.Subscribe(
            new string[] { "/tts" },
            new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
		mqttClient.Subscribe(
            new string[] { "/motion" },
            new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
		mqttClient.Subscribe(
            new string[] { "/facial" },
            new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
	}
	
	// Update is called once per frame
	void Update () {
		if (received_tts.Length > 0)
        {
            TTS.text = received_tts;
            received_tts = "";
        }
		if (received_facial.Length > 0) {
			robotFacialRenderer.Play(received_facial);
			received_facial = "";
		}
		if (received_motion.Length > 0) {
			REEL.PoseAnimation.RobotTransformController.Instance.PlayGesture(received_motion);
			received_motion = "";
		}
	}

	void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e) 
    {
		String topic = e.Topic;
        String message = System.Text.Encoding.UTF8.GetString(e.Message);
		Debug.Log("Topic: " + topic + ", Message: " + message);
		switch (topic) {
			case "/tts":
				received_tts = message;
				break;
			case "/facial":
				received_facial = message;
				break;
			case "/motion":
				received_motion = message;
				break;
		}
    } 

	private static WebSurvey _instance = null;
	public static WebSurvey Instance
    {
        get
        {
            if (_instance == null)
            {
				_instance = FindObjectOfType(typeof(WebSurvey)) as WebSurvey;
            }
            return _instance;
        }
    }

}
