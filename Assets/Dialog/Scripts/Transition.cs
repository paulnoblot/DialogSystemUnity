using Serialization;
using System.Collections.Generic;

public class Transition : ISerializable
{
    public int idNode = 0;
    public bool allTrue = true;
    public List<Logic> logics = new List<Logic>();

    public bool Evaluate(State state)
    {
        if (allTrue)
        {
            foreach (Logic l in logics)
                if (!l.Evaluate(state))
                    return false;
            return true;
        }
        else
        {
            foreach (Logic l in logics)
                if (l.Evaluate(state))
                    return true;
            return false;
        }
    }

    public void Serialize(Serializer _serializer)
    {
        _serializer.Serialize(ref idNode);
        _serializer.Serialize(ref allTrue);
        _serializer.Serialize(logics);
    }
}