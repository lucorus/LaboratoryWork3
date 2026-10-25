using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LaboratoryWork3
{
    internal class Program
    {
        static void print(int[] arr, int n)
        {
            for(int i = 0; i<n; i++)
            {
                Console.Write($"{arr[i]} ");
            }
        }

        static void SortAbs(ref int[] arr, int n)
        {
            if (n < 2) 
                return;

            int mid = Math.Abs(arr[n/2]);
            int[] arr_more = new int[n];  // элементы, которые больше опорного
            int[] arr_equal = new int[n]; // элементы равные опорному
            int[] arr_less = new int[n];  // элементы, которые меньше опорного
            int cnt_more = 0, cnt_equal = 0, cnt_less = 0;
            for (int i = 0; i < n; i++)
            {
                if (Math.Abs(arr[i]) > mid)
                    arr_more[cnt_more++] = arr[i];
                else if (Math.Abs(arr[i]) == mid)
                    arr_equal[cnt_equal++] = arr[i];
                else
                    arr_less[cnt_less++] = arr[i];
            }

            SortAbs(ref arr_less, cnt_less);
            SortAbs(ref arr_more, cnt_more);

            Array.Resize(ref arr_less, cnt_less);
            Array.Resize(ref arr_equal, cnt_equal);
            Array.Resize(ref arr_more, cnt_more);
            arr = arr_less.Concat(arr_equal).Concat(arr_more).ToArray();
        }


        static int[] CreateArr(int n, int min = -100, int max = 101)
        {
            int[] arr = new int[n];
            Random rnd = new Random();
            for(int i = 0; i<n; i++)
            {
                arr[i] = rnd.Next(min, max);
            }
            return arr;
        }

        
        static void task1_14()
        {
            Console.WriteLine("Введите количество элементов в массиве:");
            int n = int.Parse(Console.ReadLine());
            int[] arr = new int[n];
            arr = CreateArr(n);
            Console.WriteLine($"Массив длинною {n} заполнен случайными значениями");
            Console.Write("arr = { ");
            print(arr, n);
            Console.Write("}\n\n");


            int ans1 = 0;  // количество нулей
            foreach (int el in arr)
                if (el == 0) ans1++;
            Console.WriteLine($"Количество нулевых элементов: {ans1}\n");


            int ans2 = 0;  // сумма элементов после минимального
            int min = 99999999;
            foreach (int el in arr) 
                if (el < min) min = el;
            for(int i = n-1;  i >= 0; i--)
            {
                if (arr[i] == min) break;
                ans2 += arr[i];
            }
            Console.WriteLine($"Сумма элементов после минимального равна {ans2}\n");


            SortAbs(ref arr, n);
            Console.WriteLine("Отсортировав массив по возрастанию модулей получаем:");
            Console.Write("arr = { ");
            print(arr, n);
            Console.Write("}");
        }

        //////////////////////////////////////////////////////////////////////////////////

        static int[,] CreateMatrix(int n, int m, int min = -100, int max = 101)
        {
            int[,] arr = new int[n, m];
            Random rnd = new Random();
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    arr[i, j] = rnd.Next(min, max);
                }
            }
            return arr;
        }


        static int[] CreateChain(int[,] arr, int top, int left, int bottom, int right, int len)
        {
            int[] chain = new int[len];
            int ind = 0;

            for (int j = left; j < right; j++)
                chain[ind++] = arr[top, j];
            for (int i2 = top; i2 < bottom; i2++)
                chain[ind++] = arr[i2, right];
            for (int j = right; j > left; j--)
                chain[ind++] = arr[bottom, j];
            for (int i2 = bottom; i2 > top; i2--)
                chain[ind++] = arr[i2, left];

            return chain;
        }

        // сдвигаем цепочку вправо на ring_k (разрезаем её на две части и склеиваем)
        static int[] MoveChain(int[] chain, int len, int ring_k)
        {
            int[] new_chain = new int[len];
            Array.Copy(chain, len - ring_k, new_chain, 0, ring_k);
            Array.Copy(chain, 0, new_chain, ring_k, len - ring_k);
            return new_chain;
        }

        // перезаписываем кольцо в матрице
        static void WriteChain(ref int[,] arr, int[] chain, int top, int left, int bottom, int right)
        {
            int ind = 0;

            for (int j = left; j < right; j++)
                arr[top, j] = chain[ind++];
            for (int i2 = top; i2 < bottom; i2++)
                arr[i2, right] = chain[ind++];
            for (int j = right; j > left; j--)
                arr[bottom, j] = chain[ind++];
            for (int i2 = bottom; i2 > top; i2--)
                arr[i2, left] = chain[ind++];
        }

        static void PrintMatrix(int[,] arr, int n, int m)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    Console.Write($"{arr[i, j]} ");
                }
                Console.WriteLine();
            }
        }


        static void task2_14()
        {
            Console.WriteLine("Введите число строк в матрице:");
            int n = int.Parse(Console.ReadLine());
            Console.WriteLine("Введите число столбцов в матрице:");
            int m = int.Parse(Console.ReadLine());
            int[,] arr = CreateMatrix(n, m, 0, 10);
            Console.WriteLine("Введите k:");
            int k = int.Parse(Console.ReadLine());

            Console.WriteLine("Случайно сгенерированная матрица имеет вид:");
            PrintMatrix(arr, n, m);

            int rings = Math.Min(m, n) / 2;
            for (int i = 0; i < rings; i++)
            {
                int rows = n - 2 * i;
                int cols = m - 2 * i;
                int len = 2 * (rows + cols) - 4;  // длина кольца
                int ring_k = k % len;   // фактический размер сдвига
                int top = i, left = i, bottom = n - 1 - i, right = m - 1 - i;

                int[] chain = CreateChain(arr, top, left, bottom, right, len);
                int[] new_chain = MoveChain(chain, len, ring_k);
                WriteChain(ref arr, new_chain, top, left, bottom, right);
            }

            Console.WriteLine("Получилась матрица:");
            PrintMatrix(arr, n, m);
        }

        ///////////////////////////////////////////////////////////////////
        
        static bool NotEvenDigitInNum(int n)
        {
            while(n > 0)
            {
                if (n % 2 == 0)
                    return false;  // в числе есть чётные цифры
                n = n / 10;
            }
            return true;
        }

        static bool AllDigitsDistinct(int n)
        {
            n = Math.Abs(n);
            bool[] seen = new bool[10];
            while (n > 0)
            {
                int d = n % 10;
                if (seen[d]) return false;
                seen[d] = true;
                n = n / 10;
            }
            return true;
        }

        static void task3_14()
        {
            Console.WriteLine("Введите n:");
            int n = int.Parse(Console.ReadLine());
            Console.WriteLine("Введите m:");
            int m = int.Parse(Console.ReadLine());

            int ans1 = 0;
            for(int i = n % 2 == 1?n:n+1 ; i <= m; i+=2)
            {
                if (NotEvenDigitInNum(i))
                    ans1++;
            }
            Console.WriteLine($"Чисел из нечётных цифр на промежутке [{n};{m}]: {ans1} штук");


            int ans2 = 0;
            for (int i = n; i <= m; i++)
            {
                if (AllDigitsDistinct(i))
                    ans2++;
            }
            Console.WriteLine($"Чисел из различных цифр на промежутке [{n};{m}]: {ans2} штук");
        }


        static void Main(string[] args)
        {
            while(true)
            {
                const int N = 3;

                Console.Clear();
                Console.WriteLine("Выберите задание:");
                for(int i = 0; i < N; i ++)
                {
                    Console.WriteLine($"{i+1}. Задание {i + 1}");
                }
                Console.WriteLine($"{N + 1}. Выход");
                int n = int.Parse(Console.ReadLine());

                switch (n)
                {
                    case 1:
                        task1_14();
                        Console.WriteLine("\n\nНажмите Enter для возврата в меню");
                        Console.ReadLine(); 
                        break;
                    case 2:
                        task2_14();
                        Console.WriteLine("\n\nНажмите Enter для возврата в меню");
                        Console.ReadLine(); 
                        break;
                    case 3:
                        task3_14();
                        Console.WriteLine("\n\nНажмите Enter для возврата в меню");
                        Console.ReadLine(); 
                        break;
                    default:
                        Console.WriteLine("Выполнение программы завершено"); 
                        return;
                }
            }
        }
    }
}
