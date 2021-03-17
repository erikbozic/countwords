using BenchmarkDotNet.Attributes;

namespace optimized
{
    [MemoryDiagnoser]
    public class Benchamrks
    {
        [Benchmark]
        public void CsharpOptimized1()
        {
            Implementations.CsharpOptimized1();
        }

        [Benchmark]
        public void CsharpOptimized2()
        {
            Implementations.CsharpOptimized2();
        }
    }
}

