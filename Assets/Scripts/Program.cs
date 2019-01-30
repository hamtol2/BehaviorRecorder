using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

public class SimsimiResult
{
    public string id;
    public int result;
    public string msg;
    public string response;
}

public class Program : MonoBehaviour
{
//    const string FILENAME = "/begin.txt";

//    // Script Editing
//    public InputField mIfScript;

//    public Text text;

//    // Dialog
//    public InputField mIfDialog;
//    public InputField mIfSay;

//    // Setting Panel
//    public InputField btAddress;
//    public InputField speechRecognition;
//    public InputField mRecognizedWord;
//    public InputField mSimsimiReply;

//    const string SIMSIMI_URI = "http://sandbox.api.simsimi.com/request.p?key=418cf7fc-ea4f-4ed4-88d7-f4a900f13cba&lc=ko&ft=1.0&text=";
//    // static HttpClient _simsimi = new HttpClient();

//    private MqttClient mqttClient;

//    //	PreProcess proc = new PreProcess ();


//    RiveScript.RiveScript _rs = new RiveScript.RiveScript(utf8: true, debug: true);

//    public void OnStartSpeechRecognition()
//    {
//        Debug.Log("OnStartSpeechRecognition");
//        SpeechRecognition.Instance.Enable();
//    }

//    public void OnLoadScript()
//    {
//        try
//        {
//            mIfScript.text = System.IO.File.ReadAllText(Application.dataPath + FILENAME);
//        }
//        catch (Exception ex)
//        {
//            Debug.Log(ex.ToString());
//        }
//    }

//    public void OnSaveScript()
//    {
//        System.IO.File.WriteAllText(Application.dataPath + FILENAME, mIfScript.text);
//    }

//    public void OnSay()
//    {
//        mIfDialog.text += "[Human] " + mIfSay.text + "\n";
//        var reply = _rs.reply("default", mIfSay.text);
//        mIfDialog.text += "[Robot] " + reply + "\n";
//        Debug.Log("reply: " + reply);
//        if (reply.Contains("NOT_MATCHED"))
//        {
//            Debug.Log("Not matched");
//        }
//        else
//        {
//            Arbitor.Instance.Insert(reply);
//        }
//    }

//    public void OnSleepy()
//    {
//        var reply = _rs.reply("default", "졸려");
//        Arbitor.Instance.Insert(reply);
//    }

//    public void OnRecognition()
//    {
//        Parse(speechRecognition.text);
//    }

//    public void OnConnectBT()
//    {
//        var btAddr = btAddress.text;
//        if (BluetoothManager.Instance.ConnectBT(btAddr))
//        {
//            PlayerPrefs.SetString("BTDeviceAddress", btAddr);
//        }
//    }

//    public void AskSimsimi()
//    {
//        StartCoroutine(GetSimsimi(mRecognizedWord.text));
//    }

//    public void SimsimiSpeak()
//    {
//        if (mSimsimiReply.text.Length > 0)
//        {
//            Arbitor.Instance.Insert(mSimsimiReply.text);
//        }
//    }



//    // Use this for initialization
//    void Start()
//    {

//        btAddress.text = PlayerPrefs.GetString("BTDeviceAddress", "DB:E7:DF:00:C0:0E");

//#if UNITY_EDITOR || UNITY_WEBGL || UNITY_STANDALONE
//        Debug.Log("UNITY_EDITOR");
//        if (_rs.loadFile(Application.dataPath + FILENAME))
//#elif UNITY_ANDROID
//		Debug.Log("UNITY_ANDROID");
//		if (_rs.loadFile (Application.persistentDataPath + FILENAME))
//#elif UNITY_IOS
//		Debug.Log("UNITY_IOS");
//		if (_rs.loadFile (Application.persistentDataPath + FILENAME))
//#endif
//        {
//            _rs.sortReplies();
//            Debug.Log("Successfully load file");

//            // try {
//            // 	var r1 = _rs.reply("default", "안녕");
//            // 	text.text = r1;
//            // }
//            // catch (System.Exception ex) {
//            // 	Debug.Log (string.Format ("{0}", ex));
//            // 	text.text = string.Format ("{0}", ex);
//            // }
//        }
//        else
//        {
//            Debug.Log("Fail to load " + Application.persistentDataPath + FILENAME + " file");
//        }

//        mqttClient = new MqttClient("iot.onairsoft.com", 1883, false, null);

//        // register to message received 
//        //		mqttClient.MqttMsgSubscribed += Client_MqttMsgSubscribed;
//        mqttClient.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

//        string clientId = Guid.NewGuid().ToString();
//        mqttClient.Connect(clientId, "powerst", "actech88");
//        Debug.Log("MQTT IsConnected: " + mqttClient.IsConnected);

//        // subscribe to the topic "/home/temperature" with QoS 2 
//        // mqttClient.Subscribe (new string[] { "mindslab" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

//        ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) =>
//        {
//            return true;
//        };

//        Parse("안녕");
//    }

//    void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
//    {
//        String message = System.Text.Encoding.UTF8.GetString(e.Message);
//        Debug.Log("Received: " + message);
//        Arbitor.Instance.Insert(message);
//    }

//    List<string> greet = new List<string> { "안녕하세요", "만나서반가워요", "좋은 하루군요" };
//    private IEnumerator Process(string input)
//    {
//        //		string url = "http://localhost:5009/intent/" + input;
//        string url = "http://iot.onairsoft.com:5009/intent/" + input;

//        WWW www = new WWW(url);

//        yield return www;
//        // check for errors
//        if (www.error == null)
//        {
//            mRecognizedWord.text = input;

//            Debug.Log("WWW Ok!: " + www.text);
//            EaiIntent intent = JsonUtility.FromJson<EaiIntent>(www.text);
//            Debug.Log("result: " + intent.result);
//            Debug.Log("intent: " + intent.intent);
//            if (intent.intent.Contains("인사"))
//            {
//                string reply = greet[UnityEngine.Random.Range(0, greet.Count)];
//                Arbitor.Instance.Insert(reply);
//                Debug.Log(reply);
//            }
//            else
//            {
//                var reply = _rs.reply("default", input);
//                Debug.Log("reply: " + reply);
//                if (reply.Contains("NOT_MATCHED"))
//                {
//                    Debug.Log("Not matched. Request to API.AI");
//                }
//                else
//                {
//                    Arbitor.Instance.Insert(reply);
//                }
//            }
//        }
//        else
//        {
//            Debug.Log("WWW Error: " + www.error);
//            var reply = _rs.reply("default", input);
//            Debug.Log("reply: " + reply);
//            if (reply.Contains("NOT_MATCHED"))
//            {
//                Debug.Log("Not matched. Request to API.AI");
//            }
//            else
//            {
//                Arbitor.Instance.Insert(reply);
//            }
//        }
//    }

//    public void Parse(string input)
//    {
//        Debug.Log("Parse: " + input);

//        StartCoroutine(Process(input));


//        //		input = PreProcess.Instance.Run (input);
//        //
//        //		if (input.Length > 0) {
//        //			mRecognizedWord.text = input;
//        //			
//        //			var reply = _rs.reply ("default", input);
//        //			Debug.Log("reply: " + reply);
//        //			if (reply.Contains("NOT_MATCHED")) {
//        //				Debug.Log("Not matched. Request to API.AI");
//        //				Arbitor.Instance.Insert (GetApiAi (input));
//        //			}
//        //			else {
//        //				Arbitor.Instance.Insert(reply);
//        //			}
//        //		}
//    }

//    IEnumerator GetSimsimi(string input)
//    {
//        Debug.Log("Request: " + SIMSIMI_URI + input);
//        UnityWebRequest request = UnityWebRequest.Get(SIMSIMI_URI + input);
//        yield return request.Send();
//        Debug.Log(request.downloadHandler.text);

//        SimsimiResult simsimiResult = JsonUtility.FromJson<SimsimiResult>(request.downloadHandler.text);
//        if (simsimiResult.result == 100)
//        {
//            mSimsimiReply.text = simsimiResult.response;
//        }
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        var recognizedWord = SpeechRecognition.Instance.GetResult();
//        if (recognizedWord != null & recognizedWord.Length > 0)
//            Parse(recognizedWord);
//    }

//    private static Program _instance = null;
//    public static Program Instance
//    {
//        get
//        {
//            if (_instance == null)
//            {
//                _instance = FindObjectOfType(typeof(Program)) as Program;
//            }
//            return _instance;
//        }
//    }
}