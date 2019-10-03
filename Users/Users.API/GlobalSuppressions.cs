// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task", Justification = "ASP.NET Core doesn't have synchronization context")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "It's safe to suppress for DTOs", Scope = "namespaceanddescendants", Target = "Users.API.DTO")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1814:Prefer jagged arrays over multidimensional", Justification = "Migrations contract requirements", Scope = "namespaceanddescendants", Target = "Users.API.Migrations")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Migrations are aute-generated", Scope = "namespaceanddescendants", Target = "Users.API.Migrations")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "EF models", Scope = "namespaceanddescendants", Target = "Users.API.Infrastructure")]
