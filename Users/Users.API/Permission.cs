namespace Users.API
{
    // DO NOT modify existing Ids
    public class Permission : Enumeration
    {
        public static readonly Permission OrdersView = new Permission(1, nameof(OrdersView), "Orders - View Section");
        public static readonly Permission OrdersEdit = new Permission(2, nameof(OrdersEdit), "Orders - Edit");

        public static readonly Permission AdminView = new Permission(1000, nameof(AdminView), "Admin - View Section");
        public static readonly Permission AdminRolesView = new Permission(1001, nameof(AdminRolesView), "Admin - View Roles");
        public static readonly Permission AdminRolesEdit = new Permission(1002, nameof(AdminRolesEdit), "Admin - Edit Roles");
        public static readonly Permission AdminRolesDelete = new Permission(1003, nameof(AdminRolesDelete), "Admin - Delete Roles");
        public static readonly Permission AdminUsersView = new Permission(1004, nameof(AdminUsersView), "Admin - View Users");
        public static readonly Permission AdminUsersEdit = new Permission(1005, nameof(AdminUsersEdit), "Admin - Edit Users");

        private Permission(long id, string name, string description) : base(id, name, description)
        {
        }
    }
}