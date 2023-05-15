using SimVillage.Persistence;
using System.IO;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using SimVillage.Model.Building;
using static System.Formats.Asn1.AsnWriter;

namespace SimVillage.Model
{
    public class Persistence
    {
        private SaveStore store;
        public StoredGame[] StoredGames;
        private string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SimVillage/saves");
        public Persistence() {
            store = new SaveStore();
            StoredGames = new StoredGame[5];
        }

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

        public async Task UpdateStoredGames()
        {
            foreach (string item in await store.GetFilesAsync())
            {
                int slot = Convert.ToInt32(item.Substring(item.IndexOf("_") - 1, 1));
                string name = item.Substring(item.IndexOf("_") + 1, item.IndexOf(".") - item.IndexOf("_") - 1);
                StoredGames[slot - 1] = new StoredGame { Slot = slot, Name = name, Modified = await store.GetModifiedTimeAsync(item) };
            }

            for (int i = 0; i < 5; i++)
            {
                if (StoredGames[i] == null)
                    StoredGames[i] = new StoredGame { Slot = i + 1, Name = string.Empty };
            }
        }
    }
}
