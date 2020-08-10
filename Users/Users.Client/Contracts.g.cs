//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v13.7.0.0 (NJsonSchema v10.1.24.0 (Newtonsoft.Json v12.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------

using Clients.Common;

#pragma warning disable 108 // Disable "CS0108 '{derivedDto}.ToJson()' hides inherited member '{dtoBase}.ToJson()'. Use the new keyword if hiding was intended."
#pragma warning disable 114 // Disable "CS0114 '{derivedDto}.RaisePropertyChanged(String)' hides inherited member 'dtoBase.RaisePropertyChanged(String)'. To make the current member override that implementation, add the override keyword. Otherwise add the new keyword."
#pragma warning disable 472 // Disable "CS0472 The result of the expression is always 'false' since a value of type 'Int32' is never equal to 'null' of type 'Int32?'
#pragma warning disable 1573 // Disable "CS1573 Parameter '...' has no matching param tag in the XML comment for ...
#pragma warning disable 1591 // Disable "CS1591 Missing XML comment for publicly visible type or member ..."

namespace Users.Client.Contracts
{
    using System = global::System;
    
    [System.CodeDom.Compiler.GeneratedCode("NSwag", "13.7.0.0 (NJsonSchema v10.1.24.0 (Newtonsoft.Json v12.0.0.0))")]
    public partial interface IUsersClient
    {
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<UserProfileDto> Profile_GetUserProfileAsync(string sub);
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<UserProfileDto> Profile_GetUserProfileAsync(string sub, System.Threading.CancellationToken cancellationToken);
    
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<RolesViewModel> Roles_GetRolesViewModelAsync();
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<RolesViewModel> Roles_GetRolesViewModelAsync(System.Threading.CancellationToken cancellationToken);
    
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<RoleDto>> Roles_GetRolesAsync();
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<RoleDto>> Roles_GetRolesAsync(System.Threading.CancellationToken cancellationToken);
    
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task Roles_CreateRoleAsync(CreateEditRoleDto role);
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task Roles_CreateRoleAsync(CreateEditRoleDto role, System.Threading.CancellationToken cancellationToken);
    
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<RoleDto> Roles_GetRoleByIdAsync(long id);
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<RoleDto> Roles_GetRoleByIdAsync(long id, System.Threading.CancellationToken cancellationToken);
    
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task Roles_DeleteRoleAsync(long id);
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task Roles_DeleteRoleAsync(long id, System.Threading.CancellationToken cancellationToken);
    
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task Roles_UpdateRoleAsync(long id, CreateEditRoleDto role);
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task Roles_UpdateRoleAsync(long id, CreateEditRoleDto role, System.Threading.CancellationToken cancellationToken);
    
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<UsersViewModel> Users_GetViewModelAsync();
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<UsersViewModel> Users_GetViewModelAsync(System.Threading.CancellationToken cancellationToken);
    
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task Users_UpdateUserRolesAsync(long id, UpdateUserRolesDto roles);
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task Users_UpdateUserRolesAsync(long id, UpdateUserRolesDto roles, System.Threading.CancellationToken cancellationToken);
    
    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.1.24.0 (Newtonsoft.Json v12.0.0.0)")]
    public partial class UserProfileDto 
    {
        [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public long Id { get; set; }
    
        [Newtonsoft.Json.JsonProperty("sub", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Sub { get; set; }
    
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Name { get; set; }
    
        [Newtonsoft.Json.JsonProperty("email", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Email { get; set; }
    
        [Newtonsoft.Json.JsonProperty("hasGlobalRole", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool HasGlobalRole { get; set; }
    
        [Newtonsoft.Json.JsonProperty("permissions", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.IEnumerable<PermissionDto> Permissions { get; set; }
    
    
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.1.24.0 (Newtonsoft.Json v12.0.0.0)")]
    public partial class PermissionDto 
    {
        [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public long Id { get; set; }
    
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Name { get; set; }
    
        [Newtonsoft.Json.JsonProperty("description", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Description { get; set; }
    
    
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.1.24.0 (Newtonsoft.Json v12.0.0.0)")]
    public partial class RolesViewModel 
    {
        [Newtonsoft.Json.JsonProperty("roles", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.IEnumerable<RoleDto> Roles { get; set; }
    
        [Newtonsoft.Json.JsonProperty("allPermissions", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.IEnumerable<PermissionDto> AllPermissions { get; set; }
    
    
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.1.24.0 (Newtonsoft.Json v12.0.0.0)")]
    public partial class RoleDto 
    {
        [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public long Id { get; set; }
    
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Name { get; set; }
    
        [Newtonsoft.Json.JsonProperty("permissions", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.IEnumerable<long> Permissions { get; set; }
    
        [Newtonsoft.Json.JsonProperty("isGlobal", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool IsGlobal { get; set; }
    
    
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.1.24.0 (Newtonsoft.Json v12.0.0.0)")]
    public partial class CreateEditRoleDto 
    {
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Name { get; set; }
    
        [Newtonsoft.Json.JsonProperty("permissions", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.IEnumerable<long> Permissions { get; set; }
    
        [Newtonsoft.Json.JsonProperty("isGlobal", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool IsGlobal { get; set; }
    
    
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.1.24.0 (Newtonsoft.Json v12.0.0.0)")]
    public partial class UsersViewModel 
    {
        [Newtonsoft.Json.JsonProperty("users", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.IEnumerable<UserDto> Users { get; set; }
    
        [Newtonsoft.Json.JsonProperty("roles", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.IEnumerable<RoleDto> Roles { get; set; }
    
    
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.1.24.0 (Newtonsoft.Json v12.0.0.0)")]
    public partial class UserDto 
    {
        [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public long Id { get; set; }
    
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Name { get; set; }
    
        [Newtonsoft.Json.JsonProperty("roles", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.IEnumerable<long> Roles { get; set; }
    
    
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.1.24.0 (Newtonsoft.Json v12.0.0.0)")]
    public partial class UpdateUserRolesDto 
    {
        [Newtonsoft.Json.JsonProperty("roles", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.IEnumerable<long> Roles { get; set; }
    
    
    }

}

#pragma warning restore 1591
#pragma warning restore 1573
#pragma warning restore  472
#pragma warning restore  114
#pragma warning restore  108