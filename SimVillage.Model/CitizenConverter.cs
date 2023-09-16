using SimVillage.Model.Building;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimVillage.Model
{
    public class CitizenConverter : JsonConverter<Citizen>
    {
        private List<Citizen> readedCitizens = new List<Citizen>();

        public override Citizen? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using JsonDocument doc = JsonDocument.ParseValue(ref reader);
            JsonElement rootElement = doc.RootElement;

            int age = rootElement.GetProperty("Age").GetInt32();
            int salary = rootElement.GetProperty("Salary").GetInt32();
            EducationLevel educationLevel = (EducationLevel)rootElement.GetProperty("EducationLevel").GetInt32();
            bool pensioner = rootElement.GetProperty("Pensioner").GetBoolean();
            int happiness = rootElement.GetProperty("Happiness").GetInt32();
            int hadToMove = rootElement.GetProperty("HadToMove").GetInt32();
            Residental home = rootElement.GetProperty("Home").Deserialize<Residental>(options)!;
            Building.Building workPlace = rootElement.GetProperty("WorkPlace").Deserialize<Building.Building>(options)!;
            JsonElement.ArrayEnumerator enumerator = rootElement.GetProperty("PaidTaxes").EnumerateArray();
            List<double> paidTaxes = new List<double>();
            foreach (JsonElement element in enumerator)
            {
                paidTaxes.Add(element.GetDouble());
            }
            int pension = rootElement.GetProperty("Pension").GetInt32();

            Citizen citizen = new Citizen(age, home) { Salary = salary, EducationLevel = educationLevel, Pensioner = pensioner, Happiness = happiness, HadToMove = hadToMove, WorkPlace = workPlace, PaidTaxes = paidTaxes, Pension = pension };
            Citizen? existingCitizen;

            if ((existingCitizen = GetExistingCitizen(citizen)) != null)
                return existingCitizen;
            else
            {
                AddCitizenToList(citizen);
                return citizen;
            }
        }

        public override void Write(Utf8JsonWriter writer, Citizen value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("Age", value.Age);
            writer.WriteNumber("Salary", value.Salary);
            writer.WriteNumber("EducationLevel", (int)value.EducationLevel);
            writer.WriteBoolean("Pensioner", value.Pensioner);
            writer.WriteNumber("Happiness", value.Happiness);
            writer.WriteNumber("HadToMove", value.HadToMove);
            writer.WritePropertyName("Home");
            JsonSerializer.Serialize(writer, value.Home, options);
            writer.WritePropertyName("WorkPlace");
            JsonSerializer.Serialize(writer, value.WorkPlace, options);
            writer.WriteStartArray("PaidTaxes");
            foreach (double tax in value.PaidTaxes)
            {
                writer.WriteNumberValue(tax);
            }
            writer.WriteEndArray();
            writer.WriteNumber("Pension", value.Pension);
            writer.WriteEndObject();
        }

        private Citizen? GetExistingCitizen(Citizen citizen)
        {
            return readedCitizens.Where(c => c.Age == citizen.Age && c.Happiness == citizen.Happiness && c.EducationLevel == citizen.EducationLevel && c.Pensioner == citizen.Pensioner && c.Pension == citizen.Pension && c.HadToMove == citizen.HadToMove && c.Home == citizen.Home && c.WorkPlace == citizen.WorkPlace && c.Salary == citizen.Salary).FirstOrDefault();
        }

        private void AddCitizenToList(Citizen citizen)
        {
            readedCitizens.Add(citizen);
        }
    }
}