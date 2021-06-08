using System.Collections.Generic;

namespace ScannerApp.DTOs
{
    public class ScanResultList
    {
        public ScanResultList()
        {
            Scans = new List<ScanViewModel>();
        }

        public List<ScanViewModel> Scans { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}
