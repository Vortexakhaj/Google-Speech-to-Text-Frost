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
            // ���ļ���
            //languages.Add("zh-cn");
            // ���ķ���
            //languages.Add("zh-tw");
            // Ӣ��
            //languages.Add("en");
            // ����
            languages.Add("kn");
            //languages.Add("ja");
            // ����
            //languages.Add("ko");
            // ����
            //languages.Add("fr");
            // ����
            //languages.Add("de");
            // ����
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