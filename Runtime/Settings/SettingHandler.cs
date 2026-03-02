using UnityEngine;
// Class for unity inspector
// Подумать над названием
public abstract class SettingHandler : ScriptableObject
{
    public abstract void Apply();
    public abstract void Load();
    public abstract void Save();

    public void ApplyAndSave()
    {
        Apply();
        Save();
    }
}
