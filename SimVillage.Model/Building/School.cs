namespace SimVillage.Model.Building
{
    public class School : Building
    {
        int MaxStudents;
        public SchoolTypes Type { get; private set; }
        public List<Citizen> Students { get; set; }

        public School(int x, int y, SchoolTypes type)
        {
            FireChance = 2;
            IsOnFire = false;
            MaxStudents = 300;
            PowerConsumption = 50;
            if (type == SchoolTypes.Elementary)
            { 
                Size = (1, 2);
                Health = 150;
            }
            else
            { 
                Size = (2, 2);
                Health = 250;
            }
            Type = type;
            Cost = 600;
            X = x;
            Y = y;
            Students = new List<Citizen>();
        }

        public int GetMaxStudent()
        {
            return MaxStudents;
        }

        public int GetStudents()
        {
            return Students.Count;
        }

        public void SetStudents(Citizen student)
        {
            if (Students.Count < MaxStudents)
                Students.Append(student);
        }

        public void GiveEducation()
        {
            foreach(Citizen student in Students)
            {
                if(Type == SchoolTypes.Elementary)
                {
                    student.EducationLevel = EducationLevel.Middle;
                    if (student.WorkPlace != null)
                        student.Salary = 1000;
                }
                else if(Type == SchoolTypes.University)
                {
                    student.EducationLevel = EducationLevel.Higher;
                    if (student.WorkPlace != null)
                        student.Salary = 1500;
                }
            }
        }

        public override String ToString()
        {
            return "School type: " + Type + "\nCurrent students " + Students.Count + "\nMaximum students: " + MaxStudents + "\nPower consumption: " + PowerConsumption + "\nMaintenance cost: " + Cost / 100 + "\nHealth: " + Health + "\n";
        }
    }
}
