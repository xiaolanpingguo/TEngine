using System;

namespace Lockstep.Framework
{
    public interface IEvent { }
    public interface IGlobal { }
    public interface IAsset { }
    public interface IEntity : INeedBackup { }
    public interface IContexts { }
    public interface IComponent : INeedBackup { }
    public interface INeedBackup { }

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