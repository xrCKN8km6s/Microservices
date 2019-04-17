using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Common
{
    public abstract class Enumeration : EqualityComparer<Enumeration>, IEquatable<Enumeration>
    {
        public long Id { get; }
        public string Name { get; }
        public string Description { get; }

        protected Enumeration(long id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        public static T Parse<T>(long id) where T : Enumeration
        {
            return GetAll<T>().Single(f => f.Id == id);
        }

        public static bool TryParse<T>(long id, out T permission) where T : Enumeration
        {
            permission = GetAll<T>().FirstOrDefault(f => f.Id == id);
            return permission != null;
        }

        public static IReadOnlyCollection<T> GetAll<T>() where T : Enumeration
        {
            var fields = typeof(T).GetFields(
                BindingFlags.DeclaredOnly |
                BindingFlags.Static |
                BindingFlags.Public);

            return fields.Select(s => (T)s.GetValue(null)).ToArray();
        }

        public static implicit operator string(Enumeration permission)
        {
            return permission.Name;
        }

        public override int GetHashCode(Enumeration obj)
        {
            return obj.Id.GetHashCode();
        }

        public override bool Equals(Enumeration x, Enumeration y)
        {
            if (x is null && y is null) return true;
            if (x is null || y is null) return false;
            if (ReferenceEquals(x, y)) return true;
            return x.Id == y.Id;
        }

        public bool Equals(Enumeration other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Enumeration other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}