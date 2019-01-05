namespace Maze_Generator
{
    public class Cell
    {
        public bool NorthWall = true;
        public bool SouthWall = true;
        public bool WestWall = true;
        public bool EastWall = true;
        public int Id;
        public int Row;
        public int Column;
        public bool Visited = false;

        public Cell(int id, int row, int column)
        {
            this.Id = id;
            this.Row = row;
            this.Column = column;
        }

        /// <summary>
        /// Allow to check state of integrity of a Cell
        /// </summary>
        /// <returns></returns>
        public bool CheckWalls()
        {
            if (this.EastWall == false || this.NorthWall == false || this.SouthWall == false || this.WestWall == false)
            {
                return false;
            }

            return true;
        }
    }
}
