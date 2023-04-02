using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SimVillage.Persistence
{
    internal class DataAccess
    {
        public async Task<GameState> LoadAsync(string path)
        {
            try
            {
                string jsonString = File.ReadAllText(path);
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
                var result = await JsonSerializer.DeserializeAsync<GameState>(stream);
                return result == null ? throw new GameStateException() : result;
            }
            catch
            {
                throw new GameStateException();
            }
        }

        public async Task SaveAsync(string path, GameState data)
        {
            try
            {
                await using FileStream createStream = File.Create(path);
                await JsonSerializer.SerializeAsync(createStream, data);
            }
            catch
            {
                throw new GameStateException();
            }
        }
    }
}
