using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KuduSync.NET
{
    public class DeploymentManifest
    {
        private List<string> _paths;

        public DeploymentManifest(string path)
        {
            ManifestFilePath = path;
            _paths = new List<string>();
        }

        public static DeploymentManifest LoadManifestFile(string path)
        {
            var deploymentManifest = new DeploymentManifest(path);

            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                deploymentManifest._paths = File.ReadAllLines(path).ToList();
            }

            return deploymentManifest;
        }

        public void SaveManifestFile()
        {
            if (!string.IsNullOrWhiteSpace(ManifestFilePath))
            {
                File.WriteAllLines(ManifestFilePath, _paths);
            }
        }

        public string ManifestFilePath { get; }

        public void AddPath(string rootPath, string path, string targetSubFolder)
        {
            string relativePath = FileSystemHelpers.GetRelativePath(rootPath, path);
            relativePath = string.IsNullOrEmpty(targetSubFolder)
                ? relativePath
                : Path.Combine(targetSubFolder, relativePath);
            _paths.Add(relativePath);
        }

        public IEnumerable<string> Paths => _paths;
    }
}
