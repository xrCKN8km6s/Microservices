namespace BFF;

public static class AuthorizePolicies
{
    public const string OrdersView = nameof(OrdersView);
    public const string OrdersEdit = nameof(OrdersEdit);

    public const string AdminView = nameof(AdminView);

    public const string AdminRolesView = nameof(AdminRolesView);
    public const string AdminRolesEdit = nameof(AdminRolesEdit);
    public const string AdminRolesDelete = nameof(AdminRolesDelete);

    public const string AdminUsersView = nameof(AdminUsersView);
    public const string AdminUsersEdit = nameof(AdminUsersEdit);
}
