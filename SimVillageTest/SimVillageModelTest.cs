using Moq;
using SimVillage.Model;
using SimVillage.Model.Building;
using System.Reflection;

namespace SimVillageTest
{
    [TestClass]
    public class SimVillageModelTest
    {
        public class SkipRemovingAttribute : Attribute
        {
        }

        public TestContext TestContext { get; set; }

        private City model = null!;

        private Mock<Persistence> mock = null!;

        [TestInitialize]
        public void Initialize()
        {
            mock = new Mock<Persistence>();

            model = new City(mock.Object);
            model.NewGame("Testing");

            bool skipremoving = GetType().GetMethod(TestContext.TestName).GetCustomAttributes<SkipRemovingAttribute>().Any();
            if (!skipremoving)
            {
                foreach (List<Zone> rows in model.Map)
                {
                    foreach (Zone zone in rows)
                    {
                        if (zone.Building?.GetType() == typeof(Forest))
                        {
                            model.DemolishZone(zone.X, zone.Y);
                        }
                    }
                }
            }
        }

        [TestMethod]
        [SkipRemoving]
        public void NewGameTest()
        {
            int forestCount = 0;
            Assert.AreEqual("Testing", model.Name);
            foreach (List<Zone> rows in model.Map)
            {
                foreach (Zone zone in rows)
                {
                    Assert.AreEqual(ZoneType.General, zone.ZoneType);
                    if (zone.Building?.GetType() == typeof(Forest))
                    {
                        forestCount++;
                        Assert.AreEqual(10, ((Forest)zone.Building).Age);
                    }
                }
            }
            Assert.IsTrue(forestCount > 0 && forestCount <= 15);
            Assert.AreEqual(5000, model.GetBudget());
            Assert.AreEqual(typeof(Road), model.Map[29][0].Building.GetType());
            Assert.AreEqual(30, model.Height());
            Assert.AreEqual(60, model.Width());
        }

        [TestMethod]
        public void CantDemolishMainRoad()
        {
            model.DemolishZone(29, 0);
            Assert.AreEqual(typeof(Road), model.Map[29][0].Building.GetType());
        }

        [TestMethod]
        public void CantBuildOnOccupiedZone()
        {
            model.NewZone(29, 0, ZoneType.Residental);
            Assert.AreEqual(ZoneType.General, model.Map[29][0].ZoneType);
            model.BuildBuilding(new PowerLine(29, 0));
            Assert.AreEqual(typeof(Road), model.Map[29][0].Building.GetType());
        }

        [TestMethod]
        public void NewStoreZone()
        {
            model.NewZone(23, 2, ZoneType.Store);
            Assert.AreEqual(ZoneType.Store, model.Map[23][2].ZoneType);
            Assert.AreEqual(typeof(Store), model.Map[23][2].Building.GetType());
            Assert.AreEqual(0, ((Store)model.Map[23][2].Building).Workers);
        }

        [TestMethod]
        public void NewIndustrialZone()
        {
            model.NewZone(23, 2, ZoneType.Industrial);
            Assert.AreEqual(ZoneType.Industrial, model.Map[23][2].ZoneType);
            Assert.AreEqual(typeof(Industrial), model.Map[23][2].Building.GetType());
            Assert.AreEqual(0, ((Industrial)model.Map[23][2].Building).Workers);
        }

        [TestMethod]
        public void NewResidentalZone()
        {
            model.NewZone(23, 2, ZoneType.Residental);
            Assert.AreEqual(ZoneType.Residental, model.Map[23][2].ZoneType);
            Assert.AreEqual(null, model.Map[23][2].Building);
        }

        [TestMethod]
        public void BuildingBuildings()
        {
            model.BuildBuilding(new Forest(23, 2));
            Assert.AreEqual(typeof(Forest), model.Map[23][2].Building.GetType());
            model.BuildBuilding(new FireDepartment(23, 3));
            Assert.AreEqual(typeof(FireDepartment), model.Map[23][3].Building.GetType());
            model.BuildBuilding(new PoliceDepartment(23, 4));
            Assert.AreEqual(typeof(PoliceDepartment), model.Map[23][4].Building.GetType());
            model.BuildBuilding(new PowerLine(23, 5));
            Assert.AreEqual(typeof(PowerLine), model.Map[23][5].Building.GetType());
            model.BuildBuilding(new Road(23, 6));
            Assert.AreEqual(typeof(Road), model.Map[23][6].Building.GetType());
            model.BuildBuilding(new PowerPlant(24, 0));
            Assert.AreEqual(typeof(PowerPlant), model.Map[24][0].Building.GetType());
            Assert.AreEqual(typeof(PowerPlant), model.Map[24][1].Building.GetType());
            Assert.AreEqual(typeof(PowerPlant), model.Map[25][0].Building.GetType());
            Assert.AreEqual(typeof(PowerPlant), model.Map[25][1].Building.GetType());
            model.BuildBuilding(new School(0, 0, SchoolTypes.Elementary));
            Assert.AreEqual(typeof(School), model.Map[0][0].Building.GetType());
            Assert.AreEqual(typeof(School), model.Map[0][1].Building.GetType());
            model.BuildBuilding(new School(1, 0, SchoolTypes.University));
            Assert.AreEqual(typeof(School), model.Map[1][0].Building.GetType());
            Assert.AreEqual(typeof(School), model.Map[1][1].Building.GetType());
            Assert.AreEqual(typeof(School), model.Map[2][0].Building.GetType());
            Assert.AreEqual(typeof(School), model.Map[2][1].Building.GetType());
            model.BuildBuilding(new Stadium(0, 34));
            Assert.AreEqual(typeof(Stadium), model.Map[0][34].Building.GetType());
            Assert.AreEqual(typeof(Stadium), model.Map[0][35].Building.GetType());
            Assert.AreEqual(typeof(Stadium), model.Map[1][34].Building.GetType());
            Assert.AreEqual(typeof(Stadium), model.Map[1][35].Building.GetType());
        }

        [TestMethod]
        public void UpgradingZone()
        {
            model.NewZone(0, 0, ZoneType.Store);
            Assert.AreEqual(10, ((Store)model.Map[0][0].Building).MaxWorkers);
            Assert.AreEqual(1, model.Map[0][0].Building.Density);
            model.UpgradeZone(0, 0);
            Assert.AreEqual(22, ((Store)model.Map[0][0].Building).MaxWorkers);
            model.UpgradeZone(0, 0);
            Assert.AreEqual(46, ((Store)model.Map[0][0].Building).MaxWorkers);
        }
    }
}