using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static void Main()
    {
        int[] numbers = GenerateRandomArray(100_000_000, 1, 1001);

        Stopwatch stopwatch = Stopwatch.StartNew();
        long singleThreadSum = CalculateSingleThreadSum(numbers);
        stopwatch.Stop();
        Console.WriteLine($"Однопоточная сумма: {singleThreadSum}");
        Console.WriteLine($"Время выполнения 1 поток: {stopwatch.ElapsedMilliseconds} мс");

        stopwatch.Restart();
        long multiThreadSum = CalculateMultiThreadSum(numbers, 4);
        stopwatch.Stop();
        Console.WriteLine($"Многопоточная сумма: {multiThreadSum}");
        Console.WriteLine($"Время выполнения 4 потока: {stopwatch.ElapsedMilliseconds} мс");
    }

    static int[] GenerateRandomArray(int size, int min, int max)
    {
        Random random = new Random();
        int[] array = new int[size];
        for (int i = 0; i < size; i++)
        {
            array[i] = random.Next(min, max);
        }
        return array;
    }

    static long CalculateSingleThreadSum(int[] numbers)
    {
        long sum = 0;
        foreach (int number in numbers)
        {
            sum += number;
        }
        return sum;
    }

    static long CalculateMultiThreadSum(int[] numbers, int threadCount)
    {
        long[] partialSums = new long[threadCount];
        int chunkSize = numbers.Length / threadCount;
        Task[] tasks = new Task[threadCount];

        for (int i = 0; i < threadCount; i++)
        {
            int threadIndex = i;
            int startIndex = i * chunkSize;
            int endIndex = (i == threadCount - 1) ? numbers.Length : (i + 1) * chunkSize;

            tasks[i] = Task.Run(() =>
            {
                long localSum = 0;
                for (int j = startIndex; j < endIndex; j++)
                {
                    localSum += numbers[j];
                }
                partialSums[threadIndex] = localSum;
            });
        }

        Task.WaitAll(tasks);

        long total = 0;
        for (int i = 0; i < threadCount; i++)
            total += partialSums[i];

        return total;
    }
}