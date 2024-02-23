using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_of_Life
{
    public class Cell
    {
        int size;
        bool dead;
        int turnsAlive = 0;

        public Cell(int size, bool dead, int turnsAlive)
        {
            this.Size = size;
            this.Dead = dead;
            this.TurnsAlive = turnsAlive;
        }

        public int Size { get => size; set => size = value; }
        public bool Dead { get => dead; set => dead = value; }
        public int TurnsAlive { get => turnsAlive; set => turnsAlive = value; }
    }
}
