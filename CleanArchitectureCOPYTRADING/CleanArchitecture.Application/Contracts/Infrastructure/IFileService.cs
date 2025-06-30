using CleanArchitecture.Application.Models;

namespace CleanArchitecture.Application.Contracts.Infrastructure
{
    public interface IFileService
    {
        Task<bool> DeleteAllFiles(string folderPath);
    }
}
