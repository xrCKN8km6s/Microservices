using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Users.API.Infrastructure;
using Users.API.Queries;
using Xunit;

namespace Users.UnitTests
{
    public class UserQueriesTests
    {
        [Fact]
        public async Task UserQueries_GetRolesViewModelAsync_ReturnEmptyRolesWhenNoRolesInDB()
        {
            var options = new DbContextOptionsBuilder<UsersContext>()
                .UseInMemoryDatabase(
                    databaseName: "UserQueriesTests_GetRolesViewModelAsync_ReturnEmptyRolesWhenNoRolesInDB")
                .Options;

            await using var ctx = new UsersContext(options);

            var sut = new UsersQueries(ctx);

            var result = await sut.GetRolesViewModelAsync();

            Assert.Empty(result.Roles);
        }

        [Fact]
        public async Task UserQueries_GetRolesViewModelAsync_IgnoresRolesWithIsActiveFalse()
        {
            var options = new DbContextOptionsBuilder<UsersContext>()
                .UseInMemoryDatabase(
                    databaseName: "UserQueriesTests_GetRolesViewModelAsync_IgnoresRolesWithIsActiveFalse")
                .Options;

            await using var ctx = new UsersContext(options);

            ctx.Roles.Add(new Role {Name = "role1", IsActive = false});
            ctx.SaveChanges();

            var sut = new UsersQueries(ctx);

            var result = await sut.GetRolesViewModelAsync();

            Assert.Empty(result.Roles);
        }
    }
}
