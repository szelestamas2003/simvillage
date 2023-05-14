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
        private string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SimVillage/saves");
        public Persistence() { }

        public async Task saveGame(StoredGame storedgame, GameState data)
        {
            string fileName = "slot" + storedgame.Slot.ToString() + "_" + data.Name + ".json";
            if (storedgame.Name != string.Empty)
            {
                File.Delete(Path.Combine(folder, "slot" + storedgame.Slot.ToString() + "_" + storedgame.Name + ".json"));
            }
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            try
            {
                Directory.CreateDirectory(folder);
                await using FileStream createStream = File.Create(Path.Combine(folder, fileName));
                await JsonSerializer.SerializeAsync(createStream, data, options);
            }
            catch
            {
                throw new GameStateException();
            }
        }

        public async Task<GameState> loadGame(StoredGame storedgame)
        {
            try
            {
                string jsonString = File.ReadAllText(Path.Combine(folder, "slot" + storedgame.Slot.ToString() + "_" + storedgame.Name + ".json"));
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
                GameState? result = await JsonSerializer.DeserializeAsync<GameState>(stream);
                return result == null ? throw new GameStateException() : result;
            }
            catch
            {
                throw new GameStateException();
            }
        }
    }
}
