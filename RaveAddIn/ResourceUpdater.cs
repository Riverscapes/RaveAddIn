using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;


namespace RaveAddIn
{
    public class ResourceUpdater
    {
        public readonly string ResourceURL;
        public readonly string BusinessLogicPrefix;
        public readonly string SymbologyPrefix;

        public class downloadResults
        {
            public int downloaded = 0;
            public int skipped = 0;
            public int errors = 0;
        }

        public class UpdateResults
        {
            public downloadResults business_logic { get; set; }
            public downloadResults symbology_lyrs { get; set; }
            public downloadResults base_maps_xml { get; set; }

            public int TotalDownloads { get { return business_logic.downloaded + symbology_lyrs.downloaded + base_maps_xml.downloaded; } }
        }

        public ResourceUpdater(string resource_url, string business_logic_prefix, string symbology_prefix)
        {
            ResourceURL = resource_url;
            BusinessLogicPrefix = business_logic_prefix;
            SymbologyPrefix = symbology_prefix;
        }

        public UpdateResults Update(string targetDir)
        {
            UpdateResults results = new UpdateResults();

            // Download the RiverscapesXML repository manifest
            Directory.CreateDirectory(targetDir);
            string json_url = string.Format("{0}index.json", ResourceURL);
            string manifest_path = Path.Combine(targetDir, "index.json");
            requestDownload(json_url, manifest_path);

            // Loop over business logic files in the mainfest and download those that have a different MD5 from those on GitHub
            using (StreamReader file = File.OpenText(manifest_path))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JObject o2 = (JObject)JToken.ReadFrom(reader);
                Dictionary<string, string> mani = JsonConvert.DeserializeObject<Dictionary<string, string>>(o2.ToString());

                results.business_logic = downloadManifestFiles(ResourceURL, mani, @"RaveBusinessLogic\/.*\.xml", targetDir);
                results.symbology_lyrs = downloadManifestFiles(ResourceURL, mani, @"Symbology\/esri\/.*\.lyr", targetDir);
                results.base_maps_xml = downloadManifestFiles(ResourceURL, mani, "BaseMaps.xml", targetDir);
            }

            return results;
        }

        private downloadResults downloadManifestFiles(string resource_url, Dictionary<string, string> manifest, string regex_pattern, string target_dir)
        {
            downloadResults results = new downloadResults();

            Regex re = new Regex(regex_pattern, RegexOptions.Compiled);

            foreach (KeyValuePair<string, string> kvp in manifest)
            {
                try
                {
                    if (re.IsMatch(kvp.Key))
                    {
                        string remote_path = string.Format("{0}{1}", resource_url, kvp.Key);
                        string local_path = Path.Combine(target_dir, kvp.Key.Replace('/', Path.DirectorySeparatorChar));
                        string local_md5 = GetMD5(local_path);

                        if (local_md5 == kvp.Value)
                        {
                            results.skipped++;
                        }
                        else
                        {
                            requestDownload(remote_path, local_path);
                            results.downloaded++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    results.errors++;
                }
            }

            return results;
        }

        private void requestDownload(string remotePath, string localPath, byte[] expected_md5 = null)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(localPath));

            // http://csharpexamples.com/download-files-synchronous-asynchronous-url-c/
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(remotePath, localPath);
            }
        }


        private string GetMD5(string file_path)
        {
            if (System.IO.File.Exists(file_path))
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(file_path))
                    {
                        return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty).ToLower();
                    }
                }
            }

            return string.Empty;
        }
    }
}
