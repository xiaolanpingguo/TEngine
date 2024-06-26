using System.Collections;
using System.Collections.Generic;
using System.Text;
using Lockstep.Framework;


namespace Lockstep.Game
{
    public static class SerializeUtil
    {
        public static void Write<T>(this Serializer writer, IList<T> lst) where T : IComponent, new()
        {
            writer.Write(lst?.Count ?? 0);
            foreach (var item in lst)
            {
                writer.Write(item == null);
                item?.Serialize(writer);
            }
        }

        public static T[] ReadArray<T>(this Deserializer reader, T[] _) where T : IComponent, new()
        {
            var count = reader.ReadInt32();
            var lst = new T[count];
            for (int i = 0; i < count; i++)
            {
                var isNull = reader.ReadBoolean();
                T item = null;
                if (!isNull)
                {
                    item = new T();
                    item.Deserialize(reader);
                }

                lst[i] = item;
            }

            return lst;
        }

        public static List<T> ReadList<T>(this Deserializer reader, IList<T> _) where T : IComponent, new()
        {
            var count = reader.ReadInt32();
            var lst = new List<T>();
            for (int i = 0; i < count; i++)
            {
                var isNull = reader.ReadBoolean();
                T item = null;
                if (!isNull)
                {
                    item = new T();
                    item.Deserialize(reader);
                }

                lst.Add(item);
            }

            return lst;
        }

        public static void DumpList(string name, IList lst, StringBuilder sb, string prefix)
        {
            sb.AppendLine(prefix + name + " Count" + ":" + lst.Count.ToString());
            sb.Append("[");
            for (int i = 0; i < lst.Count; i++)
            {
                var item = lst[i];
                sb.Append(i + ":" + item.ToString());
            }

            sb.Append("]");
        }
    }
}