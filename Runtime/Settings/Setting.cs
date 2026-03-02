using UnityEngine;


public abstract class Setting<TValue> : SettingHandler
{
    [SerializeField] protected string key; 

    public string Key => key;

    public abstract TValue CurrentValue { get; }

    public virtual void SetValue(TValue value)
    {
        throw new System.NotImplementedException();
    }

    public virtual bool IsMinValue { get; protected set; }
    public virtual bool IsMaxValue { get; protected set; }

    public virtual void SetNextValue()
    {
        throw new System.NotImplementedException();
    }
    public virtual void SetPreviousValue()
    {
        throw new System.NotImplementedException();
    }
}