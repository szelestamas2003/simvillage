using SimVillage.Model.Building;
using SimVillage.Persistence;

namespace SimVillage.Model
{
    public class City
    {
        private const int mapWidth = 60;

        private const int mapHeight = 30;

        private int peopleAtStart = 30;

        private bool canDemolish = false;

        private string cityName;

        private DateTime date = new DateTime(2000, 1, 1);

        private Persistence dataAccess;

        private Finances Finances;

        private static Zone[,] map = null!;

        public int Width() { return mapWidth;}

        public int Height() { return mapHeight;}

        public string Name { get { return cityName; } }

        public DateTime Date { get { return date; } }

        public Zone[,] Map { get { return map; } }

        private List<Citizen> citizens = null!;

        private List<Industrial> availableIndustrials = null!;

        private List<Store> availableStores = null!;

        private List<Residental> availableHouses = null!;

        private List<School> avaibleSchools = null!;

        public EventHandler? gameAdvanced;

        public EventHandler? gameCreated;

        public EventHandler? gameChanged;

        public EventHandler? ConflictDemolish;

        public List<Citizen> Citizens { get { return citizens; } }

        public City(Persistence dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public void newGame(string name)
        {
            cityName = name;
            Finances = new Finances(5000);
            citizens = new List<Citizen>();
            availableStores = new List<Store>();
            availableIndustrials = new List<Industrial>();
            availableHouses = new List<Residental>();
            avaibleSchools = new List<School>();


            map = new Zone[mapHeight, mapWidth];

            for (int i = 0; i < mapHeight; i++)
            {
                for (int j = 0; j < mapWidth; j++)
                {
                    map[i, j] = new Zone
                    {
                        X = i,
                        Y = j,
                        Occupied = false
                    };
                    if (j == 0 && i == 29)
                    {
                        BuildBuilding(new Road(i, j), true);
                    }
                }
            }
            Random random = new Random();

            for (int i = 0; i < 15; i++)
            {
                int x = random.Next(0, mapHeight);
                int y = random.Next(0, mapWidth);
                Forest forest = new Forest(x, y);
                forest.SetAge(10);
                BuildBuilding(forest, true);

            }
            OnGameCreated();
        }

        public int GetBudget()
        {
            return Finances != null ? Finances.getBudget() : 0;
        }

        public void CanDemolish(bool boolean)
        {
            canDemolish = boolean;
        }

        private void GiveEducation(School school)
        {
            school.GiveEducation();
        }

        public void demolishZone(int x, int y)
        {
            if (y == 0 && x == 29)
                return;
            Building.Building building = map[x, y].getBuilding();
            Zone zone = map[x, y];
            bool conflict = false;
            if (building != null && (building.GetType() == typeof(Residental) || building.GetType() == typeof(Industrial) || building.GetType() == typeof(Store) || building.GetType() == typeof(Road)))
            {
                switch (building)
                {
                    case Road:
                        zone.DowngradeZone();
                        foreach (Citizen citizen in citizens)
                        {
                            if (calcDistance(citizen.GetHome(), citizen.GetWorkPlace()) == -1)
                            {
                                conflict = true;
                                break;
                            }
                        }
                        zone.BuildBuilding(building);
                        break;
                    case Residental:
                        if (((Residental)building).GetInhabitans() > 0)
                            conflict = true;
                        break;
                    case Industrial:
                        if (((Industrial)building).GetWorkers() > 0)
                            conflict = true;
                        break;
                    case Store:
                        if (((Store)building).GetWorkers() > 0)
                            conflict = true;
                        break;
                    default:
                        break;
                }
            }
            if (conflict)
                OnConflictDemolish();
            if (building != null)
            {
                if (!conflict || (conflict && canDemolish))
                {
                    bool added_money = false;
                    if (building.GetType() == typeof(Road))
                    {
                        zone.DowngradeZone();
                        foreach (Zone zones in map)
                        {
                            if (zones.getBuilding() != null && zones.getBuilding().GetType() != typeof(Road) && zones.getBuilding().GetType() != typeof(PowerLine) && zones.getBuilding().GetType() != typeof(Forest) && calcDistance(map[29, 0].getBuilding(), zones.getBuilding()) == -1)
                            {
                                zone.BuildBuilding(building);
                                return;
                            }
                        }
                        zone.BuildBuilding(building);
                    }
                    List<(int, int)> buildingZones = new List<(int, int)>();
                    if (building.GetSize().Item1 == 1)
                    {
                        buildingZones.Add((building.GetX(), building.GetY()));
                        if (building.GetSize().Item2 == 2)
                            buildingZones.Add((building.GetX(), building.GetY() + 1));
                    } else
                    {
                        buildingZones.Add((building.GetX(), building.GetY()));
                        buildingZones.Add((building.GetX() + 1, building.GetY() + 1));
                        buildingZones.Add((building.GetX() + 1, building.GetY()));
                        buildingZones.Add((building.GetX(), building.GetY() + 1));
                    }
                    foreach ((int, int) zones in buildingZones)
                    {
                        if (map[zones.Item1, zones.Item2].DowngradeZone())
                        {
                            if (map[zones.Item1, zones.Item2].ZoneType != ZoneType.General)
                                Finances.addIncome("Demolished a " + map[x, y].ToString(), map[x, y].getCost() / 2, date);
                        }
                        if (building != null && added_money == false)
                        {
                            Finances.addIncome("Demolished a " + map[x, y].ToString(), building.GetCost() / 2, date);
                            added_money = true;
                        }
                    }
                }
            } else
            {
                if (map[x, y].DowngradeZone())
                {
                    Finances.addIncome("Demolished a " + map[x, y].ToString(), map[x, y].getCost() / 2, date);
                }
            }
            if (conflict && canDemolish)
            {
                List<Citizen> CitizensLeft = new List<Citizen>();
                if (building.GetType() == typeof(Road))
                {
                    foreach (Citizen citizen in Citizens)
                    {
                        if (calcDistance(citizen.GetHome(), citizen.GetWorkPlace()) == -1)
                        {
                            Building.Building workPlace = citizen.GetWorkPlace();
                            workPlace.SetAccessibility(false);
                            if (workPlace.GetType() == typeof(Store))
                            {
                                Store store = (Store)workPlace;
                                store.WorkerLeft();
                                if (!availableStores.Contains(store))
                                    availableStores.Add(store);
                            }
                            else if (workPlace.GetType() == typeof(Industrial))
                            {
                                Industrial industrial = (Industrial)workPlace;
                                industrial.WorkerLeft();
                                if (!availableIndustrials.Contains(industrial))
                                    availableIndustrials.Add(industrial);
                            }
                            map[workPlace.GetX(), workPlace.GetY()].RemoveCitizenFromZone(citizen);
                            citizen.GetHome().MoveOut();
                            if (!availableHouses.Contains(citizen.GetHome()))
                                availableHouses.Add(citizen.GetHome());
                            map[citizen.GetHome().GetX(), citizen.GetHome().GetY()].RemoveCitizenFromZone(citizen);
                            citizen.MoveOut();
                            PeopleMoveIn(citizen);
                            if (citizen.GetHome() == null)
                                CitizensLeft.Add(citizen);
                            else
                                citizen.PlusHadToMove();
                        }
                    }
                } else if (building.GetType() == typeof(Residental))
                {
                    foreach (Citizen citizen in Citizens)
                    {
                        if (citizen.GetHome() == building)
                        {
                            Building.Building workPlace = citizen.GetWorkPlace();
                            if (workPlace.GetType() == typeof(Store))
                            {
                                Store store = (Store)workPlace;
                                store.WorkerLeft();
                                if (!availableStores.Contains(store))
                                    availableStores.Add(store);
                            }
                            else if (workPlace.GetType() == typeof(Industrial))
                            {
                                Industrial industrial = (Industrial)workPlace;
                                industrial.WorkerLeft();
                                if (!availableIndustrials.Contains(industrial))
                                    availableIndustrials.Add(industrial);
                            }
                            map[workPlace.GetX(), workPlace.GetY()].RemoveCitizenFromZone(citizen);
                            citizen.MoveOut();
                            PeopleMoveIn(citizen);
                            if (citizen.GetHome() == null)
                                CitizensLeft.Add(citizen);
                            else
                                citizen.PlusHadToMove();
                        }
                    }
                } else if (building.GetType() == typeof(Industrial))
                {
                    foreach (Citizen citizen in Citizens)
                    {
                        citizen.SetWorkPlace(null!);
                        citizen.SetSalary(0);
                    }
                } else
                {
                    foreach (Citizen citizen in Citizens)
                    {
                        citizen.SetWorkPlace(null!);
                        citizen.SetSalary(0);
                    }
                }
                Finances.addExpenses("Demolished a " + zone.ToString() + " and you had conflict with people", building.GetCost() / 2, date);
                citizens.RemoveAll(i => CitizensLeft.Contains(i));
                canDemolish = false;
            }
            OnGameChanged();
        }

        public async Task Save()
        {
            if (dataAccess == null)
            {
                throw new InvalidOperationException("No data access is provided");
            }
            GameState g = new GameState();
            await dataAccess.saveGame("path", g);
        }

        public async Task Load()
        {
            if (dataAccess == null)
            {
                throw new InvalidOperationException("No data access is provided");
            }
            await dataAccess.loadGame("path");
        }

        public void UpgradeZone(int x, int y)
        {
            if (map[x, y].ZoneType != ZoneType.General && map[x, y].getBuilding() != null && map[x, y].getBuilding().GetDensity() < 3)
            {
                switch (map[x, y].getBuilding())
                {
                    case Residental r:
                        r.SetDensity(r.GetDensity() + 1);
                        r.SetMaxInhabitants(r.GetMaxInhabitants() * 2 + 2);
                        if (!availableHouses.Contains(r))
                            availableHouses.Add(r);
                        break;
                    case Industrial i:
                        i.SetDensity(i.GetDensity() + 1);
                        i.SetMaxWorkers(i.GetMaxWorkers() * 2 + 2);
                        if (!availableIndustrials.Contains(i))
                            availableIndustrials.Add(i);
                        break;
                    case Store s:
                        s.SetDensity(s.GetDensity() + 1);
                        s.SetMaxWorkers(s.GetMaxWorkers() * 2 + 2);
                        if (!availableStores.Contains(s))
                            availableStores.Add(s);
                        break;
                }
                if (map[x, y].getBuilding().GetDensity() == 2)
                    Finances.addExpenses("Upgraded " + map[x, y].ToString() + " to level 2", map[x, y].getCost(), date);
                else
                    Finances.addExpenses("Upgraded " + map[x, y].ToString() + " to level 3", map[x, y].getCost(), date);
                OnGameChanged();
            }
        }

        private void CollectingTaxes()
        {
            double tax = 0;
            foreach (Zone zone in map)
            {
                if(zone.ZoneType == ZoneType.Store)
                {
                    tax += Finances.getTax(ZoneType.Store)/100 * zone.getPeople().Count * 50;
                }
                else if(zone.ZoneType == ZoneType.Industrial)
                {
                    tax += Finances.getTax(ZoneType.Industrial)/100 * zone.getPeople().Count * 50;
                }
                else if(zone.ZoneType == ZoneType.Residental)
                {
                    foreach(Citizen citizen in zone.getPeople())
                    {
                        tax += citizen.GetSalary() * Finances.getTax(ZoneType.Residental)/100;
                    }
                }
            }
            tax = Math.Round(tax);
            Finances.addIncome("Tax", Convert.ToInt32(tax), date);
        }

        static public int calcDistance(Building.Building from, Building.Building to)
        {
            List<int> distances = new List<int>();
            HashSet<Road> visited = new HashSet<Road>();
            int n = 0;
            distancesFromTo(null, from, to, distances, visited, n);
            distances.Sort();
            return distances.Count != 0 ? distances[0] : -1;
        }

        static private void distancesFromTo(Building.Building from, Building.Building current, Building.Building to, List<int> distances, HashSet<Road> visited, int n)
        {
            if (map == null)
            {
                return;
            }

            bool found = false;
            List<(int, int)> buildingZones = new List<(int, int)>();
            if (current.GetSize().Item1 == 1)
            {
                buildingZones.Add((current.GetX(), current.GetY()));
                if (current.GetSize().Item2 == 2)
                    buildingZones.Add((current.GetX(), current.GetY() + 1));
            }
            else
            {
                buildingZones.Add((current.GetX(), current.GetY()));
                buildingZones.Add((current.GetX() + 1, current.GetY() + 1));
                buildingZones.Add((current.GetX() + 1, current.GetY()));
                buildingZones.Add((current.GetX(), current.GetY() + 1));
            }
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    foreach ((int, int) zones in buildingZones)
                    {
                        if (zones.Item1 + i >= 0 && zones.Item1 + i < mapHeight && zones.Item2 + j >= 0 && zones.Item2 + j < mapWidth)
                        {
                            if (map[zones.Item1 + i, zones.Item2 + j].getBuilding() == to)
                            {
                                distances.Add(n);
                                found = true;
                            }
                        }
                    }
                }
            }

            if (!found)
            {
                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        foreach ((int, int) zones in buildingZones)
                        {
                            if (!(Math.Abs(i) == Math.Abs(j)) && zones.Item1 + i >= 0 && zones.Item1 + i < mapHeight && zones.Item2 + j >= 0 && zones.Item2 + j < mapWidth)
                            {
                                Building.Building building = map[zones.Item1 + i, zones.Item2 + j].getBuilding();
                                if (building != from && building != null && building.GetType() == typeof(Road) && !visited.Contains(building))
                                {
                                    visited.Add((Road)building);
                                    distancesFromTo(current, building, to, distances, visited, ++n);
                                }
                            }
                        }
                    }
                }
            }
        }

        public int getHappiness()
        {
            int happiness = 0;
            foreach (Citizen i in citizens)
            {
                happiness += i.calcHappiness();
            }
            return happiness;
        }

        public bool newZone(int x, int y, ZoneType zoneType)
        {
            if (map[x, y].SetZone(zoneType))
            {
                if (zoneType == ZoneType.Industrial && calcDistance(map[x, y].getBuilding(), map[29, 0].getBuilding()) != -1)
                {
                    availableIndustrials.Add((Industrial)map[x, y].getBuilding());
                    map[x, y].getBuilding().SetAccessibility(true);
                }
                else if (zoneType == ZoneType.Store && calcDistance(map[x, y].getBuilding(), map[29, 0].getBuilding()) != -1)
                {
                    availableStores.Add((Store)map[x, y].getBuilding());
                    map[x, y].getBuilding().SetAccessibility(true);
                }
                Finances.addExpenses("Built a " + map[x, y].ToString(), map[x, y].getCost(), date);
                OnGameChanged();
                return true;
            } else
            {
                return false;
            }
        }

        public void BuildBuilding(Building.Building building, bool inConstructor = false)
        {
            if (building == null)
                return;
            
            bool freeZone = true;
            List<(int, int)> buildingZones = new List<(int, int)>();
            if (building.GetSize().Item1 == 1)
            {
                buildingZones.Add((building.GetX(), building.GetY()));
                if (building.GetSize().Item2 == 2)
                    buildingZones.Add((building.GetX(), building.GetY() + 1));
            }
            else
            {
                buildingZones.Add((building.GetX(), building.GetY()));
                buildingZones.Add((building.GetX() + 1, building.GetY() + 1));
                buildingZones.Add((building.GetX() + 1, building.GetY()));
                buildingZones.Add((building.GetX(), building.GetY() + 1));
            }
            foreach ((int, int) zones in buildingZones)
            {
                if (zones.Item1 >= 0 && zones.Item1 < mapHeight && zones.Item2 >= 0 && zones.Item2 < mapWidth)
                {
                    Zone zone = map[zones.Item1, zones.Item2];
                    freeZone = freeZone && (zone.ZoneType == ZoneType.General && zone.getBuilding() == null);
                } else
                {
                    freeZone = false;
                    break;
                }
            }
            if (freeZone)
            {
                if (!inConstructor)
                {
                    Finances.addExpenses("Built a ", building.GetCost(), date);
                    if (calcDistance(map[29, 0].getBuilding(), building) != -1)
                        building.SetAccessibility(true);
                    if (building.GetType() == typeof(Road))
                    {
                        foreach (Zone zone in map)
                        {
                            if (zone.getBuilding() != null && !zone.getBuilding().GetAccessibility() && calcDistance(map[29, 0].getBuilding(), zone.getBuilding()) != -1)
                            {
                                zone.getBuilding().SetAccessibility(true);
                                switch (zone.getBuilding())
                                {
                                    case Industrial:
                                        availableIndustrials.Add((Industrial)zone.getBuilding());
                                        break;
                                    case Store:
                                        availableStores.Add((Store)zone.getBuilding());
                                        break;
                                    case School:
                                        avaibleSchools.Add((School)zone.getBuilding());
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
                if (building.GetType() == typeof(School) && calcDistance(map[29, 0].getBuilding(), building) != -1)
                    avaibleSchools.Add((School)building);
                 
                foreach ((int, int) zones in buildingZones)
                {
                    map[zones.Item1, zones.Item2].BuildBuilding(building);
                }
                if (!inConstructor)
                    OnGameChanged();
                return;
            } else
            {
                if (!inConstructor)
                    OnGameChanged();
                return;
            }
        }

        private void GetJobForUnemployed()
        {
            foreach(Citizen citizen in citizens)
            {
                if (citizen.GetWorkPlace() == null)
                {
                    int inStores = 0;
                    int inIndustrial = 0;
                    foreach (Store store in availableStores)
                    {
                        inStores += store.GetWorkers();
                    }
                    foreach (Industrial industrial in availableIndustrials)
                    {
                        inIndustrial += industrial.GetWorkers();
                    }
                    if (inIndustrial <= inStores)
                    {
                        Industrial building = null!;
                        int minDist = int.MaxValue;
                        List<Industrial> delete = new List<Industrial>();
                        foreach (Industrial industrial in availableIndustrials)
                        {
                            int dist = calcDistance(citizen.GetHome(), industrial);
                            if (dist != -1 && dist < minDist && industrial.FreeSpace())
                            {
                                minDist = dist;
                                building = industrial;
                            }
                            else if (building != null && !building.FreeSpace())
                                delete.Add(building);
                        }
                        availableIndustrials.RemoveAll(e => delete.Contains(e));
                        if (building != null)
                        {
                            building.NewWorker();
                            citizen.SetWorkPlace(building);
                        }
                    }
                    else
                    {
                        Store building = null!;
                        int minDist = int.MaxValue;
                        List<Store> delete = new List<Store>();
                        foreach (Store store in availableStores)
                        {
                            int dist = calcDistance(citizen.GetHome(), store);
                            if (dist != -1 && dist < minDist && store.FreeSpace())
                            {
                                minDist = dist;
                                building = store;
                            }
                            else if (building != null && !building.FreeSpace())
                                delete.Add(building);
                        }
                        availableStores.RemoveAll(i => delete.Contains(i));
                        if (building != null)
                        {
                            building.NewWorker();
                            citizen.SetWorkPlace(building);
                        }
                    }
                    switch (citizen.GetEducation())
                    {
                        case EducationLevel.Basic:
                            citizen.SetSalary(500);
                            break;
                        case EducationLevel.Middle:
                            citizen.SetSalary(1000);
                            break;
                        case EducationLevel.Higher:
                            citizen.SetSalary(1500);
                            break;
                    }
                }
            }
        }

        public void AdvanceTime()
        {
            DateTime previous_date = date;
            date = date.AddDays(1);
            if (date.Month > previous_date.Month)
            {
                CollectingTaxes();
                double upkeep = 0;
                foreach (Zone zone in map)
                {
                    if (zone.ZoneType == ZoneType.General && zone.getBuilding() != null)
                    {
                        upkeep += zone.getBuilding().GetCost() * 0.01;
                    }
                }
                Finances.addExpenses("Monthly running expenses", Convert.ToInt32(upkeep), date);
            } else if (date.Year > previous_date.Year)
            {
                foreach (School school in avaibleSchools)
                    GiveEducation(school);

                foreach (Zone zone in map)
                {
                    if (zone.getBuilding() != null && zone.getBuilding().GetType() == typeof(Forest))
                        ((Forest)zone.getBuilding()).AgeUp();
                }

                foreach (Citizen citizen in citizens)
                {
                    Residental home = citizen.GetHome();
                    Building.Building workPlace = citizen.GetWorkPlace();
                    if (!citizen.AgeUp())
                    {
                        map[workPlace.GetX(), workPlace.GetY()].RemoveCitizenFromZone(citizen);
                        map[home.GetX(), home.GetY()].RemoveCitizenFromZone(citizen);
                        citizens.Remove(citizen);
                        PeopleMoveIn(Citizen.ReGen18());
                    }
                }
            }
            GoToSchool();
            GetJobForUnemployed();
            PeopleMoveIn();
            OnTimeAdvanced();
        }

        private void GoToSchool()
        {
            if (citizens.Count == 0 || avaibleSchools.Count == 0)
                return;

            Random random = new Random();
            Citizen citizen;
            
            citizen = citizens[random.Next(citizens.Count)];

            foreach (School school in avaibleSchools)
            {
                if (citizen.GetEducation() == EducationLevel.Basic && school.GetSchoolType() == SchoolTypes.Elementary && school.GetMaxStudent() > school.GetStudents())
                {
                    if (random.NextDouble() <= 40)
                        school.SetStudents(citizen);
                    break;
                } else if (citizen.GetEducation() == EducationLevel.Middle && school.GetSchoolType() == SchoolTypes.University && school.GetMaxStudent() > school.GetStudents())
                {
                    if (random.NextDouble() <= 20)
                        school.SetStudents(citizen);
                    break;
                }
            }
        }

        private void PeopleMoveIn(Citizen citizen = null!)
        {
            if (peopleAtStart > 0 || citizen != null)
            {
                Zone houseZone = null!;
                Residental house = null!;
                if (availableHouses.Count == 0)
                {
                    foreach (Zone zone in map)
                    {
                        if (zone.ZoneType == ZoneType.Residental && !zone.Occupied)
                        {
                            zone.BuildBuilding();
                            if (calcDistance(zone.getBuilding(), map[29, 0].getBuilding()) == -1)
                            {
                                zone.DowngradeZone();
                                zone.SetZone(ZoneType.Residental);
                                continue;
                            }
                            else
                            {
                                zone.DowngradeZone();
                                zone.SetZone(ZoneType.Residental);
                                houseZone = zone;
                                break;
                            }
                        }
                    }
                } else
                {
                    List<Residental> delete = new List<Residental>();
                    foreach (Residental houses in availableHouses)
                    {
                        if (houses.FreeSpace())
                        {
                            house = houses;
                            break;
                        } else
                        {
                            delete.Add(houses);
                        }
                    }
                    availableHouses.RemoveAll(h => delete.Contains(h));
                }
                int inStores = 0;
                int inIndustrial = 0;
                foreach (Store store in availableStores)
                {
                    inStores += store.GetWorkers();
                }
                foreach (Industrial industrial in availableIndustrials)
                {
                    inIndustrial += industrial.GetWorkers();
                }
                if (houseZone != null)
                {
                    house = new Residental(houseZone.X, houseZone.Y);
                }
                if (house != null && inIndustrial <= inStores)
                {
                    Industrial building = null!;
                    int minDist = int.MaxValue;
                    List<Industrial> delete = new List<Industrial>();
                    foreach (Industrial industrial in availableIndustrials)
                    {
                        int dist = calcDistance(house, industrial);
                        if (dist != -1 && dist < minDist && industrial.FreeSpace())
                        {
                            minDist = dist;
                            building = industrial;
                        }
                        else if (building != null && !building.FreeSpace())
                            delete.Add(building);
                    }
                    availableIndustrials.RemoveAll(e => delete.Contains(e));
                    if (building != null)
                    {
                        if (houseZone != null)
                        {
                            houseZone.Occupied = true;
                            houseZone.BuildBuilding();
                            house = (Residental)houseZone.getBuilding();
                            availableHouses.Add(house);
                        }
                        if (citizen == null)
                        {
                            citizen = Citizen.ReGen(house);
                            citizens.Add(citizen);
                            peopleAtStart--;
                        } else
                            citizen.SetHome(house);
                        house.MoveIn();
                        map[house.GetX(), house.GetY()].addCitizenToZone(citizen);
                        map[building.GetX(), building.GetY()].addCitizenToZone(citizen);
                        building.NewWorker();
                        citizen.SetWorkPlace(building);
                        citizen.SetSalary(500);
                    }
                } else if (house != null)
                {
                    Store building = null!;
                    int minDist = int.MaxValue;
                    List<Store> delete = new List<Store>();
                    foreach (Store store in availableStores)
                    {
                        int dist = calcDistance(house, store);
                        if (dist != -1 && dist < minDist && store.FreeSpace())
                        {
                            minDist = dist;
                            building = store;
                        } else if (building != null &&!building.FreeSpace())
                            delete.Add(building);
                    }
                    availableStores.RemoveAll(i => delete.Contains(i));
                    if (building != null)
                    {
                        if (houseZone != null)
                        {
                            houseZone.Occupied = true;
                            houseZone.BuildBuilding();
                            house = (Residental)houseZone.getBuilding();
                            availableHouses.Add(house);
                        }
                        if (citizen == null)
                        {
                            citizen = Citizen.ReGen(house);
                            citizens.Add(citizen);
                            peopleAtStart--;
                        } else
                            citizen.SetHome(house);
                        house.MoveIn();
                        map[house.GetX(), house.GetY()].addCitizenToZone(citizen);
                        map[building.GetX(), building.GetY()].addCitizenToZone(citizen);
                        building.NewWorker();
                        citizen.SetWorkPlace(building);
                        citizen.SetSalary(500);
                    }
                }
            } else
            {
                int AVGhappiness = getHappiness() / citizens.Count;
                Residental house = null!;
                Zone houseZone = null!;
                if (availableHouses.Count == 0)
                {
                    foreach (Zone zone in map)
                    {
                        if (zone.ZoneType == ZoneType.Residental)
                        {
                            zone.BuildBuilding();
                            if (calcDistance(zone.getBuilding(), map[29, 0].getBuilding()) == -1)
                            {
                                zone.DowngradeZone();
                                zone.SetZone(ZoneType.Residental);
                                continue;
                            }
                            else
                            {
                                zone.DowngradeZone();
                                zone.SetZone(ZoneType.Residental);
                            }
                        }
                        if (zone.ZoneType == ZoneType.Residental && !zone.Occupied)
                        {
                            houseZone = zone;
                            break;
                        }
                    }
                } else
                {
                    List<Residental> delete = new List<Residental>();
                    foreach (Residental houses in availableHouses)
                    {
                        if (houses.FreeSpace())
                        {
                            house = houses;
                            break;
                        }
                        else
                        {
                            delete.Add(houses);
                        }
                    }
                    availableHouses.RemoveAll(h => delete.Contains(h));
                }
                int inStores = 0;
                int inIndustrial = 0;
                foreach (Store store in availableStores)
                {
                    inStores += store.GetWorkers();
                }
                foreach (Industrial industrial in availableIndustrials)
                {
                    inIndustrial += industrial.GetWorkers();
                }
                if (houseZone != null)
                {
                    house = new Residental(houseZone.X, houseZone.Y);
                }
                if (house != null && inIndustrial < inStores)
                {
                    Industrial building = null!;
                    int minDist = int.MaxValue;
                    List<Industrial> delete = new List<Industrial>();
                    foreach (Industrial industrial in availableIndustrials)
                    {
                        int dist = calcDistance(house, industrial);
                        if (dist != -1 && dist < minDist && industrial.FreeSpace())
                        {
                            minDist = dist;
                            building = industrial;
                        }
                        else if (building != null && !building.FreeSpace())
                            delete.Add(building);
                    }
                    availableIndustrials.RemoveAll(i => delete.Contains(i));
                    bool found = false;
                    for (int i = 0; i < 4 && !found; i++)
                    {
                        for (int j = 0; j < 4 && !found; j++)
                        {
                            if (map[house.GetX() + i, house.GetY() + j].ZoneType == ZoneType.Industrial)
                            {
                                found = true;
                                break;
                            }
                        }
                    }
                    if (building != null && minDist < 30 && !found && AVGhappiness > 5)
                    {
                        if (houseZone != null)
                        {
                            houseZone.Occupied = true;
                            houseZone.BuildBuilding();
                            house = (Residental)houseZone.getBuilding();
                            availableHouses.Add(house);
                        }
                        citizen = Citizen.ReGen(house);
                        house.MoveIn();
                        building.NewWorker();
                        citizen.SetWorkPlace(building);
                        citizen.SetSalary(500);
                        citizens.Add(citizen);
                        map[house.GetX(), house.GetY()].addCitizenToZone(citizen);
                        map[building.GetX(), building.GetY()].addCitizenToZone(citizen);
                    }
                }
                else if (house != null)
                {
                    Store building = null!;
                    int minDist = int.MaxValue;
                    List<Store> delete = new List<Store>();
                    foreach (Store store in availableStores)
                    {
                        int dist = calcDistance(house, store);
                        if (dist != -1 && dist < minDist && store.FreeSpace())
                        {
                            minDist = dist;
                            building = store;
                        }
                        else if (building != null && !building.FreeSpace())
                            delete.Add(building);
                    }
                    availableStores.RemoveAll(s => delete.Contains(s));
                    bool found = false;
                    for (int i = 0; i < 4 && !found; i++)
                    {
                        for (int j = 0; j < 4 && !found; j++)
                        {
                            if (map[house.GetX() + i, house.GetY() + j].ZoneType == ZoneType.Industrial)
                            {
                                found = true;
                                break;
                            }
                        }
                    }
                    if (building != null && minDist < 30 && !found && AVGhappiness > 5)
                    {
                        if (houseZone != null)
                        {
                            houseZone.Occupied = true;
                            houseZone.BuildBuilding();
                            house = (Residental)houseZone.getBuilding();
                            availableHouses.Add(house);
                        }
                        citizen = Citizen.ReGen(house);
                        house.MoveIn();
                        building.NewWorker();
                        citizen.SetWorkPlace(building);
                        citizen.SetSalary(500);
                        citizens.Add(citizen);
                        map[house.GetX(), house.GetY()].addCitizenToZone(citizen);
                        map[building.GetX(), building.GetY()].addCitizenToZone(citizen);
                    }
                }
            }
            OnGameChanged();
        }

        private void OnTimeAdvanced()
        {
            gameAdvanced?.Invoke(this, EventArgs.Empty);
        }

        private void OnGameCreated()
        {
            gameCreated?.Invoke(this, EventArgs.Empty);
        }

        private void OnConflictDemolish()
        {
            ConflictDemolish?.Invoke(this, EventArgs.Empty);
        }

        private void OnGameChanged()
        {
            gameChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}