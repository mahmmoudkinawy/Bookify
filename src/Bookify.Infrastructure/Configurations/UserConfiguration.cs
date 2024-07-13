using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder.ToTable("users");

		builder.HasKey(u => u.Id);

		builder.Property(u => u.FirstName).HasMaxLength(200).HasConversion(u => u.Value, firstName => new FirstName(firstName));

		builder.Property(u => u.LastName).HasMaxLength(200).HasConversion(u => u.Value, lastName => new LastName(lastName));

		builder.Property(u => u.Email).HasMaxLength(400).HasConversion(u => u.Value, email => new Domain.Users.Email(email));

		builder.HasIndex(u => u.Email).IsUnique();

		builder.HasIndex(u => u.IdentityId).IsUnique();
	}
}
