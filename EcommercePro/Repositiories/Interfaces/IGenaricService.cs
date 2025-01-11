using Microsoft.EntityFrameworkCore.Storage;

namespace EtisiqueApi.Repositiories.Interfaces
{
	public interface IGenaricService<T> where T : class
	{
		public Task<(bool Succeeded,string[] Errors)>AddAsync(T Entirty);
		public (bool Succeeded, string[] Errors)Update(T Entirty);
		public (bool Succeeded, string[] Errors )Delete(T Entirty);	
		public Task<T> GetByIdAsync(int id);
		public IQueryable<T> GetTableAsTracking();
		public IQueryable<T> GetTableAsNoTracking();
		public Task<IDbContextTransaction> BeginTransactionAsync();
		public void Commit(IDbContextTransaction trans);
		public void Rollback(IDbContextTransaction trans);
		public void Save();

		public List<T> GetAll();
			 







    }
}
