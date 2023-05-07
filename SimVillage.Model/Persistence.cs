using SimVillage.Persistence;
using System.IO;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;


namespace SimVillage.Model
{
    public class Persistence
    {
        public Persistence() { }

        public async Task saveGame(string path, GameState data)
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            folder = Path.Combine(folder, "SimVillage/saves");
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

        public async Task<GameState> loadGame(string path)
        {
            try
            {
                string jsonString = File.ReadAllText(path);
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
