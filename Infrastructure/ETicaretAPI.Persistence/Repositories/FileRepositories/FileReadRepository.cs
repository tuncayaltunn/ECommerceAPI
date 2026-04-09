using System;
using ETicaretAPI.Application.Repositories.IFileRepositories;
using ETicaretAPI.Persistence.Contexts;

namespace ETicaretAPI.Persistence.Repositories.FileRepositories
{
    public class FileReadRepository : ReadRepository<Domain.Entities.File>,
        IFileReadRepository
    {
        public FileReadRepository(ETicaretAPIDbContext context) : base(context)
        {
        }
    }
}

