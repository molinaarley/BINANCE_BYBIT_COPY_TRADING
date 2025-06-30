using CleanArchitecture.Application.Models;

namespace CleanArchitecture.Application.Contracts.Infrastructure
{
    public interface IDonetZipService
    {
        Task<bool> ZipFolderPosition(string filepath,string destinationZip);
    }
}
