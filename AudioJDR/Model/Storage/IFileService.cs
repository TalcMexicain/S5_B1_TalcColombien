using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public interface IFileService
    {
        Task ExportStoryAsync(string fileName, byte[] fileContent);
        Task<byte[]> ImportStoryAsync(); 
    }
}
