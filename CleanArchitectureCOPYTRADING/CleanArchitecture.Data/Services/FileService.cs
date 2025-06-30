using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using CleanArchitecture.Application.Contracts.Infrastructure;
using Ionic.Zip;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Infrastructure.Services
{

    public class FileService : IFileService
    {
        public ILogger<FileService> _logger { get; }
        public FileService() { }
        
        public FileService(ILogger<FileService> logger)
        {
            _logger = logger;
        }
        public async Task<bool> DeleteAllFiles(string folderPath)
        {
           var files= System.IO.Directory.GetFiles(folderPath, "*.*");

            foreach (string s in files)
            {
                if (File.Exists(s) )
                {
                    File.Delete(s);
                }
            }
            return true;
        }
    }      
}
