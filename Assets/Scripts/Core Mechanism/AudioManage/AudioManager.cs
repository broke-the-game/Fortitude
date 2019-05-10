using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AudioManaging {
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        private void Awake()
        {
            Instance = this;
        }

        [SerializeField]
        private string m_audioPath;

        public string AudioPath { get => m_audioPath; }

        [System.Serializable]
        public class DictionaryIntAudioClip : SerializableDictionary<int, AudioClip> { }

        [SerializeField, HideInInspector]
        public DictionaryIntAudioClip AudioClipMap = new DictionaryIntAudioClip();

        public void Play(AudioEnum audioEnum)
        {
            AudioClip clipToPlay;
            if (AudioClipMap.TryGetValue((int)audioEnum, out clipToPlay))
            {
                AudioSource.PlayClipAtPoint(clipToPlay, Camera.main.transform.position);
            }
        }

#if UNITY_EDITOR
        [SerializeField, HideInInspector]
        public List<AudioClip> AudioClipList = new List<AudioClip>();

        public void SetPath_Editor(string path)
        {
            m_audioPath = path;
        }
        [SerializeField]
        public string[] EditableKeys = new string[0];
#endif
    }
}
