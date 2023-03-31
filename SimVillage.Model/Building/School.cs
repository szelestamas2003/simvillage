namespace SimVillage.Model.Building
{
    public class School : Building
    {
        int MaxStudents;
        int Students;
        SchoolTypes Type;

        public School(SchoolTypes type, int x, int y)
        {
            MaxStudents = 300;
            PowerConsumption = 50;
            Type = type;
            Size = type == SchoolTypes.University ? (2, 2) : (1, 2);
            Cost = type == SchoolTypes.University ? 50 : 25;
            X = x;
            Y = y;
        }

        public int GetMaxStudent()
        {
            return MaxStudents;
        }
        public int GetStudents()
        {
            return Students;
        }
        public void SetStudents(int students)
        {
            Students = students;
        }

        public void ModifyStudents(int students)
        {
            Students += students;
        }

        public SchoolTypes GetSchoolType()
        {
            return Type;
        }


    }
}
