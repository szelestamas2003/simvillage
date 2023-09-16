using SimVillage.Model.Building;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SimVillage.Model
{
    public class BuildingConverter : JsonConverter<Building.Building>
    {
        private Dictionary<string, List<Building.Building>> readedBuildings = new Dictionary<string, List<Building.Building>>();

        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(Building.Building) || typeToConvert.IsSubclassOf(typeof(Building.Building));
        }

        public override Building.Building? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using JsonDocument doc = JsonDocument.ParseValue(ref reader);
            JsonElement rootElement = doc.RootElement;
            string? typeDiscriminator = rootElement.GetProperty("TypeDiscriminator").GetString();
            if (string.IsNullOrEmpty(typeDiscriminator))
                throw new JsonException();
            int x = rootElement.GetProperty("X").GetInt32();
            int y = rootElement.GetProperty("Y").GetInt32();

            Building.Building? existingBuilding = GetExistingBuilding(x, y, typeDiscriminator);

            if (existingBuilding != null)
                return existingBuilding;

            bool isOnFire = rootElement.GetProperty("IsOnFire").GetBoolean();
            int density = rootElement.GetProperty("Density").GetInt32();
            bool occupied = rootElement.GetProperty("Occupied").GetBoolean();
            bool isPowered = rootElement.GetProperty("IsPowered").GetBoolean();
            bool isAccessible = rootElement.GetProperty("IsAccessible").GetBoolean();
            int health = rootElement.GetProperty("Health").GetInt32();

            Building.Building building;

            switch (typeDiscriminator)
            {
                case "FireDepartment":
                    bool unitAvaible = rootElement.GetProperty("UnitAvailable").GetBoolean();
                    building = new FireDepartment(x, y) { UnitAvailable = unitAvaible, IsOnFire = isOnFire, Density = density, Occupied = occupied, IsPowered = isPowered, IsAccessible = isAccessible, Health = health };
                    break;
                case "Forest":
                    int age = rootElement.GetProperty("Age").GetInt32();
                    building = new Forest(x, y) { Age = age, IsOnFire = isOnFire, Density = density, Occupied = occupied, IsPowered = isPowered, IsAccessible = isAccessible, Health = health };
                    break;
                case "Industrial":
                    int maxWorkers = rootElement.GetProperty("MaxWorkers").GetInt32();
                    int workers = rootElement.GetProperty("Workers").GetInt32();
                    building = new Industrial(x, y) { MaxWorkers = maxWorkers, Workers = workers, IsOnFire = isOnFire, Density = density, Occupied = occupied, IsPowered = isPowered, IsAccessible = isAccessible, Health = health };
                    break;
                case "Residental":
                    int maxInhabitants = rootElement.GetProperty("MaxInhabitants").GetInt32();
                    int inhabitants = rootElement.GetProperty("Inhabitants").GetInt32();
                    building = new Residental(x, y) { MaxInhabitants = maxInhabitants, Inhabitants = inhabitants, IsOnFire = isOnFire, Density = density, Occupied = occupied, IsPowered = isPowered, IsAccessible = isAccessible, Health = health };
                    break;
                case "School":
                    int type = rootElement.GetProperty("Type").GetInt32();
                    List<Citizen> citizens = new List<Citizen>();
                    JsonElement.ArrayEnumerator enumerator = rootElement.GetProperty("Students").EnumerateArray();
                    foreach (JsonElement element in enumerator)
                    {
                        citizens.Add(element.Deserialize<Citizen>(options)!);
                    }
                    building = new School(x, y, (SchoolTypes)type) { Students = citizens, IsOnFire = isOnFire, Density = density, Occupied = occupied, IsPowered = isPowered, IsAccessible = isAccessible, Health = health };
                    break;
                case "Store":
                    maxWorkers = rootElement.GetProperty("MaxWorkers").GetInt32();
                    workers = rootElement.GetProperty("Workers").GetInt32();
                    building = new Store(x, y) { MaxWorkers = maxWorkers, Workers = workers, IsOnFire = isOnFire, Density = density, Occupied = occupied, IsPowered = isPowered, IsAccessible = isAccessible, Health = health };
                    break;
                default:
                    Type types = typeof(Building.Building).Assembly.GetType("SimVillage.Model.Building." + typeDiscriminator)!;
                    building = (Building.Building)types.GetConstructor(new[] { typeof(int), typeof(int) })!.Invoke(new object?[] { x, y });
                    building.Occupied = occupied;
                    building.Density = density;
                    building.IsOnFire = isOnFire;
                    building.IsPowered = isPowered;
                    building.IsAccessible = isAccessible;
                    building.Health = health;
                    break;
            }

            AddBuildingToExistingList(building, typeDiscriminator);
            return building;
        }

        public override void Write(Utf8JsonWriter writer, Building.Building value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("TypeDiscriminator", value.GetType().Name);
            writer.WriteNumber("X", value.X);
            writer.WriteNumber("Y", value.Y);
            writer.WriteBoolean("IsOnFire", value.IsOnFire);
            writer.WriteNumber("Density", value.Density);
            writer.WriteBoolean("Occupied", value.Occupied);
            writer.WriteBoolean("IsPowered", value.IsPowered);
            writer.WriteBoolean("IsAccessible", value.IsAccessible);
            writer.WriteNumber("Health", value.Health);
            switch (value)
            {
                case FireDepartment:
                    writer.WriteBoolean("UnitAvailable", ((FireDepartment)value).UnitAvailable);
                    break;
                case Forest:
                    writer.WriteNumber("Age", ((Forest)value).Age);
                    break;
                case Industrial ind:
                    writer.WriteNumber("MaxWorkers", ind.MaxWorkers);
                    writer.WriteNumber("Workers", ind.Workers);
                    break;
                case Residental res:
                    writer.WriteNumber("MaxInhabitants", res.MaxInhabitants);
                    writer.WriteNumber("Inhabitants", res.Inhabitants);
                    break;
                case School sch:
                    writer.WriteNumber("Type", (int)sch.Type);
                    writer.WriteStartArray("Students");
                    foreach (Citizen citizen in sch.Students)
                    {
                        JsonSerializer.Serialize(writer, citizen, options);
                    }
                    writer.WriteEndArray();
                    break;
                case Store store:
                    writer.WriteNumber("MaxWorkers", store.MaxWorkers);
                    writer.WriteNumber("Workers", store.Workers);
                    break;
            }
            writer.WriteEndObject();
        }

        private Building.Building? GetExistingBuilding(int x, int y, string typeDiscriminator)
        {
            if (readedBuildings.ContainsKey(typeDiscriminator))
                return readedBuildings[typeDiscriminator].Where(b => b.X == x && b.Y == y && b.GetType().Name == typeDiscriminator).FirstOrDefault();
            else
                return null;
        }

        private void AddBuildingToExistingList(Building.Building building, string typeDiscriminator)
        {
            if (readedBuildings.ContainsKey(typeDiscriminator))
                readedBuildings[typeDiscriminator].Add(building);
            else
                readedBuildings.Add(typeDiscriminator, new List<Building.Building>() { building });
        }
    }
}
