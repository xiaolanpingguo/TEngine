using System;

namespace Lockstep.Framework
{
    public interface INeedBackup { }
    public interface IEntity : INeedBackup { }
    public interface IComponent : INeedBackup { }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false,
        Inherited = true)]
    public class NoBackupAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false,
        Inherited = true)]
    public class BackupAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false,
        Inherited = true)]
    public class ReRefBackupAttribute : Attribute { }
}