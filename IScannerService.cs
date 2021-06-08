using System;
using System.Threading.Tasks;
using ScannerApp.DTOs;

namespace ScannerApp
{
    public interface IScannerService
    {
        Task<ScanResult> Scan(byte[] scan);
        Task<ScanResultList> GetScans(int take, int skip = 0);
        Task<ScanResult> GetScan(Guid id);
    }
}
