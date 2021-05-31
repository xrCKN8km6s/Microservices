namespace Users.API
{
    // DO NOT modify existing Ids
    public class Permission : Enumeration
    {
        public static readonly Permission OrdersView = new(1, nameof(OrdersView), "Orders - View Section");
        public static readonly Permission OrdersEdit = new(2, nameof(OrdersEdit), "Orders - Edit");

        public static readonly Permission AdminView = new(1000, nameof(AdminView), "Admin - View Section");
        public static readonly Permission AdminRolesView = new(1001, nameof(AdminRolesView), "Admin - View Roles");
        public static readonly Permission AdminRolesEdit = new(1002, nameof(AdminRolesEdit), "Admin - Edit Roles");
        public static readonly Permission AdminRolesDelete = new(1003, nameof(AdminRolesDelete), "Admin - Delete Roles");
        public static readonly Permission AdminUsersView = new(1004, nameof(AdminUsersView), "Admin - View Users");
        public static readonly Permission AdminUsersEdit = new(1005, nameof(AdminUsersEdit), "Admin - Edit Users");

        private Permission(long id, string name, string description) : base(id, name, description)
        {
        }
    }
}
