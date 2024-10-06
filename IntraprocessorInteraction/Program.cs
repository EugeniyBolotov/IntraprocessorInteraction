using System.Diagnostics;

internal class Program
{
    private static void Main(string[] args)
    {
        int[] arraySizes = { 100000, 1000000, 10000000 };
        foreach (int i in arraySizes)
        {
            Stopwatch stopwatch = new Stopwatch();
            int[] array = CreateRandomIntArray(i);

            Console.WriteLine($"размер массива: {i}");

            Console.WriteLine("\n\n\nПоследовательный подсчет элементов");
            stopwatch.Start();
            long sum = CasualSum(array);
            stopwatch.Stop();
            Console.WriteLine($"Сумма элементов массива: {sum};  Время подсчета: {stopwatch.ElapsedMilliseconds} мс");

            Console.WriteLine("\n\n\nПараллельный подсчет с помощью Linq");
            stopwatch.Restart();
            sum = ParallelLinqSum(array);
            stopwatch.Stop();
            Console.WriteLine($"Сумма элементов массива: {sum};  Время подсчета: {stopwatch.ElapsedMilliseconds} мс");

            Console.WriteLine("\n\n\nПараллельный подсчет с использованием Thread");
            stopwatch.Restart();
            sum = ParallelSum(array);
            stopwatch.Stop();
            Console.WriteLine($"Сумма элементов массива: {sum};  Время подсчета: {stopwatch.ElapsedMilliseconds} мс");
        }
    }

    static int[] CreateRandomIntArray(int size)
    {
        Random rnd = new Random();
        int[] array = new int[size];
        for (int i = 0; i < size; i++)
        {
            array[i] = rnd.Next(-1000, 1000);
        }
        return array;
    }
    static long CasualSum(int[] array)
    {
        long sum = 0;
        for (var i = 0; i < array.Length; i++)
        {
            sum += array[i];
        }
        return sum;
    }

    static long ParallelLinqSum(int[] array)
    {
        return array.AsParallel().Sum();
    }


    static int ParallelSum(int[] array)
    {
        int partialSum = 0;
        List<Thread> threads = new List<Thread>();
        int numThreads = Environment.ProcessorCount;

        int chunkSize = array.Length / numThreads;
        for (int i = 0; i < numThreads; i++)
        {
            int start = i * chunkSize;
            int end = (i == numThreads - 1) ? array.Length : (i + 1) * chunkSize;
            Thread thread = new Thread(() =>
            {
                int threadSum = 0;
                for (int j = start; j < end; j++)
                {
                    threadSum += array[j];
                }
                Interlocked.Add(ref partialSum, threadSum);
            });
            thread.Start();
            threads.Add(thread);
        }

        foreach (Thread thread in threads)
        {
            thread.Join();
        }

        return partialSum;
    }
}