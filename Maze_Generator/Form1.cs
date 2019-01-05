using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Maze_Generator
{
    public partial class Form1 : Form
    {
        private Maze maze;

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Handle the event when the generate button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Generate_Click(object sender, EventArgs e)
        {
            System.Drawing.Graphics graphicsObj;
            graphicsObj = this.CreateGraphics();
            graphicsObj.Clear(Color.WhiteSmoke);

            //Generate maze
            maze = new Maze(Convert.ToInt32(numericUpDown1.Value), Convert.ToInt32(numericUpDown2.Value));
            maze.Generate();

            maze.DrawMaze(graphicsObj);

        }

        /// <summary>
        /// Handle the event when the solve button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Resolve_Click(object sender, EventArgs e)
        {
            System.Drawing.Graphics graphicsObj;
            graphicsObj = this.CreateGraphics();

            //In case of clicking on resolve first
            if (maze == null)
            {
                maze = new Maze(Convert.ToInt32(numericUpDown1.Value), Convert.ToInt32(numericUpDown2.Value));
                maze.Generate();
            }

            //Clear and re-draw properly the maze
            graphicsObj.Clear(Color.WhiteSmoke);
            maze.DrawMaze(graphicsObj);

            foreach (Cell cell in maze.CellList)
            {
                cell.Visited = false;
            }

            maze.DrawSolution(graphicsObj);
        }
    }
}
