using System;

namespace Postgrest.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TableAttribute : Attribute
    {
        public string Name { get; }

        public TableAttribute(string name) => Name = name;
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ColumnAttribute : Attribute
    {
        public string Name { get; }
        public ColumnAttribute(string name) => Name = name;
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PrimaryKeyAttribute : Attribute
    {
        public string Name { get; }
        public bool AutoIncrement { get; }

        public PrimaryKeyAttribute(string name, bool autoIncrement = true)
        {
            Name = name;
            AutoIncrement = autoIncrement;
        }
    }
}