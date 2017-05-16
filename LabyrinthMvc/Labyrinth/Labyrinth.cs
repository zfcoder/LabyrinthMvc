using System.Collections;
using System.Drawing;
using System.IO;
using System.Text;

namespace LabyrinthMvc.Labyrinth
{
    public delegate void Del();

    internal class Labyrinth
    {
        int[,] _map;
        int _w, _h;
        int _ax, _ay, _bx, _by;
        const int Blank = -2;         // свободная непомеченная ячейка
        int _minLength = -1;
        int[] _px, _py;
        ArrayList _path;

        public int Length { get { return _minLength; } }
        public int[,] Map { get { return _map; } }
        public int Width { get { return _w; } }
        public int Height { get { return _h; } }
        public Point A { get { return new Point(_ax, _ay); } }
        public Point B { get { return new Point(_bx, _by); } }
        public ArrayList Path { get { return _path; } }
        public string PathAsText { get; private set; }


        public Labyrinth(string filePath)
        {
            var s = File.OpenText(filePath);
            string read;

            var rowCount = 0;
            while ((read = s.ReadLine()) != null)
            {
                _w = read.Length;
                rowCount++;
            }
            _h = rowCount;

            s.DiscardBufferedData();
            s.BaseStream.Seek(0, SeekOrigin.Begin);


            _path = new ArrayList();
            _map = new int[_h, _w];
            _px = new int[_w * _h];
            _py = new int[_w * _h];
            _ax = _ay = _bx = _by = -1;

            for (var i = 0; i < _h; i++)
                for (var j = 0; j < _w; j++)
                    _map[i, j] = -2;

            for (var i = 0; i < _h; i++)
                if ((read = s.ReadLine()) != null)
                    for (var j = 0; j < _w; j++)
                    {
                        if (read[j] == '1')
                            _map[i, j] = -1;
                    }
            s.Close();
        }

        public Labyrinth(int w, int h)
        {
            _w = w;
            _h = h;
            _path = new ArrayList();
            _map = new int[h, w];
            _px = new int[w * h];
            _py = new int[w * h];
            _ax = _ay = _bx = _by = -1;
            for (var i = 0; i < h; i++)
                for (var j = 0; j < w; j++)
                    _map[i, j] = -2;
        }

        public void SetBlockType(int x, int y, int type)
        {
            if (x >= 0 && x < _w && y >= 0 && y < _h)
            {
                if (type == -1 || type == -2)
                {
                    _map[y, x] = type;
                    if (x == _ax && y == _ay)
                        _ax = _ay = -1;
                    if (x == _bx && y == _by)
                        _bx = _by = -1;
                }
                if (type == 1)
                {
                    _ax = x;
                    _ay = y;
                }
                if (type == 2)
                {
                    _bx = x;
                    _by = y;
                }
            }
        }

        public bool Find()
        {
            return Find(_ax, _ay, _bx, _by);
        }

        private bool Find(int ax, int ay, int bx, int by)
        {
            _path.Clear();
            _minLength = -1;
            if (ax != -1 && ay != -1 && bx != -1 && by != -1)
            {
                int[] dx = { 1, 0, -1, 0 };   // смещения, соответствующие соседям ячейки
                int[] dy = { 0, 1, 0, -1 };   // справа, снизу, слева и сверху
                int d, x, y, k;
                bool stop;

                for (var i = 0; i < _h; i++)
                    for (var j = 0; j < _w; j++)
                        if (_map[i, j] != -1)
                            _map[i, j] = -2;

                // распространение волны
                d = 0;
                _map[ay, ax] = 0;            // стартовая ячейка помечена 0
                do
                {
                    stop = true;               // предполагаем, что все свободные клетки уже помечены
                    for (y = 0; y < _h; ++y)
                        for (x = 0; x < _w; ++x)
                            if (_map[y, x] == d)                         // ячейка (x, y) помечена числом d
                            {
                                for (k = 0; k < 4; ++k)                    // проходим по всем непомеченным соседям
                                    if ((y + dy[k] >= 0 && y + dy[k] < _h) && (x + dx[k] >= 0 && x + dx[k] < _w))
                                        if (_map[y + dy[k], x + dx[k]] == Blank)
                                        {
                                            stop = false;                            // найдены непомеченные клетки
                                            _map[y + dy[k], x + dx[k]] = d + 1;      // распространяем волну
                                        }
                            }
                    d++;
                } while (!stop);

                if (_map[by, bx] == Blank) { _minLength = -1; return false; }  // путь не найден

                // восстановление пути
                _minLength = _map[by, bx];  // длина кратчайшего пути из (ax, ay) в (bx, by)
                x = bx;
                y = by;
                d = _minLength;

                while (d > 0)
                {
                    _path.Add(new Point(x, y)); // записываем ячейку (x, y) в путь
                    d--;
                    for (k = 0; k < 4; ++k)
                        if ((y + dy[k] >= 0 && y + dy[k] < _h) && (x + dx[k] >= 0 && x + dx[k] < _w))
                            if (_map[y + dy[k], x + dx[k]] == d)
                            {
                                x = x + dx[k];
                                y = y + dy[k];  // переходим в ячейку, которая на 1 ближе к старту
                                break;
                            }
                }
                _path.Add(new Point(ax, ay));   // теперь px[0..len] и py[0..len] - координаты ячеек пути

                var str = new StringBuilder();
                _path.Reverse();
                for (var i = 0; i < _path.Count - 1; i++)
                {
                    var point1 = (Point)_path[i];
                    var point2 = (Point)_path[i + 1];
                    if (point1.X - point2.X == -1)
                    {
                        str.Append(1);
                    }
                    if (point1.X - point2.X == 1)
                    {
                        str.Append(3);
                    }
                    if (point1.Y - point2.Y == -1)
                    {
                        str.Append(2);
                    }
                    if (point1.Y - point2.Y == 1)
                    {
                        str.Append(0);
                    }
                }
                _path.Reverse();
                PathAsText = str.ToString();
                return true;
            }
            return false;
        }
    }
}