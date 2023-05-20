using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimVillage.Model
{
    public class SaveStore
    {
        public async Task<IEnumerable<string>> GetFilesAsync()
        {
            return await Task.Run(() => Directory.GetFiles(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SimVillage/saves")).Select(Path.GetFileName).Where(name => name!.StartsWith("slot") && name!.EndsWith(".json")).OfType<string>());
        }

        public async Task<DateTime> GetModifiedTimeAsync(string name)
        {
            FileInfo fileInfo = new FileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SimVillage/saves", name));

            return await Task.Run(() => fileInfo.LastWriteTime);
        }
    }
}