using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace coursework_15
{
    class Game
    {
        int[,] map; //доска
        int size; //размер поля
        public int space_x, space_y; //координаты нуля
        int[] dx = new int[] { 0, -1, 0, 1 };//смещения пустой клетки
        int[] dy = new int[] { 1, 0, -1, 0 };
        char[] move_desc = new char[] { 'D', 'L', 'U', 'R' };// Возможные ходы
        int[] opposite_move = new int[] { 2, 3, 0, 1 };// Противоположные ходы (2, 3, 0, 1)                                                       
        const int infinity = 10000;
        //int x0, // координаты пустой клетки
        //    y0; // координаты пустой клетки
        int[] goalX = new int[16]; //goalX[i] - координата x i-й пятнашки, ...
        int[] goalY = new int[16];
        int[,] boardGoal = new int[4, 4]; // доска целевого состояния

        int minPrevIteration; //deepness глубина
                              //string resultString результат
                              //минимум стоимости среди нерассмотренных узлов
        static Random rand = new Random();

        //конструктор поля
        public Game(int size)
        {
            if (size < 2) size = 2;
            if (size > 5) size = 5;
            this.size = size;
            map = new int[size, size];
        }

        //заполняем поле костяшками от 1 до 15
        public void start()
        {
            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                    map[x, y] = coordstopos(x, y) + 1;
            space_x = size - 1;
            space_y = size - 1;
            map[space_x, space_y] = 0;
        }

        //перемещение костяшки
        public void shift(int position)
        {
            int x, y;
            postocoord(position, out x, out y);
            if (Math.Abs(space_x - x) + Math.Abs(space_y - y) != 1)
                return;
            map[space_x, space_y] = map[x, y];
            map[x, y] = 0;
            space_x = x;
            space_y = y;
        }

        //случайный ход
        public void shift_random()
        {
            int a = rand.Next(0, 4);
            int x = space_x;
            int y = space_y;

            switch (a)
            {
                case 0: x--; break;
                case 1: x++; break;
                case 2: y--; break;
                case 3: y++; break;
            }
            shift(coordstopos(x, y));
        }


        // проверка на победу
        public bool check_num()
        {
            if (!(space_x == size - 1 && space_y == size - 1))
                return false;
            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                    if (!(x == size - 1 && y == size - 1))
                        if (map[x, y] != coordstopos(x, y) + 1)
                            return false;
            return true;
        }

        // определяем значение для фишки по номеру кнопки
        public int get_num(int position)
        {
            int x, y;
            postocoord(position, out x, out y);
            if (x < 0 || x >= size) return 0;
            if (y < 0 || y >= size) return 0;
            return map[x, y];
        }

        // находим номер фишки по координатам
        public int coordstopos(int x, int y)
        {
            if (x < 0) x = 0;
            if (x > size - 1) x = size - 1;
            if (y < 0) y = 0;
            if (y > size - 1) y = size - 1;
            return x * size + y;
        }

        // находим координаты фишки по номеру
        private void postocoord(int position, out int x, out int y)
        {
            if (position < 0) position = 0;
            if (position > size * size - 1) position = size * size - 1;
            x = position / size;
            y = position % size;
        }

        #region Алгоритм IDA* и поиск в глубину для нахождения кратчайшего решения

        //инициализирует целевое состояние
        public void initGoalArrays()
        {
            for (int i = 0; i < 15; i++)
            {
                goalX[i + 1] = i % 4;
                goalY[i + 1] = i / 4;
                goalX[0] = 4;
                goalY[0] = 4;
            }
        }

        //меняет местами две пятнашки
        private void swap(int y1, int x1, int y2, int x2)
        {
            int value1, value2;
            value1 = map[y1, x1];
            value2 = map[y2, x2];
            map[y1, x1] = value2;
            map[y2, x2] = value1;
        }

        //эвристическая оценочная функция Манхеттеновское расстояние
        private void estimate(out int manhattan)
        {
            int i, j, value, m;
            manhattan = 0;
            for (i = 0; i < 4; i++)
            {
                for (j = 0; j < 4; j++)
                {
                    value = map[i, j];
                    if (value > 0)
                    {
                        m = Math.Abs(i - goalY[value]) + Math.Abs(j - goalX[value]);
                        manhattan = manhattan + m;
                    }
                }
            }
        }

        //поиск в глубину с обрезанием f=g+h < deepness
        private void recSearch(int g, int previousMove, int x0, int y0, int step, int deepness, out bool tf,
                        out string resultString)
        {
            int h;
            int manhattan;
            estimate(out manhattan);
            h = manhattan;
            tf = false;
            resultString = null;
            step = 0;
            // h = минимум ходов к цели
            if (h == 0)
            {
                tf = true;
                return; // если это цель - ура!
            }
            // если то, что мы прошли (g) + то, что нам как минимум осталось (h)
            // больше допустимой глубины - выход.
            int f;
            f = g + h;
            if (f > deepness)
            {
                //нaходим минимум стоимости среди "обрезаных" узлов
                if (minPrevIteration > f)
                    minPrevIteration = f;
                return;
            }
            int newx, newy;
            bool res;
            // делаем всевозможные ходы
            for (int i = 0; i < 4; i++)
            {
                if (opposite_move[i] != previousMove)
                {
                    // новые координаты пустой клетки
                    newx = x0 + dx[i];
                    newy = y0 + dy[i];
                    if ((newy <= 3) && (newy >= 0) && (newx <= 3) && (newx >= 0))
                    {
                        swap(y0, x0, newy, newx); // двигаем пустую клетку на новое место
                        recSearch(g + 1, i, newx, newy, step, deepness, out tf, out resultString); // рекурсивный поиск с новой позиции
                        res = tf;
                        swap(y0, x0, newy, newx); // возвращаем пустую клетку назад
                        if (res == true) //если было найдено решение
                        {
                            resultString = move_desc[i] + resultString; //записываем этот ход
                            step++;
                            tf = true;
                            return; // и выходим
                        }
                    }
                }
            }
            return; //цели не нашли
        }

        //итерация глубины и IDA*
        public bool idaStar(out string result)
        {
            bool res;
            int x0 = -1;
            int y0 = -1;
            bool tf;
            int step;
            string resultString;
            res = false;
            result = null;
            int manhattan;
            estimate(out manhattan);
            int deepness = manhattan; // начинаем с h для начального состояния
            while (deepness <= 70)
            {
                minPrevIteration = infinity; // инициализация для поиска минимума
                                             // поиск пустой клетки
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (map[i, j] == 0)
                        {
                            x0 = j;
                            y0 = i;
                        }
                    }
                }
                step = 0;
                recSearch(0, -1, x0, y0, step, deepness, out tf, out resultString);
                deepness = minPrevIteration;
                res = tf;
                if (res)
                {
                    result = resultString;
                    break;
                }
            }
            return res;
        }
        #endregion
    }
}
