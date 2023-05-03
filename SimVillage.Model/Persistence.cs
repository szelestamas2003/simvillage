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
            try
            {
                await using FileStream createStream = File.Create(path);
                string string2 = JsonSerializer.Serialize(data);
                await JsonSerializer.SerializeAsync(createStream, data);
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
