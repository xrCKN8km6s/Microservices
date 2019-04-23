﻿//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v12.2.3.0 (NJsonSchema v9.13.35.0 (Newtonsoft.Json v11.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------

namespace Users.Client.Contracts
{
    #pragma warning disable

    [System.CodeDom.Compiler.GeneratedCode("NSwag", "12.2.3.0 (NJsonSchema v9.13.35.0 (Newtonsoft.Json v11.0.0.0))")]
    public partial interface IUsersClient
    {
        /// <exception cref="ClientException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<UserProfileDto> Profile_GetUserProfileAsync(string sub);
    
        /// <exception cref="ClientException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        System.Threading.Tasks.Task<UserProfileDto> Profile_GetUserProfileAsync(string sub, System.Threading.CancellationToken cancellationToken);
    
        /// <exception cref="ClientException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<RolesViewModel> Roles_GetRolesViewModelAsync();
    
        /// <exception cref="ClientException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        System.Threading.Tasks.Task<RolesViewModel> Roles_GetRolesViewModelAsync(System.Threading.CancellationToken cancellationToken);
    
        /// <exception cref="ClientException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<RoleDto>> Roles_GetRolesAsync();
    
        /// <exception cref="ClientException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<RoleDto>> Roles_GetRolesAsync(System.Threading.CancellationToken cancellationToken);
    
        /// <exception cref="ClientException">A server side error occurred.</exception>
        System.Threading.Tasks.Task Roles_CreateRoleAsync(CreateEditRoleDto role);
    
        /// <exception cref="ClientException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        System.Threading.Tasks.Task Roles_CreateRoleAsync(CreateEditRoleDto role, System.Threading.CancellationToken cancellationToken);
    
        /// <exception cref="ClientException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<RoleDto> Roles_GetRoleByIdAsync(long id);
    
        /// <exception cref="ClientException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        System.Threading.Tasks.Task<RoleDto> Roles_GetRoleByIdAsync(long id, System.Threading.CancellationToken cancellationToken);
    
        /// <exception cref="ClientException">A server side error occurred.</exception>
        System.Threading.Tasks.Task Roles_DeleteRoleAsync(long id);
    
        /// <exception cref="ClientException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        System.Threading.Tasks.Task Roles_DeleteRoleAsync(long id, System.Threading.CancellationToken cancellationToken);
    
        /// <exception cref="ClientException">A server side error occurred.</exception>
        System.Threading.Tasks.Task Roles_UpdateRoleAsync(long id, CreateEditRoleDto role);
    
        /// <exception cref="ClientException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        System.Threading.Tasks.Task Roles_UpdateRoleAsync(long id, CreateEditRoleDto role, System.Threading.CancellationToken cancellationToken);
    
        /// <exception cref="ClientException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<UsersViewModel> Users_GetViewModelAsync();
    
        /// <exception cref="ClientException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        System.Threading.Tasks.Task<UsersViewModel> Users_GetViewModelAsync(System.Threading.CancellationToken cancellationToken);
    
        /// <exception cref="ClientException">A server side error occurred.</exception>
        System.Threading.Tasks.Task Users_UpdateUserRolesAsync(long id, UpdateUserRolesDto roles);
    
        /// <exception cref="ClientException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        System.Threading.Tasks.Task Users_UpdateUserRolesAsync(long id, UpdateUserRolesDto roles, System.Threading.CancellationToken cancellationToken);
    
    }
    
    

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "9.13.35.0 (Newtonsoft.Json v11.0.0.0)")]
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
    
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "9.13.35.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class PermissionDto 
    {
        [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public long Id { get; set; }
    
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Name { get; set; }
    
        [Newtonsoft.Json.JsonProperty("description", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Description { get; set; }
    
    
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "9.13.35.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class RolesViewModel 
    {
        [Newtonsoft.Json.JsonProperty("roles", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.IEnumerable<RoleDto> Roles { get; set; }
    
        [Newtonsoft.Json.JsonProperty("allPermissions", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.IEnumerable<PermissionDto> AllPermissions { get; set; }
    
    
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "9.13.35.0 (Newtonsoft.Json v11.0.0.0)")]
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
    
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "9.13.35.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class CreateEditRoleDto 
    {
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Name { get; set; }
    
        [Newtonsoft.Json.JsonProperty("permissions", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.IEnumerable<long> Permissions { get; set; }
    
        [Newtonsoft.Json.JsonProperty("isGlobal", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool IsGlobal { get; set; }
    
    
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "9.13.35.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class UsersViewModel 
    {
        [Newtonsoft.Json.JsonProperty("users", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.IEnumerable<UserDto> Users { get; set; }
    
        [Newtonsoft.Json.JsonProperty("roles", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.IEnumerable<RoleDto> Roles { get; set; }
    
    
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "9.13.35.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class UserDto 
    {
        [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public long Id { get; set; }
    
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Name { get; set; }
    
        [Newtonsoft.Json.JsonProperty("roles", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.IEnumerable<long> Roles { get; set; }
    
    
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "9.13.35.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class UpdateUserRolesDto 
    {
        [Newtonsoft.Json.JsonProperty("roles", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.IEnumerable<long> Roles { get; set; }
    
    
    }

    #pragma warning restore
}