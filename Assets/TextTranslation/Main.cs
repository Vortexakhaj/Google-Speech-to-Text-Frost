using UnityEngine;
using UnityEngine.UI;
using UniLang;
using System.Collections.Generic;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition.Examples
{
    public class Main : MonoBehaviour
    {
        GCSR_Example _gcsr;

        public Text textInput;
        public Button translateBtn;
        public Text resultText;
        //public Dropdown languageType;

        void Start()
        {
            _gcsr = GetComponent<GCSR_Example>();
            
            var languages = new List<string>();
            // 中文简体
            //languages.Add("zh-cn");
            // 中文繁体
            //languages.Add("zh-tw");
            // 英语
            //languages.Add("en");
            // 日语
            languages.Add("kn");
            //languages.Add("ja");
            // 韩语
            //languages.Add("ko");
            // 法语
            //languages.Add("fr");
            // 德语
            //languages.Add("de");
            // 俄语
            //languages.Add("ru");
            //languageType.AddOptions(languages);

            translateBtn.onClick.AddListener(Translate);

            //languageType.onValueChanged.AddListener((v) => { Translate(); });
        }

        public void Translate()
        {
            Translator.Do("en", "hi", textInput.text, (translated_str) => {resultText.text = translated_str;});
        }
    }
}