using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Users.DTO
{
    public class RoleDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long[] Permissions { get; set; }
        public bool IsGlobal { get; set; }
    }

    public class CreateEditRoleDto : IValidatableObject
    {
        public string Name { get; set; }
        public long[] Permissions { get; set; }
        public bool IsGlobal { get; set; }

        //after ASP.Core 3 is released it will be possible to apply camel case to dictionary keys (use nameof(IsGlobal) instead of "isGlobal")
        //https://github.com/aspnet/AspNetCore/issues/7439#issuecomment-462399829
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                yield return new ValidationResult("Name is required.", new[] {"name"});
            }

            if (IsGlobal && Permissions.Length > 0)
            {
                yield return new ValidationResult("Global role can't have permissions.", new[] { "isGlobal", "permissions" });
            }
        }
    }
}