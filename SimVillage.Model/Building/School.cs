﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimVillage.Model.Building
{
    public class School : Building
    {
        int MaxStudents;
        int Students;
        SchoolTypes Type;

        public School(List<Tile> tile, SchoolTypes type)
        {
            MaxStudents = 300;
            SetTiles(tile);
            SetPowerConsumption(50);
            Type = type;
            cost = 600;
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
        public SchoolTypes GetSchoolType()
        {
            return Type;
        }


    }
}
