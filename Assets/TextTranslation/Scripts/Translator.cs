using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace UniLang
{
    /// <summary>
    /// 翻译
    /// </summary>
    public class Translator : MonoBehaviour
    {
        /// <summary>
        /// google翻译api
        /// </summary>
        const string k_Url = "https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}";

        static Translator s_instance;

        /// <summary>
        /// 翻译接口
        /// </summary>
        /// <param name="sourceLang">原始语言类型</param>
        /// <param name="targetLang">目标语言类型</param>
        /// <param name="text">要翻译的文字</param>
        /// <param name="cb">翻译回调</param>
        public static void Do(string sourceLang, string targetLang, string text, Action<string> cb)
        {
            if (null == s_instance)
            {
                var obj = new GameObject("Translation");
                //obj.hideFlags = HideFlags.HideAndDontSave;
                DontDestroyOnLoad(obj);
                s_instance = obj.AddComponent<Translator>();
            }
            s_instance.Run(sourceLang, targetLang, text, cb);
        }


        public void Run(string sourceLang, string targetLand, string text, Action<string> cb)
        {
            StartCoroutine(TranslateAsync(sourceLang, targetLand, text, cb));
        }

        IEnumerator TranslateAsync(string sourceLang, string targetLand, string text, Action<string> cb)
        {
            var requestUrl = String.Format(k_Url, new object[] { sourceLang, targetLand, text });
            Debug.Log("url: " + requestUrl);
            UnityWebRequest req = UnityWebRequest.Get(requestUrl);
            yield return req.SendWebRequest();


            if (string.IsNullOrEmpty(req.error))
            {
                Debug.Log(req.downloadHandler.text);
                JSONArray jsonArray = JSONConvert.DeserializeArray(req.downloadHandler.text);
                jsonArray = (JSONArray)(jsonArray[0]);
                jsonArray = (JSONArray)(jsonArray[0]);
                cb((string)jsonArray[0]);
            }
            else
            {
                cb(req.error);
            }
        }
    }
}
