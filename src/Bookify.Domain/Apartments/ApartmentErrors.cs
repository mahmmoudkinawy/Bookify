﻿using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Apartments;
public static class ApartmentErrors
{
    public static Error NotFound = new(
        "Apartments.NotFound",
        "The Apartment with the specified identifier was not found");
}