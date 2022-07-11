using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class XVerseWeb 
{
    public enum XVerseRequestTypes{
        GET, POST, DOWNLOAD
    }

    /// <summary>
    /// This somehow DOES NOT WORK ON THE LOCAL.
    /// </summary>
    /// <param name="URL"></param>
    /// <param name="type"> use XVerseRequestTypes</param>
    /// <param name="successHandler"> callback on success </param>
    /// <param name="failHandler"> default null, callback on failure</param>
    /// <param name="formData">for POST: key1, value1, key2, value2, .... must be strings.</param>
    /// <returns></returns>
    public static IEnumerator WebRequest(string URL, XVerseRequestTypes type, 
        Action<ResponseData> successHandler, Action<FailResponseData> failHandler = null, params string[] formData)
    {
        switch (type)
        {
            case XVerseRequestTypes.GET:
                using (UnityWebRequest request = UnityWebRequest.Get(URL))
                {
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
                break;

            case XVerseRequestTypes.POST:
                WWWForm form = ParseFormParameters(formData);
                using (UnityWebRequest request = UnityWebRequest.Post(URL, form))
                {
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
                break;

        }
        // send and wait
        //request.SetRequestHeader("Accept", "text/plain");
        //request.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/103.0.0.0 Safari/537.36");
        //request.SetRequestHeader("Accept-Encoding", "gzip, deflate, br");
        //request.SetRequestHeader("Connection", "keep-alive");
    }


    protected static WWWForm ParseFormParameters(string[] parameters)
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

    /// <summary>
    /// Local get request, uses the HttpWebRequest instead of UnityWebRequest, which does not work locally.
    /// </summary>
    /// <param name="URL"></param>
    /// <param name="successHandler"></param>
    /// <param name="failHandler"></param>
    /// <param name="timeOut"> in milliseconds, cancel request if no response for this amount of time.</param>
    public static void GetWebRequestLocal(string URL,
        Action<ResponseData> successHandler, Action<FailResponseData> failHandler = null, int timeOut = 30 * 1000)
    {
        try
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(URL);
            request.Method = "GET";
            request.Timeout = 30 * 1000;

            using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
            {
                HttpStatusCode status = resp.StatusCode;

                if (status == HttpStatusCode.OK)
                {
                    ResponseData data = new ResponseData();
                    var sr = new StreamReader(resp.GetResponseStream());
                    string body = sr.ReadToEnd();

                    data.Data = Encoding.UTF8.GetBytes(body);
                    data.Length = (ulong)resp.ContentLength;
                    data.ResponseCode = (long)resp.StatusCode;
                    successHandler(data);
                }
                else
                {
                    FailResponseData data = new FailResponseData();
                    data.ResponseCode = (long)resp.StatusCode;
                    data.Message = resp.StatusDescription;
                    failHandler(data);

                }
            }
        }
        catch (WebException ex)
        {
            FailResponseData data = new FailResponseData();
            data.ResponseCode = (long)ex.Status;
            data.Message = ex.Message;
            failHandler(data);
        }
    }

    private void Start()
    {
        /*
        StartCoroutine(WebRequest("http://localhost:3000/", XVerseRequestTypes.GET,
            (data) =>
            {
                Debug.Log($"RESPONSE code:{data.ResponseCode}, length:{data.Length}");
                Debug.Log($"RESPONSE BODY ---------------- ");
                Debug.Log(Encoding.UTF8.GetString(data.Data));
            },
            (data) =>
            {
                Debug.Log($"RESPONSE FAIL code:{data.ResponseCode}, message:{data.Message}");
            }));
        */

        
    }
}

public class ResponseData
{
    public byte[] Data;
    public long ResponseCode;
    public ulong Length;

    public string ResponseString {
        get {
            return Encoding.UTF8.GetString(Data);
        }
    }
}

public class FailResponseData
{
    public long ResponseCode;
    public string Message;
}
