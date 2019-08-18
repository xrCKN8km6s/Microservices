using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BFF
{
    public abstract class Enumeration : EqualityComparer<Enumeration>, IEquatable<Enumeration>, IComparable<Enumeration>, IComparable
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

        public int CompareTo(Enumeration other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (other is null) return 1;
            return Id.CompareTo(other.Id);
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            if (ReferenceEquals(this, obj)) return 0;
            return obj is Enumeration other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(Enumeration)}");
        }
    }
}