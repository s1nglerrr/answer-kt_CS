using System;
using System.Diagnostics;
using System.Threading;

class Program
{
    static long totalSum = 0;
    static readonly object lockObj = new object();

    static void Main()
    {
        int[] numbers = GenerateRandomArray(1_000_000, 1, 1001);

        Stopwatch stopwatch = Stopwatch.StartNew();
        long singleThreadSum = CalculateSingleThreadSum(numbers);
        stopwatch.Stop();
        Console.WriteLine($"Однопоточная сумма: {singleThreadSum}");
        Console.WriteLine($"Время выполнения (1 поток): {stopwatch.ElapsedMilliseconds} мс");

        totalSum = 0;
        stopwatch.Restart();
        long multiThreadSum = CalculateMultiThreadSum(numbers, 4);
        stopwatch.Stop();
        Console.WriteLine($"Многопоточная сумма: {multiThreadSum}");
        Console.WriteLine($"Время выполнения (4 потока): {stopwatch.ElapsedMilliseconds} мс");

        Console.WriteLine($"Результаты совпадают: {singleThreadSum == multiThreadSum}");
    }

    static int[] GenerateRandomArray(int size, int min, int max)
    {
        Random random = new Random();
        int[] array = new int[size];
        for (int i = 0; i < size; i++)
            array[i] = random.Next(min, max);
        return array;
    }

    static long CalculateSingleThreadSum(int[] numbers)
    {
        long sum = 0;
        foreach (int number in numbers)
            sum += number;
        return sum;
    }

    static long CalculateMultiThreadSum(int[] numbers, int threadCount)
    {
        Thread[] threads = new Thread[threadCount];
        int chunkSize = numbers.Length / threadCount;

        for (int i = 0; i < threadCount; i++)
        {
            int start = i * chunkSize;
            int end = (i == threadCount - 1) ? numbers.Length : (i + 1) * chunkSize;

            threads[i] = new Thread(() => SumRange(numbers, start, end));
            threads[i].Start();
        }

        foreach (Thread t in threads)
            t.Join();

        return totalSum;
    }

    static void SumRange(int[] numbers, int start, int end)
    {
        long localSum = 0;
        for (int i = start; i < end; i++)
            localSum += numbers[i];
        
        lock (lockObj)
        {
            totalSum += localSum;
        }
    }
}
