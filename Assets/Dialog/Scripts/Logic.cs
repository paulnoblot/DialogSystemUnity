using Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Logic : ISerializable
{
    public enum Type
    {
        Bool,
        Enum,
        String,
        
        Double,
        Float,
        Int
    }

    public enum Comparison
    {
        Equal,
        NotEqual,
        Less,
        LessOrEqual,
        Greater,
        GreaterOrEqual,
    }

    static List<KeyValuePair<Type, Comparison>> possibleComparison = new List<KeyValuePair<Type, Comparison>>()
    {
        new KeyValuePair<Type, Comparison>(Type.Bool, Comparison.Equal),
        new KeyValuePair<Type, Comparison>(Type.Bool, Comparison.NotEqual),

        new KeyValuePair<Type, Comparison>(Type.Enum, Comparison.Equal),
        new KeyValuePair<Type, Comparison>(Type.Enum, Comparison.NotEqual),

        new KeyValuePair<Type, Comparison>(Type.String, Comparison.Equal),
        new KeyValuePair<Type, Comparison>(Type.String, Comparison.NotEqual),

        new KeyValuePair<Type, Comparison>(Type.Double, Comparison.Equal),
        new KeyValuePair<Type, Comparison>(Type.Double, Comparison.NotEqual),
        new KeyValuePair<Type, Comparison>(Type.Double, Comparison.Less),
        new KeyValuePair<Type, Comparison>(Type.Double, Comparison.LessOrEqual),
        new KeyValuePair<Type, Comparison>(Type.Double, Comparison.Greater),
        new KeyValuePair<Type, Comparison>(Type.Double, Comparison.GreaterOrEqual),

        new KeyValuePair<Type, Comparison>(Type.Float, Comparison.Equal),
        new KeyValuePair<Type, Comparison>(Type.Float, Comparison.NotEqual),
        new KeyValuePair<Type, Comparison>(Type.Float, Comparison.Less),
        new KeyValuePair<Type, Comparison>(Type.Float, Comparison.LessOrEqual),
        new KeyValuePair<Type, Comparison>(Type.Float, Comparison.Greater),
        new KeyValuePair<Type, Comparison>(Type.Float, Comparison.GreaterOrEqual),

        new KeyValuePair<Type, Comparison>(Type.Int, Comparison.Equal),
        new KeyValuePair<Type, Comparison>(Type.Int, Comparison.NotEqual),
        new KeyValuePair<Type, Comparison>(Type.Int, Comparison.Less),
        new KeyValuePair<Type, Comparison>(Type.Int, Comparison.LessOrEqual),
        new KeyValuePair<Type, Comparison>(Type.Int, Comparison.Greater),
        new KeyValuePair<Type, Comparison>(Type.Int, Comparison.GreaterOrEqual)
    };

    public static List<Comparison> GetComparisonForType(Type t)
    {
        return possibleComparison.Where(x => x.Key == t).Select(x => x.Value).ToList(); 
    }

    public static bool Evaluate(Type type, Comparison comparison, object a, object b)
    {
        int v = (int)type * 10 + (int)comparison;
        switch (v)
        {
            // Bool
            case 00: // Equal
                return (bool)a == (bool)b;
            case 01: // NotEqual
                return (bool)a != (bool)b;

            // Enum
            case 10: // Equal
                return (int)a == (int)b;
            case 11: // NotEqual
                return (int)a != (int)b;

            // String
            case 20: // Equal
                return (string)a == (string)b;
            case 21: // NotEqual
                return (string)a == (string)b;

            // // Char
            // case 30: // Equal
            //     return (char)a == (char)b;
            // case 31: // NotEqual
            //     return (char)a != (char)b;
            // case 32: // Less
            //     return (char)a < (char)b;
            // case 33: // LessOrEqual
            //     return (char)a <= (char)b;
            // case 34: // Greater
            //     return (char)a > (char)b;
            // case 35: // GreaterOrEqual
            //     return (char)a >= (char)b;

            // Double
            case 30: // Equal
                return (double)a == (double)b;
            case 31: // NotEqual
                return (double)a != (double)b;
            case 32: // Less
                return (double)a < (double)b;
            case 33: // LessOrEqual
                return (double)a <= (double)b;
            case 34: // Greater
                return (double)a > (double)b;
            case 35: // GreaterOrEqual
                return (double)a >= (double)b;

            // Float
            case 40: // Equal
                return (float)a == (float)b;
            case 41: // NotEqual
                return (float)a != (float)b;
            case 42: // Less
                return (float)a < (float)b;
            case 43: // LessOrEqual
                return (float)a <= (float)b;
            case 44: // Greater
                return (float)a > (float)b;
            case 45: // GreaterOrEqual
                return (float)a >= (float)b;

            // Int
            case 50: // Equal
                return (int)a == (int)b;
            case 51: // NotEqual
                return (int)a != (int)b;
            case 52: // Less
                return (int)a < (int)b;
            case 53: // LessOrEqual
                return (int)a <= (int)b;
            case 54: // Greater
                return (int)a > (int)b;
            case 55: // GreaterOrEqual
                return (int)a >= (int)b;
        }
        return false;
    }

    public List<LogicInfo> logicInfos;
    public int type;
    public int comparison;

    bool comparerBool = false;
    string comparerString = "";
    double comparerDouble = 0;
    float comparerFloat = 0;
    int comparerInt = 0;

    public object Comparer
    {
        get
        {
            switch ((Type)type)
            {
                case Type.Bool:
                    return comparerBool;
                case Type.Enum:
                case Type.Int:
                    return comparerInt;
                case Type.String:
                    return comparerString;
                case Type.Double:
                    return comparerDouble;
                case Type.Float:
                    return comparerFloat;
            }
            return null;
        }
        set
        {
            switch ((Type)type)
            {
                case Type.Bool:
                    comparerBool = (bool)value;
                    break;
                case Type.Enum:
                case Type.Int:
                    comparerInt = (int)value;
                    break;
                case Type.String:
                    comparerString = (string)value;
                    break;
                case Type.Double:
                    comparerDouble = (double) value;
                    break;
                case Type.Float:
                    comparerFloat = (float) value;
                    break;
            }
        }
    }

    public Logic()
    {
        logicInfos = new List<LogicInfo>();
        type = 0;
        comparison = 0;
    }

    public bool Evaluate(State state)
    {
        object obj = state;
        foreach (LogicInfo info in logicInfos)
        {
            obj = info.GetValue(obj);
            if (obj == null)
                return false;
        }
        return Evaluate((Type)type, (Comparison)comparison, obj, Comparer);
    }

    public System.Type GetLastObjectType(State state)
    {
        if (logicInfos.Count > 0)
            return logicInfos.Last().GetValueType();
        return state.GetType();
    }

    public void Serialize(Serializer _serializer)
    {
        _serializer.Serialize(logicInfos);

        _serializer.Serialize(ref comparerBool);
        _serializer.Serialize(ref comparerString);
        _serializer.Serialize(ref comparerDouble);
        _serializer.Serialize(ref comparerFloat);
        _serializer.Serialize(ref comparerInt);

        _serializer.Serialize(ref type);
        _serializer.Serialize(ref comparison);
    }
}