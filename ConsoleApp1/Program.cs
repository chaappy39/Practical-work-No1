using System;
using System.Diagnostics;
using System.Threading.Tasks;

class Program
{
    static void Main()
    {
        int N = 100000000;  // Большое число для демонстрации разницы во времени

        // Последовательная версия
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        long sumSequential = 0;
        for (int i = 1; i <= N; i++)
        {
            sumSequential += i * i;
        }

        stopwatch.Stop();
        Console.WriteLine($"Последовательное вычисление: Сумма = {sumSequential}, Время = {stopwatch.ElapsedMilliseconds} мс");

        // Параллельная версия с блокировкой (lock)
        stopwatch.Restart();

        long sumParallel = 0;
        object lockObject = new object();  // Объект блокировки для синхронизации доступа к общей переменной

        Parallel.For(1, N + 1, i =>
        {
            long square = i * i;
            lock (lockObject)
            {
                sumParallel += square;
            }
        });

        stopwatch.Stop();
        Console.WriteLine($"Параллельное вычисление (с lock): Сумма = {sumParallel}, Время = {stopwatch.ElapsedMilliseconds} мс");

        // Параллельная версия без использования lock (с локальными переменными)
        stopwatch.Restart();

        long sumParallelNoLock = 0;

        Parallel.For(1, N + 1, () => 0L, (i, state, localSum) =>
        {
            return localSum + i * i;  // Локальная переменная для каждой задачи
        },
        localSum =>
        {
            lock (lockObject)
            {
                sumParallelNoLock += localSum;  // Сложение локальных результатов в общей переменной
            }
        });

        stopwatch.Stop();
        Console.WriteLine($"Параллельное вычисление (без lock внутри цикла): Сумма = {sumParallelNoLock}, Время = {stopwatch.ElapsedMilliseconds} мс");
    }
}
