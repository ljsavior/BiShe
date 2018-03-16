using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApplication.Utils
{
    class DTWUtil
    {
        public static Data.DTWResult DTW(double[,] similarityMatrix)
        {
            if (similarityMatrix == null)
            {
                throw new Exception("param is null.");
            }

            int row = similarityMatrix.GetLength(0);
            int col = similarityMatrix.GetLength(1);

            if (row == 0 || col == 0)
            {
                throw new Exception("row or col is 0.");
            }

            char[,] pathMatrix = new char[row, col];

            double[,] dp = new double[row, col];
            dp[0, 0] = similarityMatrix[0, 0];
            for (int i = 1; i < row; i++)
            {
                dp[i, 0] = dp[i - 1, 0] + similarityMatrix[i, 0];
                pathMatrix[i, 0] = '↑';
            }
            for (int j = 1; j < col; j++)
            {
                dp[0, j] = dp[0, j - 1] + similarityMatrix[0, j];
                pathMatrix[0, j] = '←';
            }
            for (int i = 1; i < row; i++)
            {
                for (int j = 1; j < col; j++)
                {
                    if (dp[i - 1, j - 1] <= dp[i - 1, j] && dp[i - 1, j - 1] <= dp[i, j - 1])
                    {
                        dp[i, j] = dp[i - 1, j - 1] + similarityMatrix[i, j];
                        pathMatrix[i, j] = '↖';
                    }
                    else if (dp[i - 1, j] <= dp[i - 1, j - 1] && dp[i - 1, j] <= dp[i, j - 1])
                    {
                        dp[i, j] = dp[i - 1, j] + similarityMatrix[i, j];
                        pathMatrix[i, j] = '↑';
                    }
                    else
                    {
                        dp[i, j] = dp[i, j - 1] + similarityMatrix[i, j];
                        pathMatrix[i, j] = '←';
                    }
                }
            }

            return new Data.DTWResult(dp, pathMatrix);
        }
    }
}
