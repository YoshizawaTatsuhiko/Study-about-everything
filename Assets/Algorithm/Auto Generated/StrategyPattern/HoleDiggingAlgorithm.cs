using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 日本語対応
public class HoleDiggingAlgorithm : Blueprint, IMazeStrategy
{
    /// <summary>通路拡張開始候補地点</summary>
    List<(int, int)> _startList = new List<(int, int)>();

    public string CreateBlueprint(int width, int height)
    {
        // 迷路の大きさが5未満だったら、エラーを出力する。
        if (width <= 0 || height <= 0) throw new System.ArgumentOutOfRangeException();
        // 縦(横)の長さが偶数だったら、奇数に変換する。
        width = width % 2 == 0 ? ++width : width;
        height = height % 2 == 0 ? ++height : height;

        // 迷路の情報を格納する。
        string[,] maze = new string[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // 迷路の外周を床で埋めておく。
                if (x * y == 0 || x == width - 1 || y == height - 1)
                {
                    maze[x, y] = "F";
                }
                // それ以外を壁で埋める。
                else
                {
                    maze[x, y] = "W";
                }
            }
        }
        DiggingPath(maze, (1, 1));

        // 通路を拡張し終えたら、迷路の外周を壁で囲む。
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (x * y == 0 || x == width - 1 || y == height - 1)
                {
                    maze[x, y] = "W";
                }
            }
        }
        return ArrayToString(maze);
    }

    /// <summary>通路を掘る</summary>
    private void DiggingPath(string[,] maze, (int, int) coodinate)
    {
        if (_startList.Count > 0) _startList.Remove(coodinate);

        int x = coodinate.Item1;
        int y = coodinate.Item2;

        while (true)
        {
            // 拡張できる方向を格納するリスト
            List<string> dirs = new List<string>();

            if (maze[x, y - 1] == "W" && maze[x, y - 2] == "W") dirs.Add("Up");
            if (maze[x, y + 1] == "W" && maze[x, y + 2] == "W") dirs.Add("Down");
            if (maze[x - 1, y] == "W" && maze[x - 2, y] == "W") dirs.Add("Left");
            if (maze[x + 1, y] == "W" && maze[x + 2, y] == "W") dirs.Add("Right");
            // 拡張できる方向がなくなったら、ループを抜ける。
            if (dirs.Count == 0) break;
            // 通路を設置する
            SetPath(maze, x, y);
            int dirsIndex = Random.Range(0, dirs.Count);

            try
            {
                switch (dirs[dirsIndex])
                {
                    case "Up":
                        SetPath(maze, x, --y);
                        SetPath(maze, x, --y);
                        break;
                    case "Down":
                        SetPath(maze, x, ++y);
                        SetPath(maze, x, ++y);
                        break;
                    case "Left":
                        SetPath(maze, --x, y);
                        SetPath(maze, --x, y);
                        break;
                    case "Right":
                        SetPath(maze, ++x, y);
                        SetPath(maze, ++x, y);
                        break;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        if (_startList.Count > 0)
        {
            int random = Random.Range(0, _startList.Count);
            DiggingPath(maze, _startList[random]);
        }
    }

    private void SetPath(string[,] maze, int x, int y)
    {
        maze[x, y] = "F";
        // x, yが共に奇数だったら、リストから削除する。
        if (x % 2 != 0 && y % 2 != 0)
        {
            _startList.Add((x, y));
        }
    }
}