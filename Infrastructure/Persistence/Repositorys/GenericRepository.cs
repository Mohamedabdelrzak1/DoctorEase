using Domain.Contracts;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using System.Linq.Expressions;

namespace Persistence.Repositorys
{
    public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey> 
        where TEntity : BaseEntity<TKey>
    {
        private readonly DoctorEaseDbContext _context;

        public GenericRepository(DoctorEaseDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(bool trackChanges = false)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            // ✅ Include المريض تلقائي عند استدعاء Bookings
            if (typeof(TEntity) == typeof(Booking))
            {
                var bookingsQuery = _context.Bookings.Include(b => b.Patient);
                query = bookingsQuery as IQueryable<TEntity>;
            }
            // ✅ Include المريض تلقائي عند استدعاء Testimonials
            else if (typeof(TEntity) == typeof(Testimonial))
            {
                var testimonialsQuery = _context.Testimonials.Include(t => t.Patient);
                query = testimonialsQuery as IQueryable<TEntity>;
            }

            return trackChanges
                ? await query.ToListAsync()
                : await query.AsNoTracking().ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(TKey id)
        {
            if (typeof(TEntity) == typeof(Booking))
            {
                var booking = await _context.Bookings
                    .Include(b => b.Patient)
                    .FirstOrDefaultAsync(b => b.Id!.Equals(id));
                return booking as TEntity;
            }
            else if (typeof(TEntity) == typeof(Testimonial))
            {
                var testimonial = await _context.Testimonials
                    .Include(t => t.Patient)
                    .FirstOrDefaultAsync(t => t.Id!.Equals(id));
                return testimonial as TEntity;
            }

            return await _context.Set<TEntity>().FindAsync(id);
        }

        public async Task AddAsync(TEntity entity) => await _context.Set<TEntity>().AddAsync(entity);

        public void Update(TEntity entity) => _context.Set<TEntity>().Update(entity);

        public void Delete(TEntity entity) => _context.Set<TEntity>().Remove(entity);

       
    }
}
