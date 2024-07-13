using Bookify.Domain.Users;

namespace Bookify.Application.Abstractions.Authentication;

public interface IAuthenticationService
{
	Task<string> RegisterAsync(User user, string Password, CancellationToken cancellationToken = default);
}
