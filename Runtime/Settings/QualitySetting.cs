
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;



[CreateAssetMenu]
public class QualitySetting : Setting<int>
{
    [SerializeField] private int defaultLevel;

    public override int CurrentValue { get => currentValue; }

    private int currentValue = 0;

    public override void SetValue(int value)
    {
        currentValue = value;
    }

    public override void Apply()
    {
        QualitySettings.SetQualityLevel(currentValue);      
    }

    public override void Load()
    {
        currentValue = PlayerPrefs.GetInt(key, defaultLevel);
    }

    public override void Save()
    {
        PlayerPrefs.SetInt(key, currentValue);
    }
  
}

