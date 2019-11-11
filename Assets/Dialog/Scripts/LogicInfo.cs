using Serialization;
using System;
using System.Reflection;
using UnityEngine;

public class LogicInfo : ISerializable
{
    public enum InfoType
    {
        Attribut,
        Propetry,
        Method
    }

    string name;
    int infoType;
    Type objectType; 

    public string Name { get { return name; } }
    public Type ObjectType { get { return objectType; } }

    public LogicInfo()
    {
        name = "";
        infoType = 0;
        objectType = null;
    }

    public LogicInfo(FieldInfo info, Type objectType)
    {
        name = info.Name;
        infoType = (int)InfoType.Attribut;
        this.objectType = objectType;
    }

    public LogicInfo(PropertyInfo info, Type objectType)
    {
        name = info.Name;
        infoType = (int)InfoType.Propetry;
        this.objectType = objectType;
    }

    public LogicInfo(MethodInfo info, Type objectType)
    {
        name = info.Name;
        infoType = (int)InfoType.Method;
        this.objectType = objectType;
    }

    public LogicInfo(string name, InfoType type, Type objectType)
    {
        this.name = name;
        this.infoType = (int)type;
        this.objectType = objectType;
    }

    public object GetValue(object obj)
    {
        switch ((InfoType)infoType)
        {
            case InfoType.Attribut:
                return obj.GetType().GetField(name).GetValue(obj);
            case InfoType.Propetry:
                return obj.GetType().GetProperty(name).GetValue(obj);
            case InfoType.Method:
                return obj.GetType().GetMethod(name).Invoke(obj, null);
        }
        return null;
    }

    public Type GetValueType()
    {
        switch ((InfoType)infoType)
        {
            case InfoType.Attribut:
                return objectType.GetField(name).FieldType;
            case InfoType.Propetry:
                return objectType.GetProperty(name).GetGetMethod().ReturnType;
            case InfoType.Method:
                return objectType.GetMethod(name).ReturnType;
        }
        return null;
    }

    public void Serialize(Serializer _serializer)
    {
        _serializer.Serialize(ref name);
        _serializer.Serialize(ref infoType);
        _serializer.Serialize(ref objectType);
    }
}
