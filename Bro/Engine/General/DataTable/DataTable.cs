using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Bro.Json;

namespace Bro
{
    #pragma warning disable 659
    public class DataTable : Dictionary<object, object>, IDataTable 
    {
        public bool Exist(object key)
        {
            return ContainsKey(key);
        }

        public override string ToString()
        {
            var result = string.Empty;
            foreach (var pair in this)
            {
                result += "\n" + pair.Key + " <=> ";
                var valueType = pair.Value.GetType();


                if (valueType.IsArray && IsSimpleTypeArray(valueType))
                {
                    if (valueType == typeof(byte[]))
                    {
                        result += "[";
                        var bytes = (byte[]) pair.Value;
                        foreach (var b in bytes)
                        {
                            result += (int) b + ", ";
                        }
                        result += "]";
                    }
                    else
                    {
                        result += JsonConvert.SerializeObject(pair.Value);
                    }
                }
                else
                {
                    result += pair.Value;
                }
            }

            return result;
        }

        private static bool IsSimpleTypeArray(System.Type valueType)
        {
            return valueType == typeof(byte[]) || valueType == typeof(int[]) || valueType == typeof(float[]) || valueType == typeof(short[]) || valueType == typeof(long[]);
        }

        public IDataTable Clone()
        {
            var newTable = new DataTable();
            foreach (var pair in this)
            {
                if (pair.Value is DataTable)
                {
                    newTable[pair.Key] = ((DataTable) pair.Value).Clone();
                }
                else
                {
                    newTable[pair.Key] = pair.Value;
                }
            }

            return newTable;
        }


        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            var other = obj as DataTable;
            if (other == null)
            {
                return false;
            }

            return Equals(this, other);
        }

        public static bool Equals(DataTable origin, DataTable other)
        {
            if (other == null && origin == null)
            {
                return true;
            }
            if (other != null && origin == null || other == null && origin != null)
            {
                return false;
            }

            foreach (var otherPair in other)
            {
                if (!origin.Exist(otherPair.Key))
                {
                    return false;
                }
                if (!origin[otherPair.Key].Equals(otherPair.Value))
                {
                    return false;
                }
            }

            foreach (var originPair in origin)
            {
                if (!other.Exist(originPair.Key))
                {
                    return false;
                }
                if (!other[originPair.Key].Equals(originPair.Value))
                {
                    return false;
                }
            }
            return true;
        }
    }
}