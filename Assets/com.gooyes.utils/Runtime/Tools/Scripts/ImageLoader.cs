using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


namespace Gooyes.Tools
{
    public static class ImageLoader
    {
        public static string url = "https://picsum.photos/200/300";

        public static IEnumerator DownloadRandomImage(Texture2D outTexture)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                Debug.Log(request.error);
            else
            {
                var texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                outTexture = texture;
            }
        }

        public static IEnumerator DownloadRandomImage(Texture2D outTexture, string url)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                Debug.Log(request.error);
            else
            {
                var texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                outTexture = texture;
            }
        }

        public static IEnumerator DownloadRandomImage(Texture2D outTexture, Action callback)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                Debug.Log(request.error);
            else
            {
                var texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                outTexture = texture;
                callback.Invoke();
            }
        }

        public static IEnumerator DownloadRandomImage(Texture2D outTexture, string url, Action callback)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                Debug.Log(request.error);
            else
            {
                var texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                outTexture = texture;
                callback.Invoke();
            }
        }
    }
}
