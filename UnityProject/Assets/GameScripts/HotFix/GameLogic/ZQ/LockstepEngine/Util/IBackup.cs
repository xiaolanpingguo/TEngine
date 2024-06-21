using System.Text;


namespace Lockstep.Framework
{
    public interface IBackup : IDumpStr
    {
        void WriteBackup(Serializer writer);
        void ReadBackup(Deserializer reader);
    }

    public interface IHashCode
    {
        int GetHash(ref int idx);
    }

    public interface IDumpStr
    {
        void DumpStr(StringBuilder sb, string prefix);
    }

    public interface IAfterBackup
    {
        void OnAfterDeserialize();
    }
}