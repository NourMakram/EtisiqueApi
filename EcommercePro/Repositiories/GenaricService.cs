using EcommercePro.Models;
using EtisiqueApi.Repositiories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EtisiqueApi.Repositiories
{
	public class GenaricService<T> : IGenaricService<T> where T : class
	{
		private Context _context;
		public GenaricService(Context context) { 
		    _context = context;
		
		}

		public async Task<(bool Succeeded, string[] Errors)> AddAsync(T Entirty)
		{
			try
			{
				await _context.Set<T>().AddAsync(Entirty);

                _context.SaveChanges();
				return (true, new string[] { });
			}
			catch (Exception ex)
			{

				return (false, new string[] {ex.Message });

			}


		}
		public async Task<IDbContextTransaction> BeginTransactionAsync()
		{
            return await this._context.Database.BeginTransactionAsync();


        }
        public async void Commit(IDbContextTransaction trans)
		{
			
			await trans.CommitAsync();
		}

        public async void Rollback(IDbContextTransaction trans)
		{
            await trans.RollbackAsync();
        }

        public (bool Succeeded, string[] Errors) Delete(T Entirty)
        {
            try
            {
                 _context.Set<T>().Remove(Entirty);
                _context.SaveChanges();
                return (true, new string[] { });
            }
            catch (Exception ex)
            {

                return (false, new string[] { ex.Message });

            }
        }

        public  virtual  async Task<T> GetByIdAsync(int id)
		{
			return await _context.Set<T>().FindAsync(id);
		}

		public IQueryable<T> GetTableAsNoTracking()
		{
			return _context.Set<T>().AsNoTracking().AsQueryable();


		}

		public IQueryable<T> GetTableAsTracking()
		{
			return _context.Set<T>().AsQueryable();
		}

		public virtual (bool Succeeded, string[] Errors)Update(T Entirty)
		{
			try
			{
				 _context.Set<T>().Update(Entirty);
				_context.SaveChanges();
				return (true, new string[] { });
			}
			catch (Exception ex)
			{

				return (false, new string[] { ex.Message });

			}
		}

        public void Save()
        {
			_context.SaveChanges();
        }

        public List<T> GetAll()
        {
			return _context.Set<T>().ToList();
        }
    }
}
