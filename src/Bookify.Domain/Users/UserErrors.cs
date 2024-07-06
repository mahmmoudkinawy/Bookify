using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Users;

public static class UserErrors
{
    public static Error NotFound = new(
        "Users.NotFound",
        "The User with the specified identifier was not found");
}