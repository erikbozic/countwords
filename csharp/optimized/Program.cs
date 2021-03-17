using BenchmarkDotNet.Running;

namespace optimized
{
    public class Program
    {
        static void Main()
        {
            var summary = BenchmarkRunner.Run<Benchamrks>();
        }
    }
}
