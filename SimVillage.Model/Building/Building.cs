using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SimVillage.Model.Building
{
    [JsonDerivedType(typeof(FireDepartment), typeDiscriminator: "FireDepartment")]
    [JsonDerivedType(typeof(Forest), typeDiscriminator: "Forest")]
    [JsonDerivedType(typeof(Industrial), typeDiscriminator: "Industrial")]
    [JsonDerivedType(typeof(PoliceDepartment), typeDiscriminator: "PoliceDepartment")]
    [JsonDerivedType(typeof(PowerLine), typeDiscriminator: "PowerLine")]
    [JsonDerivedType(typeof(PowerPlant), typeDiscriminator: "PowerPlant")]
    [JsonDerivedType(typeof(Residental), typeDiscriminator: "Residental")]
    [JsonDerivedType(typeof(Road), typeDiscriminator: "Road")]
    [JsonDerivedType(typeof(School), typeDiscriminator: "School")]
    [JsonDerivedType(typeof(Stadium), typeDiscriminator: "Stadium")]
    [JsonDerivedType(typeof(Store), typeDiscriminator: "Store")]
    public abstract class Building
    {
        protected (int, int) Size;
        public bool Occupied { get; protected set; }
        public int Density { get; protected set; }
        protected int Cost;
        protected int PowerConsumption;
        public bool IsPowered { get; protected set; }
        public bool IsAccessible { get; protected set; }
        public int X { get; protected set; }
        public int Y { get; protected set; }


        public Building((int, int) size, bool occupied, int density, int powerConsumption, int x, int y)
        {
            Size = size;
            Occupied = occupied;
            Density = density;
            PowerConsumption = powerConsumption;
            IsAccessible = false;
            IsPowered = false;
            X = x;
            Y = y;
        }
        public Building() { }

        public (int,int) GetSize() { return Size; }
        public int GetPowerConsumption() {  return PowerConsumption; }
        public int GetDensity() { return Density;}
        public bool GetOccupied() {  return Occupied;}
        public bool GetAccessibility() { return IsAccessible;}
        public bool GetIsPowered() { return IsPowered;}
        public int GetX() { return X;}
        public int GetY() { return Y;}


        public void SetSize(int size1,int size2) { Size = (size1, size2); }
        public void SetDensity(int density) { Density = density;}
        public void SetOccupied(bool occupied) {  Occupied = occupied;}
        public void SetPowerConsumption(int powerconsumption) { PowerConsumption = powerconsumption; }
        public void SetAccessibility(bool accessibility) { IsAccessible = accessibility;}
        public void SetIsPowered(bool powered) {  IsPowered = powered;}
        public void SetX(int x) {  X = x;}
        public void SetY(int y) {  Y = y;}
        public void Demolish()
        {
            Density = 0;
            Occupied = false;
            PowerConsumption = 0;
            Size = (0, 0);
        }
        public int GetCost()
        {
            return Cost;
        }
    }
}
