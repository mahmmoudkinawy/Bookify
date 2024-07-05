using Bookify.Domain.Shared;

namespace Bookify.Domain.Bookings;

public sealed record PricingDetails(Money PriceForPeriod, Money CleeningFee, Money AmenitiesUpCharge, Money TotalPrice);