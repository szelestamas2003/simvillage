using Moq;
using SimVillage.Model;
using SimVillage.Model.Building;

namespace SimVillageTest
{
    [TestClass]
    public class SimVillageModelTest
    {
        private SimVillage.Model.City model = null!;

        private Mock<Persistence> mock = null!;

        [TestInitialize]
        public void Initialize()
        {
            mock = new Mock<Persistence>();

            model = new City(mock.Object);
            model.NewGame("Testing");
        }

        [TestMethod]
        public void NewGameTest()
        {
            Assert.AreEqual("Testing", model.Name);
            foreach (List<Zone> rows in model.Map)
            {
                foreach (Zone zone in rows)
                {
                    Assert.AreEqual(ZoneType.General, zone.ZoneType);
                }
            }
            Assert.AreEqual(5000, model.GetBudget());
            Assert.AreEqual(typeof(Road), model.Map[29][0].Building.GetType());
            Assert.AreEqual(30, model.Height());
            Assert.AreEqual(60, model.Width());
        }

        [TestMethod]
        public void CantDemolishMainRoad()
        {
            model.demolishZone(29, 0);
            Assert.AreEqual(typeof(Road), model.Map[29][0].Building.GetType());
        }

        [TestMethod]
        public void CantBuildOnOccupiedZone()
        {
            model.newZone(29, 0, ZoneType.Residental);
            Assert.AreEqual(ZoneType.General, model.Map[29][0].ZoneType);
            model.BuildBuilding(new PowerLine(29, 0));
            Assert.AreEqual(typeof(Road), model.Map[29][0].Building.GetType());
        }
    }
}