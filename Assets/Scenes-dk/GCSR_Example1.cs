using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition.Examples
{
    public class GCSR_Example1 : MonoBehaviour
    {
        [SerializeField]
        private GCSpeechRecognition _speechRecognition;

        [SerializeField]
        private Button _startRecord,
                       _stopRecord,
                       _detectThreshold,
                       _recognize;

        [SerializeField]
        private InputField _contextPhrases;

        [SerializeField]
        private Dropdown _language,
                         _microphoneDevice;

        [SerializeField]
        private Text _result;


        // Start is called before the first frame update
        private void Start()
        {
            _startRecord.onClick.AddListener(StartRecordButtonOnClickHandler);
            _stopRecord.onClick.AddListener(StopRecordButtonOnClickHandler);
            _detectThreshold.onClick.AddListener(DetectThresholdButtonOnClickHandler);
            _recognize.onClick.AddListener(RecognizeButtonOnClickHandler);
            _microphoneDevice.onValueChanged.AddListener(MicrophoneDevicesDropdownOnValueChangedEventHandler);

            _language.ClearOptions();

            for (int i = 0; i < Enum.GetNames(typeof(Enumerators.LanguageCode)).Length; i++)
            {
                _language.options.Add(new Dropdown.OptionData(((Enumerators.LanguageCode)i).Parse()));
            }

            _language.value = _language.options.IndexOf(_language.options.Find(x => x.text == Enumerators.LanguageCode.en_GB.Parse()));

            _microphoneDevice.ClearOptions();

            for (int i = 0; i < _speechRecognition.GetMicrophoneDevices().Length; i++)
            {
                _microphoneDevice.options.Add(new Dropdown.OptionData(_speechRecognition.GetMicrophoneDevices()[i]));
            }

            //smart fix of dropdowns
            _microphoneDevice.value = 1;
            _microphoneDevice.value = 0;

            _speechRecognition.RecognizeSuccessEvent += RecognizeSuccessEventHandler;
            _speechRecognition.RecognizeFailedEvent += RecognizeFailedEventHandler;

            _speechRecognition.StartedRecordEvent += StartedRecordEventHandler;
            _speechRecognition.RecordFailedEvent += RecordFailedEventHandler;

            _speechRecognition.EndTalkigEvent += EndTalkigEventHandler;
        }

        private void RecognizeSuccessEventHandler(RecognitionResponse recognitionResponse)
        {
            _result.text = "Recognize Success.";

            if (recognitionResponse == null || recognitionResponse.results.Length == 0)
            {
                _result.text = "\nWords not detected.";
                return;
            }

            _result.text = "\n" + recognitionResponse.results[0].alternatives[0].transcript;

            var words = recognitionResponse.results[0].alternatives[0].words;

            if (words != null)
            {
                string times = string.Empty;

                foreach (var item in recognitionResponse.results[0].alternatives[0].words)
                {
                    times += "<color=green>" + item.word + "</color> -  start: " + item.startTime + "; end: " + item.endTime + "\n";
                }

                _result.text += "\n" + times;
            }

            string other = "\nDetected alternatives: ";

            foreach (var result in recognitionResponse.results)
            {
                foreach (var alternative in result.alternatives)
                {
                    if (recognitionResponse.results[0].alternatives[0] != alternative)
                    {
                        other += alternative.transcript + ", ";
                    }
                }
            }

            _result.text += other;
        }

        private void OnDestroy()
        {
            _speechRecognition.RecognizeSuccessEvent -= RecognizeSuccessEventHandler;
            _speechRecognition.RecognizeFailedEvent -= RecognizeFailedEventHandler;

            _speechRecognition.StartedRecordEvent -= StartedRecordEventHandler;
            _speechRecognition.RecordFailedEvent -= RecordFailedEventHandler;

            _speechRecognition.EndTalkigEvent -= EndTalkigEventHandler;
        }

        private void StartedRecordEventHandler()
        {
            _result.text = "StartedRecordEventHandler";
        }

        private void RecordFailedEventHandler()
        {
            _result.text = "RecordFailedEventHandler";
        }

        private void EndTalkigEventHandler(AudioClip clip, float[] raw)
        {
            _result.text = "EndTalkigEventHandler";
        }

        private void RecognizeFailedEventHandler(string error)
        {
            _result.text = "Recognize Failed: " + error;
        }

        private void MicrophoneDevicesDropdownOnValueChangedEventHandler(int value)
        {
            if (!_speechRecognition.HasConnectedMicrophoneDevices())
                return;
            _speechRecognition.SetMicrophoneDevice(_speechRecognition.GetMicrophoneDevices()[value]);
        }

        private void StartRecordButtonOnClickHandler()
        {
            _speechRecognition.StartRecord(false);
        }
        private void StopRecordButtonOnClickHandler()
        {
            _speechRecognition.StopRecord();
        }
        private void DetectThresholdButtonOnClickHandler()
        {
            _speechRecognition.DetectThreshold();
        }
        private void RecognizeButtonOnClickHandler()
        {
            if (_speechRecognition.LastRecordedClip == null || _speechRecognition.LastRecordedRaw == null)
                return;
            RecognitionConfig config = RecognitionConfig.GetDefault();
            config.languageCode = ((Enumerators.LanguageCode)_language.value).Parse();
            config.speechContexts = new SpeechContext[]
            {
                new SpeechContext()
                {
                    phrases = _contextPhrases.text.Replace(" ", string.Empty).Split(',')
                }
            };
            config.audioChannelCount = _speechRecognition.LastRecordedClip.channels;

            GeneralRecognitionRequest recognitionRequest = new GeneralRecognitionRequest()
            {
                audio = new RecognitionAudioContent()
                {
                    content = _speechRecognition.LastRecordedRaw.ToBase64()
                },
                config = config
            };

            _speechRecognition.Recognize(recognitionRequest);
        }
    }
}