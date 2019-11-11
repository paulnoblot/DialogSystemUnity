using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using UnityEngine;

namespace Serialization
{
    public interface ISerializable
    {
        void Serialize(Serializer _serializer);
    }

    public class Serializer
    {
        public class Writer : Serializer
        {
            public Writer() : base(Mode.Write)
            { byteWriteArray = new List<byte>(); }
        }

        public class Reader : Serializer
        {
            public Reader() : base(Mode.Read) { }
        }

        public enum Mode { Read, Write }

        public readonly Mode mode = Mode.Read;
        private List<byte> byteWriteArray = null; // used to write
        private byte[] byteReadArray = null; // used to read
        private int cursor = 0;

        private Serializer(Mode _mode)
        {
            mode = _mode;
        }

        public static Serializer CreateFromFile(string _path)
        {
            Serializer s = new Serializer(Mode.Read);
            s.byteReadArray = File.ReadAllBytes(_path);
            return s;
        }

        public void WriteBytesToFile(string _path)
        {
            File.WriteAllBytes(_path, byteWriteArray.ToArray());
        }

        public static void ReadFromFile<T>(string _path, out T _serializable)
            where T : ISerializable, new()
        {
            Serializer s = new Serializer(Mode.Read);
            s.byteReadArray = File.ReadAllBytes(_path);

            _serializable = new T();
            s.Serialize(ref _serializable);
        }
        public static void WriteToFile<T>(string _path, T _serializable)
            where T : ISerializable, new()
        {
            Serializer s = new Serializer(Mode.Write);
            s.byteWriteArray = new List<byte>();
            s.Serialize(ref _serializable);

            File.WriteAllBytes(_path, s.byteWriteArray.ToArray());
        }

        public static void ReadFromBytes<T>(List<byte> _bytes, out T _serializable)
            where T : ISerializable, new()
        {
            _serializable = new T();
            if (_bytes != null)
            {
                Serializer s = new Serializer(Mode.Read);
                s.byteReadArray = _bytes.ToArray();

                s.Serialize(ref _serializable);
            }
        }
        public static void WriteToBytes<T>(List<byte> _bytes, T _serializable)
            where T : ISerializable, new()
        {
            if (_bytes != null)
            {
                Serializer s = new Serializer(Mode.Write);
                s.byteWriteArray = new List<byte>();
                s.Serialize(ref _serializable);

                _bytes = s.byteWriteArray;
            }
        }

        // ISerializable
        public void Serialize<T>(ref T _serializable)
            where T : ISerializable, new()
        {
            Type type = (mode == Mode.Write) ? _serializable.GetType() : null;

            Serialize(ref type);

            if (mode == Mode.Read && type != null) // create the right type to handle polymorphism
            {
                _serializable = (T)Activator.CreateInstance(type);
            }

            _serializable.Serialize(this);
        }

        // polymorphism
        public void Serialize(ref Type _value)
        {
            string typeName = (mode == Mode.Write) ? _value.FullName : "";
            Serialize(ref typeName);

            if (mode == Mode.Read)
            {
                _value = Type.GetType(typeName);

                if (_value == null) // type doesn't exists
                {
                    throw new Exception("Serialize Error: Type \"" + typeName + "\" doesn't exists.");
                }
            }
        }

        // base types
        public void Serialize(ref byte _value)
        {
            switch (mode)
            {
                case Mode.Read:
                    _value = byteReadArray[cursor];
                    break;
                case Mode.Write:
                    byteWriteArray.Add(_value);
                    break;
            }
            cursor += sizeof(byte);
        }
        public void Serialize(ref bool _value)
        {
            switch (mode)
            {
                case Mode.Read:
                    _value = BitConverter.ToBoolean(byteReadArray, cursor);
                    break;
                case Mode.Write:
                    byteWriteArray.AddRange(BitConverter.GetBytes(_value));
                    break;
            }
            cursor += sizeof(bool);
        }
        public void Serialize(ref short _value)
        {
            switch (mode)
            {
                case Mode.Read:
                    _value = BitConverter.ToInt16(byteReadArray, cursor);
                    break;
                case Mode.Write:
                    byteWriteArray.AddRange(BitConverter.GetBytes(_value));
                    break;
            }
            cursor += sizeof(short);
        }
        public void Serialize(ref int _value)
        {
            switch (mode)
            {
                case Mode.Read:
                    _value = BitConverter.ToInt32(byteReadArray, cursor);
                    break;
                case Mode.Write:
                    byteWriteArray.AddRange(BitConverter.GetBytes(_value));
                    break;
            }
            cursor += sizeof(int);
        }
        public void Serialize(ref long _value)
        {
            switch (mode)
            {
                case Mode.Read:
                    _value = BitConverter.ToInt64(byteReadArray, cursor);
                    break;
                case Mode.Write:
                    byteWriteArray.AddRange(BitConverter.GetBytes(_value));
                    break;
            }
            cursor += sizeof(long);
        }
        public void Serialize(ref ushort _value)
        {
            switch (mode)
            {
                case Mode.Read:
                    _value = BitConverter.ToUInt16(byteReadArray, cursor);
                    break;
                case Mode.Write:
                    byteWriteArray.AddRange(BitConverter.GetBytes(_value));
                    break;
            }
            cursor += sizeof(ushort);
        }
        public void Serialize(ref uint _value)
        {
            switch (mode)
            {
                case Mode.Read:
                    _value = BitConverter.ToUInt32(byteReadArray, cursor);
                    break;
                case Mode.Write:
                    byteWriteArray.AddRange(BitConverter.GetBytes(_value));
                    break;
            }
            cursor += sizeof(uint);
        }

        internal void Serialize(ref object test)
        {
            throw new NotImplementedException();
        }

        public void Serialize(ref ulong _value)
        {
            switch (mode)
            {
                case Mode.Read:
                    _value = BitConverter.ToUInt64(byteReadArray, cursor);
                    break;
                case Mode.Write:
                    byteWriteArray.AddRange(BitConverter.GetBytes(_value));
                    break;
            }
            cursor += sizeof(ulong);
        }
        public void Serialize(ref float _value)
        {
            switch (mode)
            {
                case Mode.Read:
                    _value = BitConverter.ToSingle(byteReadArray, cursor);
                    break;
                case Mode.Write:
                    byteWriteArray.AddRange(BitConverter.GetBytes(_value));
                    break;
            }
            cursor += sizeof(float);
        }
        public void Serialize(ref double _value)
        {
            switch (mode)
            {
                case Mode.Read:
                    _value = BitConverter.ToDouble(byteReadArray, cursor);
                    break;
                case Mode.Write:
                    byteWriteArray.AddRange(BitConverter.GetBytes(_value));
                    break;
            }
            cursor += sizeof(double);
        }
        public void Serialize(ref char _value)
        {
            switch (mode)
            {
                case Mode.Read:
                    _value = BitConverter.ToChar(byteReadArray, cursor);
                    break;
                case Mode.Write:
                    byteWriteArray.AddRange(BitConverter.GetBytes(_value));
                    break;
            }
            cursor += sizeof(char);
        }
        public void Serialize(ref string _value)
        {
            int length = (mode == Mode.Write) ? _value.Length : 0;
            // write the input string length in write mode, 
            // or override the previous value with incoming string length in read mode
            Serialize(ref length);

            switch (mode)
            {
                case Mode.Read:
                    _value = Encoding.Unicode.GetString(byteReadArray, cursor, length * sizeof(char));
                    break;
                case Mode.Write:
                    byteWriteArray.AddRange(Encoding.Unicode.GetBytes(_value));
                    break;
            }
            cursor += length * sizeof(char);
        }

        // Array types
        public void Serialize<T>(ICollection<T> _array)
            where T : ISerializable, new()
        {
            int length = (mode == Mode.Write) ? _array.Count : 0;
            Serialize(ref length);
            T element;
            switch (mode)
            {
                case Mode.Read:
                    for (int i = 0; i < length; i++)
                    {
                        element = new T();
                        Serialize(ref element);
                        _array.Add(element);
                    }
                    break;
                case Mode.Write:
                    for (int i = 0; i < length; i++)
                    {
                        element = _array.ElementAt(i);
                        Serialize(ref element);
                    }
                    break;
            }
        }
        public void Serialize(ICollection<byte> _array)
        {
            int length = (mode == Mode.Write) ? _array.Count : 0;
            Serialize(ref length);
            
            switch (mode)
            {
                case Mode.Read:
                    for (int i = 0; i < length; i++)
                    {
                        _array.Add(byteReadArray[cursor + i]);
                    }
                    break;
                case Mode.Write:
                    for (int i = 0; i < length; i++)
                    {
                        byteWriteArray.Add(_array.ElementAt(i));
                    }
                    break;
            }
            cursor += length * sizeof(byte);
        }
        public void Serialize(ICollection<bool> _array)
        {
            int length = (mode == Mode.Write) ? _array.Count : 0;
            Serialize(ref length);

            bool element = false;
            switch (mode)
            {
                case Mode.Read:
                    for (int i = 0; i < length; i++)
                    {
                        Serialize(ref element);
                        _array.Add(element);
                    }
                    break;
                case Mode.Write:
                    for (int i = 0; i < length; i++)
                    {
                        element = _array.ElementAt(i);
                        Serialize(ref element);
                    }
                    break;
            }
        }
        public void Serialize(ICollection<short> _array)
        {
            int length = (mode == Mode.Write) ? _array.Count : 0;
            Serialize(ref length);

            short element = 0;
            switch (mode)
            {
                case Mode.Read:
                    for (int i = 0; i < length; i++)
                    {
                        Serialize(ref element);
                        _array.Add(element);
                    }
                    break;
                case Mode.Write:
                    for (int i = 0; i < length; i++)
                    {
                        element = _array.ElementAt(i);
                        Serialize(ref element);
                    }
                    break;
            }
        }
        public void Serialize(ICollection<int> _array)
        {
            int length = (mode == Mode.Write) ? _array.Count : 0;
            Serialize(ref length);

            int element = 0;
            switch (mode)
            {
                case Mode.Read:
                    for (int i = 0; i < length; i++)
                    {
                        Serialize(ref element);
                        _array.Add(element);
                    }
                    break;
                case Mode.Write:
                    for (int i = 0; i < length; i++)
                    {
                        element = _array.ElementAt(i);
                        Serialize(ref element);
                    }
                    break;
            }
        }
        public void Serialize(ICollection<long> _array)
        {
            int length = (mode == Mode.Write) ? _array.Count : 0;
            Serialize(ref length);

            long element = 0;
            switch (mode)
            {
                case Mode.Read:
                    for (int i = 0; i < length; i++)
                    {
                        Serialize(ref element);
                        _array.Add(element);
                    }
                    break;
                case Mode.Write:
                    for (int i = 0; i < length; i++)
                    {
                        element = _array.ElementAt(i);
                        Serialize(ref element);
                    }
                    break;
            }
        }
        public void Serialize(ICollection<ushort> _array)
        {
            int length = (mode == Mode.Write) ? _array.Count : 0;
            Serialize(ref length);

            ushort element = 0;
            switch (mode)
            {
                case Mode.Read:
                    for (int i = 0; i < length; i++)
                    {
                        Serialize(ref element);
                        _array.Add(element);
                    }
                    break;
                case Mode.Write:
                    for (int i = 0; i < length; i++)
                    {
                        element = _array.ElementAt(i);
                        Serialize(ref element);
                    }
                    break;
            }
        }
        public void Serialize(ICollection<uint> _array)
        {
            int length = (mode == Mode.Write) ? _array.Count : 0;
            Serialize(ref length);

            uint element = 0;
            switch (mode)
            {
                case Mode.Read:
                    for (int i = 0; i < length; i++)
                    {
                        Serialize(ref element);
                        _array.Add(element);
                    }
                    break;
                case Mode.Write:
                    for (int i = 0; i < length; i++)
                    {
                        element = _array.ElementAt(i);
                        Serialize(ref element);
                    }
                    break;
            }
        }
        public void Serialize(ICollection<ulong> _array)
        {
            int length = (mode == Mode.Write) ? _array.Count : 0;
            Serialize(ref length);

            ulong element = 0;
            switch (mode)
            {
                case Mode.Read:
                    for (int i = 0; i < length; i++)
                    {
                        Serialize(ref element);
                        _array.Add(element);
                    }
                    break;
                case Mode.Write:
                    for (int i = 0; i < length; i++)
                    {
                        element = _array.ElementAt(i);
                        Serialize(ref element);
                    }
                    break;
            }
        }
        public void Serialize(ICollection<float> _array)
        {
            int length = (mode == Mode.Write) ? _array.Count : 0;
            Serialize(ref length);

            float element = 0;
            switch (mode)
            {
                case Mode.Read:
                    for (int i = 0; i < length; i++)
                    {
                        Serialize(ref element);
                        _array.Add(element);
                    }
                    break;
                case Mode.Write:
                    for (int i = 0; i < length; i++)
                    {
                        element = _array.ElementAt(i);
                        Serialize(ref element);
                    }
                    break;
            }
        }
        public void Serialize(ICollection<double> _array)
        {
            int length = (mode == Mode.Write) ? _array.Count : 0;
            Serialize(ref length);

            double element = 0;
            switch (mode)
            {
                case Mode.Read:
                    for (int i = 0; i < length; i++)
                    {
                        Serialize(ref element);
                        _array.Add(element);
                    }
                    break;
                case Mode.Write:
                    for (int i = 0; i < length; i++)
                    {
                        element = _array.ElementAt(i);
                        Serialize(ref element);
                    }
                    break;
            }
        }
        public void Serialize(ICollection<char> _array)
        {
            int length = (mode == Mode.Write) ? _array.Count : 0;
            Serialize(ref length);

            char element = '\0';
            switch (mode)
            {
                case Mode.Read:
                    for (int i = 0; i < length; i++)
                    {
                        Serialize(ref element);
                        _array.Add(element);
                    }
                    break;
                case Mode.Write:
                    for (int i = 0; i < length; i++)
                    {
                        element = _array.ElementAt(i);
                        Serialize(ref element);
                    }
                    break;
            }
        }
        public void Serialize(ICollection<string> _array)
        {
            int length = (mode == Mode.Write) ? _array.Count : 0;
            Serialize(ref length);

            string element = "";
            switch (mode)
            {
                case Mode.Read:
                    for (int i = 0; i < length; i++)
                    {
                        Serialize(ref element);
                        _array.Add(element);
                    }
                    break;
                case Mode.Write:
                    for (int i = 0; i < length; i++)
                    {
                        element = _array.ElementAt(i);
                        Serialize(ref element);
                    }
                    break;
            }
        }

        // C# complex type
        public void Serialize(ref DateTime _value)
        {
            long binaryDate = (mode == Mode.Write) ? _value.ToBinary() : 0;
            Serialize(ref binaryDate);

            if(mode == Mode.Read)
            {
                _value = DateTime.FromBinary(binaryDate);
            }
        }

        // unity basic types
        public void Serialize(ref Vector2 _value)
        {
            Serialize(ref _value.x);
            Serialize(ref _value.y);
        }
        public void Serialize(ref Vector3 _value)
        {
            Serialize(ref _value.x);
            Serialize(ref _value.y);
            Serialize(ref _value.z);
        }
        public void Serialize(ref Vector4 _value)
        {
            Serialize(ref _value.x);
            Serialize(ref _value.y);
            Serialize(ref _value.z);
            Serialize(ref _value.w);
        }
        public void Serialize(ref Vector2Int _value)
        {
            int x = _value.x;
            int y = _value.y;
            Serialize(ref x);
            Serialize(ref y);
            _value.x = x;
            _value.y = y;
        }
        public void Serialize(ref Vector3Int _value)
        {
            int x = _value.x;
            int y = _value.y;
            int z = _value.z;
            Serialize(ref x);
            Serialize(ref y);
            Serialize(ref z);
            _value.x = x;
            _value.y = y;
            _value.z = z;
        }
        public void Serialize(ref Quaternion _value)
        {
            Serialize(ref _value.x);
            Serialize(ref _value.y);
            Serialize(ref _value.z);
            Serialize(ref _value.w);
        }
        public void Serialize(ref Matrix4x4 _value)
        {
            Serialize(ref _value.m00);
            Serialize(ref _value.m01);
            Serialize(ref _value.m02);
            Serialize(ref _value.m03);
            Serialize(ref _value.m10);
            Serialize(ref _value.m11);
            Serialize(ref _value.m12);
            Serialize(ref _value.m13);
            Serialize(ref _value.m20);
            Serialize(ref _value.m21);
            Serialize(ref _value.m22);
            Serialize(ref _value.m23);
            Serialize(ref _value.m30);
            Serialize(ref _value.m31);
            Serialize(ref _value.m32);
            Serialize(ref _value.m33);
        }
        public void Serialize(ref Color _value)
        {
            Serialize(ref _value.r);
            Serialize(ref _value.g);
            Serialize(ref _value.b);
            Serialize(ref _value.a);
        }
        public void Serialize(ref Color32 _value)
        {
            Serialize(ref _value.r);
            Serialize(ref _value.g);
            Serialize(ref _value.b);
            Serialize(ref _value.a);
        }

        // DON'T KNOW HOW TO DESERIALIZE GAMEOBJECT REF
        //public void Serialize(ref GameObject _value)
        //{

        //    int id = (mode == Mode.Read) ? _value.guid() : 0;
        //    Serialize(ref id);

        //    if (mode == Mode.Read)
        //    {
        //        _value = ???
        //    }

        //}
    }


    //public static class ICollectionExt
    //{
    //    public static void Serialize<T>(this ICollection<T> _array, Serializer _s)
    //    where T : ISerializable, new()
    //    {
    //        int length = (_s.mode == Serializer.Mode.Write) ? _array.Count : 0;
    //        _s.Serialize(ref length);
    //        T element;
    //        switch (_s.mode)
    //        {
    //            case Serializer.Mode.Read:
    //                for (int i = 0; i < length; i++)
    //                {
    //                    element = new T();
    //                    _s.Serialize(ref element);
    //                    _array.Add(element);
    //                }
    //                break;
    //            case Serializer.Mode.Write:
    //                for (int i = 0; i < length; i++)
    //                {
    //                    element = _array.ElementAt(i);
    //                    _s.Serialize(ref element);
    //                }
    //                break;
    //        }
    //    }
    //}
}
