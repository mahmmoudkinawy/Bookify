﻿using Bookify.Application.Bookings.GetBooking;
using Bookify.Application.Bookings.ReserveBooking;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Api.Controllers.Bookings;

public static class BookingsEndpoints
{
	public static IEndpointRouteBuilder MapBookingEndpoints(this IEndpointRouteBuilder builder)
	{
		builder.MapGet("/bookings/{id}", GetBooking).RequireAuthorization().WithName(nameof(GetBooking));

		builder.MapPost("/bookings", ReserveBooking).RequireAuthorization();

		return builder;
	}

	public static async Task<IResult> GetBooking([FromServices] ISender sender, Guid id, CancellationToken cancellationToken)
	{
		var query = new GetBookingQuery(id);

		var result = await sender.Send(query, cancellationToken);

		return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound();
	}

	public static async Task<IResult> ReserveBooking(
		[FromServices] ISender sender,
		ReserveBookingRequest request,
		CancellationToken cancellationToken
	)
	{
		var command = new ReserveBookingCommand(request.ApartmentId, request.UserId, request.StartDate, request.EndDate);

		var result = await sender.Send(command, cancellationToken);

		if (result.IsFailure)
		{
			return Results.BadRequest(result.Error);
		}

		return Results.CreatedAtRoute(nameof(GetBooking), new { id = result.Value }, result.Value);
	}
}
