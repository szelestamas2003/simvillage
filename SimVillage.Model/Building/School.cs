namespace SimVillage.Model.Building
{
    public class School : Building
    {
        int MaxStudents;
        SchoolTypes Type;
        List<Citizen> Students;

        public School(List<Tile> tile, SchoolTypes type)
        {
            MaxStudents = 300;
            SetTiles(tile);
            SetPowerConsumption(50);
            Type = type;
            
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
            Students.Append(student);
        }
        public SchoolTypes GetSchoolType()
        {
            return Type;
        }
        public void GiveEducation()
        {
            foreach(Citizen student in Students)
            {
                if(Type == SchoolTypes.Elementary)
                {
                    student.SetEducation(EducationLevel.Middle);
                }
                else if(Type == SchoolTypes.University)
                {
                    student.SetEducation(EducationLevel.Higher);
                }
            }
        }

        public override String ToString()
        {
            return "School type: " + Type + "\nCurrent students " + Students.Count + "\nMaximum students: " + MaxStudents + "\nPower consumption: " + PowerConsumption + "\nMaintenance cost: " + cost / 100;
        }


    }
}
