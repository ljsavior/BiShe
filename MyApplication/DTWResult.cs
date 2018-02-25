using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApplication.Data
{
    class DTWResult
    {
        public double SumSimilarity { get; }

        public double AvgSimilarity { get; }

        public int PathLength { get; private set; }

        public double[,] DataMatrix { get; }

        public char[,] PathMatrix { get; }

        public List<int[]> Map { get; private set; }

        public String PathStr { get; private set; }

        public DTWResult(double[,] dataMatrix, char[,] pathMatrix)
        {
            this.DataMatrix = dataMatrix;
            this.PathMatrix = pathMatrix;

            findPath();
            this.SumSimilarity = dataMatrix[dataMatrix.GetLength(0) - 1, dataMatrix.GetLength(1) - 1];
            this.AvgSimilarity = this.SumSimilarity / this.PathLength;
        }

        private void findPath()
        {
            int row = PathMatrix.GetLength(0), col = PathMatrix.GetLength(1);
            Stack<int[]> stack = new Stack<int[]>();
            char[,] path = new char[row, col];

            int length = 1;
            for(int i = row - 1, j = col - 1; i > 0 || j > 0; length++)
            {
                char c = PathMatrix[i,j];
                stack.Push(new int[]{i, j});
                path[i, j] = c;
                if (c == '←')
                {
                    j--;
                } else if(c == '↑')
                {
                    i--;
                } else if(c == '↖')
                {
                    i--;
                    j--;
                }
            }
            stack.Push(new int[] { 0, 0 });
            path[0, 0] = 'o';

            this.PathLength = length;

            this.Map = new List<int[]>();
            while(stack.Count > 0)
            {
                Map.Add(stack.Pop());
            }

            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < row; i++)
            {
                for(int j = 0; j < col; j++)
                {
                    sb.Append(path[i, j] == '\0' ? ' ' : path[i, j]);
                }
                sb.Append('\n');
            }
            this.PathStr = sb.ToString();
        }
    }
}
