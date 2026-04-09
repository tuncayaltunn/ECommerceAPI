using System;
using ETicaretAPI.Application.Repositories.ICompletedOrderRepositories;
using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Persistence.Contexts;

namespace ETicaretAPI.Persistence.Repositories.CompletedOrderRepositories
{
	public class CompletedOrderWriteRepository : WriteRepository<CompletedOrder>,ICompletedOrderWriteRepository
    {
        public CompletedOrderWriteRepository(ETicaretAPIDbContext context) : base(context)
        {
        }
    }
}

