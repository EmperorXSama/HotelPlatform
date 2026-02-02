using System.Reflection;
using HotelPlatform.Application.Common.Interfaces;
using HotelPlatform.Domain.Files;
using HotelPlatform.Domain.Hotels;
using HotelPlatform.Domain.Hotels.Entities;
using HotelPlatform.Domain.Ratings;
using HotelPlatform.Domain.ReferenceData;
using HotelPlatform.Domain.Users;
using HotelPlatform.Infrastructure.Data.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace HotelPlatform.Infrastructure.Data;

public class ApplicationDbContext : DbContext ,IUnitOfWork
{
    private IDbContextTransaction? _currentTransaction;
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    public DbSet<Hotel> Hotels => Set<Hotel>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Rating> Ratings => Set<Rating>();
    public DbSet<StoredFile> StoredFiles => Set<StoredFile>();
    
    
    // Entities (exposed for querying, but managed through aggregates)
    public DbSet<Room> Rooms => Set<Room>();
    
    // Reference Data
    public DbSet<HotelAmenityDefinition> HotelAmenityDefinitions => Set<HotelAmenityDefinition>();
    public DbSet<RoomAmenityDefinition> RoomAmenityDefinitions => Set<RoomAmenityDefinition>();
    public DbSet<RoomTypeDefinition> RoomTypeDefinitions => Set<RoomTypeDefinition>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        SeedReferenceData.SeedData(builder);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            if (_currentTransaction is not null)
            {
                await _currentTransaction.CommitAsync(cancellationToken);
            }
        }
        catch (Exception e)
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is not null)
            throw new InvalidOperationException("A transaction is already in progress.");

        _currentTransaction = await Database.BeginTransactionAsync(cancellationToken);
    }
    public async  Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is not null)
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
            _currentTransaction.Dispose();
            _currentTransaction = null;
        }
    }
}