using UnityEngine.Networking;
using System.Text;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    public class NetworkRequest
    {
        public long netPacketIndex;
        public Enumerators.GoogleNetworkType recognitionType;

        public UnityWebRequest request;

        public NetworkRequest(string uri, string data, long index, Enumerators.GoogleNetworkType type)
        {
            recognitionType = type;
            netPacketIndex = index;

            if (type == Enumerators.GoogleNetworkType.GET_OPERATION)
                request = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbGET);
            else
                request = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbPOST);

            if (!string.IsNullOrEmpty(data))
                request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(data));

            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            var config = GCSpeechRecognition.Instance.ServiceLocator.Get<ISpeechRecognitionManager>().CurrentConfig;

            if (config.enabledAndroidCertificateCheck)
            {
                request.SetRequestHeader("X-Android-Package", config.packageName);
                request.SetRequestHeader("X-Android-Cert", config.keySignature);
            }
        }

        public void Send()
        {
            request.Send();
        }
    }
}