using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class XVerseWeb : MonoBehaviour
{
    public enum XVerseRequestTypes{
        GET, POST, DOWNLOAD
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="URL"></param>
    /// <param name="type"> use XVerseRequestTypes</param>
    /// <param name="successHandler"> callback on success </param>
    /// <param name="failHandler"> default null, callback on failure</param>
    /// <param name="formData">for POST: key1, value1, key2, value2, .... must be strings.</param>
    /// <returns></returns>
    public IEnumerator WebRequest(string URL, XVerseRequestTypes type, 
        Action<ResponseData> successHandler, Action<FailResponseData> failHandler = null, params string[] formData)
    {
        UnityWebRequest request = null;
        switch (type)
        {
            case XVerseRequestTypes.GET:
                request = UnityWebRequest.Get(URL);
                break;
            case XVerseRequestTypes.POST:
                WWWForm form = ParseFormParameters(formData);
                request = UnityWebRequest.Post(URL, form);
                break;

        }
        // send and wait
        //request.SetRequestHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
        //request.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/103.0.0.0 Safari/537.36");
        //request.SetRequestHeader("Accept-Encoding", "gzip, deflate, br");
        //request.SetRequestHeader("Connection", "keep-alive");
        yield return request.SendWebRequest();

        // process result
        if (request.result == UnityWebRequest.Result.Success)
        {
            // successful
            ResponseData data = new ResponseData();
            data.Data = request.downloadHandler.data;
            data.Length = request.downloadedBytes;
            data.ResponseCode = request.responseCode;
            successHandler(data);
        }
        else if (request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError ||
            request.result == UnityWebRequest.Result.DataProcessingError)
        {
            // failed
            FailResponseData data = new FailResponseData();
            data.ResponseCode = request.responseCode;
            data.Message = request.error;
            failHandler(data);
        }
        else throw new ArgumentException("encountered unexpected error");
    }


    protected WWWForm ParseFormParameters(string[] parameters)
    {
        
        WWWForm form = new WWWForm();
        if (parameters.Length % 2 != 0) throw new ArithmeticException("Invalid number of parameters. The signature is key1, value1, key2, value2 ...");
        for(int i = 0; i < parameters.Length/2; ++i)
        {
            form.AddField(parameters[2 * i], parameters[2 * i + 1]);
            Debug.Log($"{parameters[2 * i]}:{parameters[2 * i + 1]}");
        }
        return form;
    }

    private void Start()
    {
        StartCoroutine(WebRequest("//localhost:3000", XVerseRequestTypes.GET,
            (data) =>
            {
                Debug.Log($"RESPONSE code:{data.ResponseCode}, length:{data.Length}");
                Debug.Log($"RESPONSE BODY ---------------- ");
                Debug.Log(data.Data);
            },
            (data) =>
            {
                Debug.Log($"RESPONSE FAIL code:{data.ResponseCode}, message:{data.Message}");
            }));
    }
}

public class ResponseData
{
    public byte[] Data;
    public long ResponseCode;
    public ulong Length;
}

public class FailResponseData
{
    public long ResponseCode;
    public string Message;
}
