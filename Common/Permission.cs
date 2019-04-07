using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Common
{
    // DO NOT modify existing Ids
    public class Permission : IEquatable<Permission>, IComparable<Permission>
    {
        public static readonly Permission ViewOrders = new Permission(1, nameof(ViewOrders), "View orders");

        public static readonly Permission EditOrders = new Permission(2, nameof(EditOrders), "Edit orders");

        public static readonly Permission ViewAdmin = new Permission(3, nameof(ViewAdmin), "View Admin");

        public static readonly Permission ViewAdminRoles = new Permission(4, nameof(ViewAdminRoles), "View Admin roles");

        public static readonly Permission EditAdminRoles = new Permission(5, nameof(EditAdminRoles), "Edit Admin roles");

        public long Id { get; }

        public string Name { get; }

        public string Description { get; }

        private Permission(long id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        public static Permission Parse(long id)
        {
            return GetAll().Single(f => f.Id == id);
        }

        public static bool TryParse(long id, out Permission permission)
        {
            permission = GetAll().FirstOrDefault(f => f.Id == id);
            return permission != null;
        }

        public static IReadOnlyCollection<Permission> GetAll()
        {
            var fields = typeof(Permission).GetFields(BindingFlags.DeclaredOnly | BindingFlags.Static |
                                                       BindingFlags.Public);

            return fields.Select(s => (Permission)s.GetValue(null)).ToArray();
        }

        #region Overrides

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
            return Equals((Permission) obj);
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

        public int CompareTo(Permission other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return Id.CompareTo(other.Id);
        }

        #endregion
    }
}