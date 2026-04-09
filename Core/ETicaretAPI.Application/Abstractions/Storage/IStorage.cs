using System;
using Microsoft.AspNetCore.Http;

namespace ETicaretAPI.Application.Abstractions.Storage
{
	public interface IStorage
	{
		Task<List<(string fileName, string pathOrContainerName)>> UploadAsync(
			string pathOrContainerName, IFormFileCollection files);
		Task DeleteAsync(string pathOrContainer, string fileName);
		List<string> GetFiles(string pathOrContainer);
		bool HasFile(string pathOrContainerName, string fileName);
	}
}

