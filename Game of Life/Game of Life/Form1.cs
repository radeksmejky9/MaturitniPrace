using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game_of_Life
{
    public partial class Form1 : Form
    {
        int baseTimerInterval = 100;
        Cell[,] cells;
        int cellSize = 20;
        int roundCount = 0;
        Random r = new Random();
        int minWindowHeight = 230;
        int minWindowWidth = 830;
        public Form1()
        {
            InitializeComponent();
            cells = new Cell[39, 20];
            InitBoard();
            canvas2.Width = cells.GetLength(0) * cellSize;
            canvas2.Height = cells.GetLength(1) * cellSize;
            timer1.Interval = baseTimerInterval;
        }

        private void TrackBar1_ValueChanged(object sender, EventArgs e)
        {
            //propisování hodnot do ostatních prvků
            WritePercentageCountsInputs(trackBar1.Value);
        }

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            //propisování hodnot do ostatních prvků
            WritePercentageCountsInputs((int)numericUpDown1.Value);

        }

        private void Canvas2_Paint(object sender, PaintEventArgs e)
        {
            //Vykreslování celého pole
            for (int i = 0; i < cells.GetLength(0); i++)
            {
                for (int j = 0; j < cells.GetLength(1); j++)
                {
                    e.Graphics.DrawRectangle(Pens.Black, i * cells[i, j].Size, j * cells[i, j].Size, cells[i, j].Size, cells[i, j].Size);
                    if (cells[i, j].Dead)
                    {
                        //Vykreslení když je mrtvý
                        e.Graphics.FillRectangle(Brushes.White, i * cells[i, j].Size + 1, j * cells[i, j].Size + 1, cells[i, j].Size, cells[i, j].Size);
                    }
                    else
                    {
                        //Jinak na základě toho kolik buňka přežila kol
                        if (cells[i, j].TurnsAlive < 2)
                        {
                            e.Graphics.FillRectangle(Brushes.Yellow, i * cells[i, j].Size + 1, j * cells[i, j].Size + 1, cells[i, j].Size, cells[i, j].Size);
                        }
                        else if (cells[i, j].TurnsAlive < 3)
                        {
                            e.Graphics.FillRectangle(Brushes.Orange, i * cells[i, j].Size + 1, j * cells[i, j].Size + 1, cells[i, j].Size, cells[i, j].Size);
                        }
                        else if (cells[i, j].TurnsAlive < 4)
                        {
                            e.Graphics.FillRectangle(Brushes.OrangeRed, i * cells[i, j].Size + 1, j * cells[i, j].Size + 1, cells[i, j].Size, cells[i, j].Size);
                        }
                        else
                        {
                            e.Graphics.FillRectangle(Brushes.Red, i * cells[i, j].Size + 1, j * cells[i, j].Size + 1, cells[i, j].Size, cells[i, j].Size);
                        }

                    }
                }
            }
        }

        public void InitBoard()
        {
            //tvorba hrací plochy
            for (int i = 0; i < cells.GetLength(0); i++)
            {
                for (int j = 0; j < cells.GetLength(1); j++)
                {
                    //tvorba jednotlivých buněk
                    Cell c = new Cell(cellSize, false, 0);
                    cells[i, j] = c;
                }
            }
            canvas2.Width = cells.GetLength(0) * cellSize;
            canvas2.Height = cells.GetLength(1) * cellSize;
            this.Width = canvas2.Width + 40 > minWindowWidth ? canvas2.Width + 40 : minWindowWidth;
            this.Height = canvas2.Height + minWindowHeight;
            Refresh();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            //vygenerování mrtvých na stisk tlačítka
            GenerateDeadUsingNumeric();
        }
        public void GenerateDeadUsingNumeric()
        {
            ClearAllCells();
            //Generování mrtvých buněk dle vložené hodnoty
            int deadCount = cells.Length - (int)(cells.Length / 100.0 * (int)numericUpDown1.Value);
            int actuallyDead = 0;

            while (deadCount > actuallyDead)
            {
                //dokud není dostatečný počet mrtvých, "zabíjí" na náhodných pozicích živé buňky
                int x = r.Next(0, cells.GetLength(0));
                int y = r.Next(0, cells.GetLength(1));
                if (!cells[x, y].Dead)
                {
                    cells[x, y].Dead = true;
                    actuallyDead++;
                }
            }
            //Zápis do všech prvků obsahující % podíl a následné překreslení hrací plochy
            Refresh();
            WritePercentageCountsAll(RecalculateStateOfCells());
            MessageBox.Show("Úspěšně vygenerováno!");
        }


        private void Canvas2_MouseClick(object sender, MouseEventArgs e)
        {
            //Přebarvení buňky na kliknutí levého tlačítka
            cells[e.X / cellSize, e.Y / cellSize].Dead = !cells[e.X / cellSize, e.Y / cellSize].Dead;
            Refresh();
            WritePercentageCountsAll(RecalculateStateOfCells());
        }

        private int RecalculateStateOfCells()
        {
            //Získání procentuálního podílu mrtvých buněk
            int dead = 0;
            foreach (var item in cells)
            {
                if (item.Dead)
                    dead++;
            }
            return (int)(100 - (dead / (double)cells.Length * 100.0));

        }

        private void WritePercentageCountsInputs(int percent)
        {
            //Propis aktuálních % do obou vstupů
            numericUpDown1.Value = percent;
            trackBar1.Value = percent;
        }
        private void WritePercentageCountsAll(int percent)
        {
            //Propis aktuálních % do všech prvků UI
            WritePercentageCountsInputs(percent);
            label2.Text = "Aktualní stav: " + percent + " % živých buněk";
        }

        private void ClearAllCells()
        {
            //Oživení všech buněk
            foreach (var item in cells)
            {
                item.Dead = false;
                item.TurnsAlive = 0;
            }
            Refresh();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            //Tlačítko pro oživení všech buněk
            ClearAllCells();
            WritePercentageCountsAll(RecalculateStateOfCells());
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            //Start/Stop tlačítko
            button2.BackColor = button2.BackColor == Color.Green ? Color.Red : Color.Green;
            button2.Text = button2.Text == "Start" ? "Stop" : "Start";
            if (button2.Text == "Start")
            {
                //Program se vrátí do fáze 1
                timer1.Stop();
                ClearAllCells();
                roundCount = 0;
                InputsEnableDisable(true);
                label6.Text = "Počet kol: " + 0;
                WritePercentageCountsAll(RecalculateStateOfCells());
            }
            else
            {
                //Začátek fáze 2
                timer1.Start();
                InputsEnableDisable(false);
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            //vytvoření kopie pole a následné překopírování obsahu
            Cell[,] cellsCopy = new Cell[cells.GetLength(0), cells.GetLength(1)];
            for (int i = 0; i < cells.GetLength(0); i++)
            {
                for (int j = 0; j < cells.GetLength(1); j++)
                {
                    cellsCopy[i, j] = new Cell(cells[i, j].Size, cells[i, j].Dead, cells[i, j].TurnsAlive);
                }
            }
            roundCount++;
            label6.Text = "Počet kol: " + roundCount;

            //výpočet sousedů pro všechna políčka a následná změna stavu dle podmínky
            int neighborCount;
            for (int i = 0; i < cellsCopy.GetLength(0); i++)
            {
                for (int j = 0; j < cellsCopy.GetLength(1); j++)
                {
                    //díky zisku počtu sousedů lze jen porovnat použitím pravidel
                    neighborCount = GetNeighborCount(i, j);
                    if (cells[i, j].Dead)
                    {
                        //Mrtvá buňka ožívá, pokud má přesně tři sousedy
                        if (neighborCount == 3)
                        {
                            cellsCopy[i, j].Dead = false;
                        }
                    }
                    else
                    {
                        //Živá buňka s dvěma nebo třemi sousedy přežívá bez změny
                        if (neighborCount == 2 || neighborCount == 3)
                        {
                            cellsCopy[i, j].Dead = false;
                            cellsCopy[i, j].TurnsAlive++;
                        }
                        //Živá buňka s méně než dvěma živými sousedy umírá
                        else if (neighborCount < 2)
                        {
                            cellsCopy[i, j].Dead = true;
                            cellsCopy[i, j].TurnsAlive = 0;

                        }
                        //Živá buňka s více než třemi živými sousedy umírá na přemnožení...
                        else if (neighborCount > 3)
                        {
                            cellsCopy[i, j].Dead = true;
                            cellsCopy[i, j].TurnsAlive = 0;

                        }
                    }
                }
            }
            //kopírování obsahu z upravené pole po kole
            for (int i = 0; i < cells.GetLength(0); i++)
            {
                for (int j = 0; j < cells.GetLength(1); j++)
                {
                    cells[i, j] = new Cell(cellsCopy[i, j].Size, cellsCopy[i, j].Dead, cellsCopy[i, j].TurnsAlive);

                }
            }
            cellsCopy = null;
            //Zápis do všech prvků obsahující % podíl a následné překreslení hrací plochy
            WritePercentageCountsAll(RecalculateStateOfCells());
            Refresh();
        }

        private int GetNeighborCount(int x, int y)
        {
            //zisk počtu sousedů
            int neighborCount = 0;
            if (!cells[x, y].Dead)
                neighborCount--;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int newx = x + i;
                    int newy = y + j;
                    //Ošetření, aby nedošlo k zásahu mimo pole
                    if ((newx > 0 && newx < cells.GetLength(0)) && (newy > 0 && newy < cells.GetLength(1)))
                    {
                        if (!cells[newx, newy].Dead)
                            neighborCount++;
                    }
                }
            }
            return neighborCount;


        }

        private void InputsEnableDisable(bool en)
        {
            //Vypnutí přístupu ke všem ovladatelným prvkům při běhu 2. fáze
            trackBar1.Enabled = en;
            numericUpDown1.Enabled = en;
            button1.Enabled = en;
            button3.Enabled = en;
            canvas2.Enabled = en;
            numericUpDown2.Enabled = en;
            numericUpDown3.Enabled = en;
            button4.Enabled = en;
        }


        private void TrackBar2_ValueChanged(object sender, EventArgs e)
        {
            //úprava časovače při změně hodnoty trackbaru
            timer1.Interval = (int)(baseTimerInterval / ((double)trackBar2.Value / 100));
            label8.Text = "Rychlost časovače: " + (trackBar2.Value / 100.0).ToString() + "x";
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            //Změna velikosti okna a přegenerování hrací plochy
            int x = (int)numericUpDown2.Value;
            int y = (int)numericUpDown3.Value;
            cells = new Cell[x, y];
            //následné přegenerování hrací plochy a % počtu mrtvých
            InitBoard();
            GenerateDeadUsingNumeric();
        }

    }
}
