
using System;
using UnityEngine;
using UnityEngine.Audio;

namespace CodeBase
{
    // Говнонастройки 
    [CreateAssetMenu]
    public class AudioMixerFloatSetting : Setting<float>
    {
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private string propertyName;

        [SerializeField] private float realStep;
        [SerializeField] private float minRealValue;
        [SerializeField] private float maxRealValue;


        [SerializeField] private float minVirtualValue;
        [SerializeField] private float maxVirtualValue;

        public override bool IsMinValue { get => currentValue == minRealValue; }
        public override bool IsMaxValue { get => currentValue == maxRealValue; }
        public override float CurrentValue { get => currentValue; }

        private float currentValue = 0;

        public override void SetNextValue()
        {
            AddValue(realStep);
        }

        public override void SetPreviousValue()
        {
            AddValue(-realStep);
        }

        private void AddValue(float value)
        {
            currentValue += value;
            currentValue = Mathf.Clamp(currentValue, minRealValue, maxRealValue);
        }

        public override void Apply()
        {
            audioMixer.SetFloat(propertyName, currentValue);
        }

        public override void Load()
        {
            currentValue = PlayerPrefs.GetFloat(key, 0);
        }

        public override void Save()
        {
            PlayerPrefs.SetFloat(key, currentValue);
        }



    }
}

