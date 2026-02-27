
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


// Как раз тут можно запилить названием всех уровней
[CreateAssetMenu]
public class QualitySetting : Setting<int>
{
    [SerializeField] private int defaultLevel;

    public List<string> Titles; // Временно

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

