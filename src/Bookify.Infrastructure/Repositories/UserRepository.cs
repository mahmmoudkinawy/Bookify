using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Repositories;

internal sealed class UserRepository(ApplicationDbContext dbContext) : Repository<User>(dbContext), IUserRepository
{
    public override void Add(User user)
    {
        foreach (var role in user.Roles)
        {
            DbContext.Attach(role);
        }

        DbContext.Add(user);
    }
}

internal sealed class ApartmentRepository(ApplicationDbContext dbContext) : Repository<Apartment>(dbContext), IApartmentRepository { }

internal sealed class BookingRepository(ApplicationDbContext dbContext) : Repository<Booking>(dbContext), IBookingRepository
{
    private static readonly BookingStatus[] ActiveBookingStatuses = { BookingStatus.Reserved, BookingStatus.Confirmed, BookingStatus.Completed };

    public async Task<bool> IsOverlappingAsync(Apartment apartment, DateRange duration, CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<Booking>()
            .AnyAsync(
                b =>
                    b.ApartmentId == apartment.Id
                    && b.Duration.Start <= duration.End
                    && b.Duration.End >= duration.Start
                    && ActiveBookingStatuses.Contains(b.Status),
                cancellationToken
            );
    }
}
