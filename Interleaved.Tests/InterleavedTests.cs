using System;
using System.Threading.Tasks;
using Xunit;

namespace BAMCIS.Parallel.Interleaved.Tests
{
    public class InterleavedTests
    {
        #region Async

        [Fact]
        public async Task TestInterleaved()
        {
            // ARRANGE
            Task<int>[] tasks = new[] {
                Task.Delay(3000).ContinueWith(_ => 3),
                Task.Delay(1000).ContinueWith(_ => 1),
                Task.Delay(2000).ContinueWith(_ => 2),
                Task.Delay(5000).ContinueWith(_ => 5),
                Task.Delay(4000).ContinueWith(_ => 4),
            };

            int[] results = new int[tasks.Length];
            int counter = 0;

            // ACT
            foreach (Task<int> completedTask in tasks.Interleaved())
            {
                int result = await completedTask;
                Console.WriteLine($"{DateTime.Now.ToString()}: {results}");
                results[counter++] = result;
            }

            // ASSERT
            for (int i = 1; i <= results.Length; i++)
            {
                Assert.Equal(i, results[i - 1]);
            }
        }

        #endregion
    }
}
