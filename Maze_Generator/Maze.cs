using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Maze_Generator;

public class Maze
{
    Random rnd = new Random();
    public int Height;
    public int Width;
    public List<Cell> CellList = new List<Cell>();

    public Maze(int providedHeight, int providedWidth)
    {
        this.Height = providedHeight;
        this.Width = providedWidth;
    }

    /// <summary>
    /// Generates a maze with previously given informations about size
    /// </summary>
    /// <returns></returns>
    public List<Cell> Generate()
        {
            List<Cell> cellList = this.CellList;
            int totalCells = Height * Width;
            int row = 1;
            int column = 1;
            Stack<Cell> cellStack = new Stack<Cell>();

            //Filling the Cell List with our generated cells, with correct rows and columns
            for (int i = 1; i != totalCells + 1; i++)
            {
                if (column == Width + 1)
                {
                    row++;
                    column = 1;
                }

                cellList.Add(new Cell(i, row, column));
                column++;
            }

            int randomId = rnd.Next(1, totalCells);
            Cell currentCell = FindCell(randomId);
            int visitedCells = 1;

            while (visitedCells < totalCells)
            {
                List<Cell> neighbors = FindNeighborCells(currentCell);

                //If there are possible cell(s) around, we pick one and destroy walls between both cells, then add cell on the stack
                if (neighbors.Count > 0)
                {
                    Cell newCell = FindCell(RandomNeighbor(neighbors).Id);
                    DestroyWall(currentCell, newCell);
                    cellStack.Push(currentCell);
                    currentCell = newCell;
                    visitedCells++;
                }
                //If there are not any possible cells, we return on previous cell
                else
                {
                    currentCell = cellStack.Pop();
                }
            }
            return CellList;
        }

    /// <summary>
    /// Allow to pick a random Cell between every member of a list
    /// </summary>
    /// <param name="neighbors"></param>
    /// <returns></returns>
    public Cell RandomNeighbor(List<Cell> neighbors)
    {
        int total = neighbors.Count;
        int randomIndex = rnd.Next(0, total);

        Cell randomCell = neighbors[randomIndex];

        return randomCell;
    }

    /// <summary>
    /// Allow to find a specific Cell with its ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Cell FindCell(int id)
    {
        var foundCell = CellList.FirstOrDefault(Cell => Cell.Id == id);
        return foundCell;
    }

    /// <summary>
    /// Find every neighbors of Cell
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    public List<Cell> FindNeighborCells(Cell cell)
    {
        List<Cell> neighbors = new List<Cell>();
        List<Cell> finalNeighbors = new List<Cell>();

        //Cells on the far left column
        if (cell.Id % Width == 1)
        {
            if (cell.Id == 1)
            {
                Cell rightCell = FindCell(2);
                Cell bottomCell = FindCell(Width + 1);
                neighbors.Add(rightCell);
                neighbors.Add(bottomCell);
            }

            else if (cell.Id == (((Width * Height) - Width) + 1))
            {
                neighbors.Add(FindCell(cell.Id - Width));
                neighbors.Add(FindCell(cell.Id + 1));
            }
            else
            {
                Cell topCell = FindCell(cell.Id - Width);
                Cell rightCell = FindCell(cell.Id + 1);
                Cell bottomCell = FindCell(cell.Id + Width);
                neighbors.Add(topCell);
                neighbors.Add(rightCell);
                neighbors.Add(bottomCell);
            }
        }
        //Cells on the far right column
        else if (cell.Id % Width == 0)
        {
            if (cell.Id == Width)
            {
                neighbors.Add(FindCell(Width - 1));
                neighbors.Add(FindCell(2 * Width));
            }
            else if (cell.Id == Width * Height)
            {
                neighbors.Add(FindCell((Width * Height) - Width));
                neighbors.Add(FindCell((Width * Height) - 1));
            }
            else
            {
                Cell topCell = FindCell(cell.Id - Width);
                Cell leftCell = FindCell(cell.Id - 1);
                Cell bottomCell = FindCell(cell.Id + Width);
                neighbors.Add(topCell);
                neighbors.Add(leftCell);
                neighbors.Add(bottomCell);
            }
        }

        //Cells on the first row
        else if (cell.Id <= Width)
        {
            Cell leftCell = FindCell(cell.Id - 1);
            Cell rightCell = FindCell(cell.Id + 1);
            Cell bottomCell = FindCell(cell.Id + Width);
            neighbors.Add(bottomCell);
            neighbors.Add(leftCell);
            neighbors.Add(rightCell);
        }
        //Cells on the last row
        else if (cell.Id >= ((Width * Height) - Width) + 1)
        {
            Cell rightCell = FindCell(cell.Id + 1);
            Cell leftCell = FindCell(cell.Id - 1);
            Cell topCell = FindCell(cell.Id - Width);
            neighbors.Add(topCell);
            neighbors.Add(rightCell);
            neighbors.Add(leftCell);
        }
        //Other cells which are not on an edge
        else
        {
            Cell topCell = FindCell(cell.Id - Width);
            Cell bottomCell = FindCell(cell.Id + Width);
            Cell rightCell = FindCell(cell.Id + 1);
            Cell leftCell = FindCell(cell.Id - 1);
            neighbors.Add(bottomCell);
            neighbors.Add(topCell);
            neighbors.Add(rightCell);
            neighbors.Add(leftCell);
        }

        //On verifie si les cases ont tous les murs de levés
        foreach (Cell neighbor in neighbors)
        {
            if (neighbor.CheckWalls())
            {
                finalNeighbors.Add(neighbor);
            }
        }

        return finalNeighbors;
    }

    /// <summary>
    /// Destroy walls between two Cells
    /// </summary>
    /// <param name="firstCell"></param>
    /// <param name="secondCell"></param>
    public void DestroyWall(Cell firstCell, Cell secondCell)
    {
        //If firstCell on top of secondCell
        if (firstCell.Id == ((secondCell.Id) - Width))
        {
            firstCell.SouthWall = false;
            secondCell.NorthWall = false;
        }
        //If firstCell under secondCell
        else if (firstCell.Id == ((secondCell.Id) + Width))
        {
            firstCell.NorthWall = false;
            secondCell.SouthWall = false;
        }
        //If firstCell on the left of secondCell
        else if (firstCell.Id == ((secondCell.Id) - 1))
        {
            firstCell.EastWall = false;
            secondCell.WestWall = false;
        }
        //If firstCell on the right of secondCell
        else if (firstCell.Id == ((secondCell.Id) + 1))
        {
            firstCell.WestWall = false;
            secondCell.EastWall = false;
        }

    }

    /// <summary>
    /// Solve a previously generated maze
    /// </summary>
    /// <returns></returns>
    public List<Cell> Solve()
    {
        Stack<Cell> solvingPath = new Stack<Cell>();
        int randomStartId = rnd.Next(1, CellList.Count);
        Cell startCell = FindCell(randomStartId);
        int randomEndId = rnd.Next(1, CellList.Count);

        Cell endCell = FindCell(randomEndId);
        Cell currentCell = startCell;
        currentCell.Visited = true;

        while(currentCell != endCell)
        {
            List<Cell> exitCells = FindExitCells(currentCell);

            //If there's a possible cell to advance
            if (exitCells.Count > 0)
            {
                Cell newCell = FindNewCell(currentCell);
                solvingPath.Push(currentCell);
                currentCell = newCell;
                currentCell.Visited = true;
            }
            //Else we come back on the previous Cell
            else
            {
                currentCell = solvingPath.Pop();
            }
        }
        solvingPath.Push(endCell);
        return solvingPath.ToList();
    }

    /// <summary>
    /// Allow to find every possible cells where you can advance when solving the maze
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    public List<Cell> FindExitCells(Cell cell)
    {
        Cell currentCell = cell;
        List<Cell> exitCells = new List<Cell>();
        List<Cell> cellList = this.CellList;

        if (cell == null)
        {
            return exitCells;
        }

        if (cell.EastWall == false)
        {
            exitCells.Add(FindCell((cell.Id)+1));
        }

        if (cell.WestWall == false)
        {
            exitCells.Add(FindCell((cell.Id) - 1));
        }

        if (cell.NorthWall == false)
        {
            exitCells.Add(FindCell((cell.Id) - this.Width));
        }
        if (cell.SouthWall == false)
        {
            exitCells.Add(FindCell((cell.Id) + this.Width));
        }

        List<Cell> verifiedExitCells = new List<Cell>();

        foreach (Cell checkCell in exitCells)
        {
            if (checkCell.Visited == false)
            {
                verifiedExitCells.Add(checkCell);
            }
        }

        return verifiedExitCells;
    }

    /// <summary>
    /// Returns a Cell which will progress the solving
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    public Cell FindNewCell(Cell cell)
    {
        List<Cell> PossibleCells = new List<Cell>();
        List<Cell> CellList = FindExitCells(cell);
        foreach (Cell CurrentCell in CellList)
        {
            if (CurrentCell.Visited == false)
            {
                PossibleCells.Add(CurrentCell);
            }
        }

        if (PossibleCells.Count == 0)
        {
            return null;
        }

        Cell randomizedCell = PossibleCells[rnd.Next(0, PossibleCells.Count - 1)];
        return randomizedCell;
    }

    /// <summary>
    /// Draws the maze when concerned button has been clicked
    /// </summary>
    /// <param name="g"></param>
    public void DrawMaze(Graphics g)
    {
        Maze maze = this;
        System.Drawing.Graphics graphicsObj;
        graphicsObj = g;

        Pen cellPen = new Pen(System.Drawing.Color.Black, 1);

        int taille = 15;

        foreach (Cell Cell in maze.CellList)
        {
            {
                if (Cell.EastWall == true)
                {
                    graphicsObj.DrawLine(cellPen, Cell.Column * taille + taille, Cell.Row * taille,
                        Cell.Column * taille + taille, Cell.Row * taille + taille);
                }

                if (Cell.WestWall == true)
                {
                    graphicsObj.DrawLine(cellPen, Cell.Column * taille, Cell.Row * taille, Cell.Column * taille,
                        Cell.Row * taille + taille);
                }

                if (Cell.NorthWall == true)
                {
                    graphicsObj.DrawLine(cellPen, Cell.Column * taille, Cell.Row * taille, Cell.Column * taille + taille,
                        Cell.Row * taille);
                }

                if (Cell.SouthWall == true)
                {
                    graphicsObj.DrawLine(cellPen, Cell.Column * taille, Cell.Row * taille + taille,
                        Cell.Column * taille + taille, Cell.Row * taille + taille);
                }

                if (Cell.SouthWall == true && Cell.NorthWall == true && Cell.WestWall == true &&
                    Cell.EastWall == true)
                {
                    graphicsObj.DrawLine(cellPen, Cell.Column * taille, Cell.Row * taille + taille,
                        Cell.Column * taille + taille, Cell.Row * taille - 4 + taille - 4);
                }
            }
        }
    }

    /// <summary>
    /// Draws the solution of the maze 
    /// </summary>
    /// <param name="g"></param>
    public void DrawSolution(Graphics g)
    {
        Maze maze = this;
        int taille = 15;

        System.Drawing.Graphics graphicsObj;
        graphicsObj = g;

        List<Cell> solvingList = maze.Solve();
        Cell start = solvingList.First();
        Cell finish = solvingList.Last();
        SolidBrush startBrush = new SolidBrush(System.Drawing.Color.Green);
        SolidBrush finishBrush = new SolidBrush(System.Drawing.Color.Red);

        Rectangle StartRect = new Rectangle(start.Column * taille + 2, start.Row * taille + 2, taille - 4, taille - 4);
        Rectangle FinishRect = new Rectangle(finish.Column * taille + 2, finish.Row * taille + 2, taille - 4, taille - 4);

        Pen cellPen = new Pen(System.Drawing.Color.Black, 1);

        for (int i = 0; i < solvingList.Count; i++)
        {
            Cell Cell = solvingList[i];

            Rectangle CellPath = new Rectangle(Cell.Column * taille + 6, Cell.Row * taille + 6, taille - 12, taille - 12);
            graphicsObj.DrawRectangle(cellPen, CellPath);
        }
        graphicsObj.FillRectangle(startBrush, StartRect);
        graphicsObj.FillRectangle(finishBrush, FinishRect);
    }
}
