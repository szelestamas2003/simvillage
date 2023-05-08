using SimVillage.Persistence;
using System.IO;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace SimVillage.Model
{
    public class Persistence
    {
        private string folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public Persistence() { }

        private string getFileName(int slot)
        {
            string fileName = "";
            switch (slot)
            {
                case 1:
                    fileName = "save1.json";
                    break;
                case 2:
                    fileName = "save2.json";
                    break;
                case 3:
                    fileName = "save3.json";
                    break;
                case 4:
                    fileName = "save4.json";
                    break;
                case 5:
                    fileName = "save5.json";
                    break;

                default:
                    break;
            }
            return fileName;
        }

        public async Task saveGame(int slot, GameState data)
        {
            folder = Path.Combine(folder, "SimVillage/saves");
            string path = getFileName(slot);
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            try
            {
                Directory.CreateDirectory(folder);
                await using FileStream createStream = File.Create(Path.Combine(folder, path));
                await JsonSerializer.SerializeAsync(createStream, data, options);
            }
            catch
            {
                throw new GameStateException();
            }
        }

        public async Task<GameState> loadGame(int slot)
        {
            folder = Path.Combine(folder, "SimVillage/saves");
            string path = getFileName(slot);
            try
            {
                string jsonString = File.ReadAllText(Path.Combine(folder, path));
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
                GameState? result = await JsonSerializer.DeserializeAsync<GameState>(stream);
                return result == null ? throw new GameStateException() : result;
            }
            catch
            {
                throw new GameStateException();
            }
        }

        public async Task deleteGame(int slot)
        {
            folder = Path.Combine(folder, "SimVillage/saves");
            string fileName = getFileName(slot);
            string uri = Path.Combine(folder, fileName);
            if (File.Exists(uri))
            {
                File.Delete(uri);
            }
            return;
        }
    }
}
