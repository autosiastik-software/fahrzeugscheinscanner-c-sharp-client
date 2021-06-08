using ScannerApp.DTOs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ScannerApp
{
    public class ScannerService : IScannerService
    {
        private readonly string _baseUrl;
        private readonly string _accessKey;
        private readonly string _dataEndpoint;

        public ScannerService(string accessKey)
        {
            _accessKey = accessKey;
            _baseUrl = "https://api.fahrzeugschein-scanner.de";
            _dataEndpoint = "https://www.fahrzeugschein-scanner.de";
        }

        public async Task<ScanResult> Scan(byte[] scan)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    using (var content =
                        new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
                    {
                        var imageContent = new ByteArrayContent(scan);
                        imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/*");

                        content.Add(imageContent, "file", "scan-" + DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss"));
                        content.Add(new StringContent("true", Encoding.UTF8), "show_cuts");
                        content.Add(new StringContent(_accessKey, Encoding.UTF8), "access_key");

                        using (var message = await client.PostAsync(_baseUrl, content))
                        {
                            var res = await message.Content.ReadAsStringAsync();
                            var scanResult = JsonConvert.DeserializeObject<ScanResult>(res);

                            if (scanResult == null)
                                scanResult = new ScanResult();

                            if (message.StatusCode != HttpStatusCode.OK)
                            {
                                if (message.StatusCode == HttpStatusCode.Unauthorized)
                                {
                                    scanResult.ErrorMessage = "Fehler beim scannen: Zugriff verweigert. Access-Key ungültig.";
                                }
                                else if (message.StatusCode == HttpStatusCode.Forbidden)
                                {
                                    scanResult.ErrorMessage = "Fehler beim scannen: Scan-Volumen überschritten.";
                                }
                                else
                                {
                                    scanResult.ErrorMessage = "Unerwarteter Fehler beim Digitalisieren.";
                                }

                                scanResult.Success = false;
                            }
                            else
                            {
                                scanResult.Success = true;
                            }

                            return scanResult;
                        }
                    }
                }
            }
            catch
            {
                return new ScanResult()
                {
                    Success = false,
                    ErrorMessage = "Unerwarteter Fehler beim Digitalisieren."
                };
            }
        }

        public async Task<ScanResultList> GetScans(int take, int skip = 0)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var url = $"{_dataEndpoint}/api/Scans/GetScans/{_accessKey}?take={take}";

                    if (skip != 0)
                        url += $"&skip={skip}";

                    using (var message = await client.GetAsync(url))
                    {
                        var res = await message.Content.ReadAsStringAsync();
                        var scans = JsonConvert.DeserializeObject<List<ScanViewModel>>(res);
                        var result = new ScanResultList();

                        if (scans != null)
                            result.Scans = scans;

                        if (message.StatusCode != HttpStatusCode.OK)
                        {
                            if (message.StatusCode == HttpStatusCode.NotFound)
                            {
                                result.ErrorMessage = "Fehler beim laden der gescannten Dokumente: User nicht gefunden";
                            }
                            else
                            {
                                result.ErrorMessage = "Unerwarteter Fehler beim Laden des gescannten Dokumente.";
                            }

                            result.Success = false;
                        }
                        else
                        {
                            result.Success = true;
                        }

                        return result;
                    }
                }
            }
            catch
            {
                return new ScanResultList()
                {
                    ErrorMessage = "Unerwarteter Fehler beim Laden des gescannten Dokumente."
                };
            }
        }

        public async Task<ScanResult> GetScan(Guid id)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var url = $"{_dataEndpoint}/api/Scans/ScanDetails/{_accessKey}/{id}/true";

                    using (var message = await client.GetAsync(url))
                    {
                        var res = await message.Content.ReadAsStringAsync();
                        var scan = JsonConvert.DeserializeObject<ScanResult>(res);

                        if (scan == null)
                            scan = new ScanResult();

                        if (message.StatusCode != HttpStatusCode.OK)
                        {
                            if (message.StatusCode == HttpStatusCode.NotFound)
                            {
                                scan.ErrorMessage = "Fehler beim Laden: Scan nicht gefunden";
                            }
                            else
                            {
                                scan.ErrorMessage = "Unerwarteter Fehler beim Laden des Scans.";
                            }
                            
                            scan.Success = false;
                        }
                        else
                        {
                            scan.Success = true;
                        }

                        return scan;
                    }

                }
            }
            catch
            {
                return new ScanResult()
                {
                    Success = false,
                    ErrorMessage = "Unerwarteter Fehler beim Laden des Scans."
                };
            }
        }
    }
}
