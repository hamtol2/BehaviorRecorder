using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BluetoothManager : MonoBehaviour {

	AndroidJavaClass btPlugin;
	List<string> _bluetoothDeviceNameList = new List<string>();
	List<byte> _recvBuffer = new List<byte>();
	bool _isBluetoothConnected;
    int _receivedData = -1;
    bool _isReceivedMessage;

	string recvMessage;

	public void OnSearchBTDevices () {
		#if UNITY_EDITOR
		#elif UNITY_ANDROID
		SearchBluetoothDevice ();
		#endif		
	}

	public bool ConnectBT(String device) {
		try {
			if (btPlugin.CallStatic<bool> ("connect", device)) {
				Debug.Log ("Successfully connected to " + device);
				return true;
			}
			else {
				Debug.Log ("Fail connected to " + device);
				return false;
			}
		} catch (Exception ex) {
			Debug.Log(ex.ToString());
			return false;
		}
	}

	public void Send (string message) {
		try {
			btPlugin.CallStatic ("send", message + System.Environment.NewLine);
		} catch (Exception ex) {
			Debug.Log(ex.ToString());
		}
	}

	void SearchBluetoothDevice()
    {
        Debug.Log("SearchBluetoothDevice");
        _bluetoothDeviceNameList.Clear();
		try {
			btPlugin.CallStatic ("SearchBluetoothDevice");
		} catch (Exception ex) {
			Debug.Log(ex.ToString());
		}
    }

	void BluetoothDevice(string deviceName) {
		Debug.Log("[BluetoothManager::BluetoothDevice] " + deviceName);
        _bluetoothDeviceNameList.Add(deviceName);
    }

    void BluetoothConnectState(string signal) {
		Debug.Log("[BluetoothManager::BluetoothConnectState] " + signal);
		if (signal == "BT_STATE_CONNECTED")
        {
			Debug.Log("Bluetooth device connected");
            _isBluetoothConnected = true;
        }
		else if (signal == "BT_STATE_DISCONNECTED")
        {
			Debug.Log("Bluetooth device disconnected");
            _isBluetoothConnected = false;
        }
    }

    void BluetoothData(string readData) {
		// byte[] recvBuffer = HexStringToByteArray(readData);
		// for (int i = 0 ; i < readData.Length/2 ; i++) {
		// 	_recvBuffer.Add(recvBuffer[i]);
		// }
		Debug.Log("[BluetoothManager::BluetoothData] " + readData);
		Debug.Log("[recvMessage] " + recvMessage);
		recvMessage += readData;
		var index_newline = recvMessage.IndexOf("\r\n");
		if (index_newline > 0 )
		{
			Debug.Log("Found new line");
			var command = recvMessage.Substring(0, index_newline);
			recvMessage = recvMessage.Substring(index_newline+2);

			Program.Instance.Parse(command);
		}
    }

	// Use this for initialization
	void Start () {
		#if UNITY_EDITOR
		#elif UNITY_ANDROID
		AndroidJavaClass androidPlugins = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject mainActivity = androidPlugins.GetStatic<AndroidJavaObject>("currentActivity");

		btPlugin = new AndroidJavaClass ("kr.ac.hansung.eai.BTPlugin");
		btPlugin.CallStatic ("initBT", mainActivity);
		#endif
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	static byte[] HexStringToByteArray(string hexString)
	{
		if (hexString == null)
			throw new ArgumentNullException("hexString");
		
		if ((hexString.Length & 1) != 0)
			throw new ArgumentOutOfRangeException("hexString", hexString, "hexString must contain an even number of characters.");
		
		byte[] result = new byte[hexString.Length/2];
		for (int i = 0; i < hexString.Length; i += 2)
			result[i/2] = byte.Parse(hexString.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);
		
		return result;
	}

	private static BluetoothManager _instance = null;
	public static BluetoothManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(BluetoothManager)) as BluetoothManager;
			}
			return _instance;
		}
	}

}
