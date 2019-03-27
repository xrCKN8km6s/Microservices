using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Common
{
    //TODO: Refactor to extract base class
    public class Permission : IEquatable<Permission>
    {
        public long Id { get; private set; }

        public string Description { get; private set; }

        private Permission(long id, string description)
        {
            Id = id;
            Description = description;
        }

        public static Permission ViewOrders = new Permission(1, "View orders");

        public static Permission EditOrders = new Permission(2, "Edit orders");

        public bool Equals(Permission other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Permission)) return false;
            return Equals((Permission)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(Permission left, Permission right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Permission left, Permission right)
        {
            return !Equals(left, right);
        }

        public static Permission Parse(long id)
        {
            return GetAll().Single(f => f.Id == id);
        }

        public static IReadOnlyCollection<Permission> GetAll()
        {
            var fields = typeof(Permission).GetFields(BindingFlags.DeclaredOnly | BindingFlags.Static |
                                                       BindingFlags.Public);

            return fields.Select(s => (Permission)s.GetValue(null)).ToArray();
        }
    }
}