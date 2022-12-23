using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwaitExercises.Core
{
    public class AsyncHelpers
    {
        public static async Task<string> GetStringWithRetriesAsync(HttpClient client, string url, int maxTries = 3, CancellationToken token = default)
        {
            // Create a method that will try to get a response from a given `url`, retrying `maxTries` number of times.
            // It should wait one second before the second try, and double the wait time before every successive retry
            // (so pauses before retries will be 1, 2, 4, 8, ... seconds).
            // * `maxTries` must be at least 2
            // * we retry if:
            //    * we get non-successful status code (outside of 200-299 range), or
            //    * HTTP call thrown an exception (like network connectivity or DNS issue)
            // * token should be able to cancel both HTTP call and the retry delay
            // * if all retries fails, the method should throw the exception of the last try
            // HINTS:
            // * `HttpClient.GetStringAsync` does not accept cancellation token (use `GetAsync` instead)
            // * you may use `EnsureSuccessStatusCode()` method

            if (maxTries < 2)
                throw new ArgumentException();

            // the method should be split here according to the guidelines
            string result = null; 
            for (var tryAttempt = 0; tryAttempt <= maxTries; tryAttempt++)
            {
                try
                {
                    if (tryAttempt != 0)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, tryAttempt - 1)), token);
                    }
                    var response = await client.GetAsync(url, token);
                    _ = response.EnsureSuccessStatusCode();
                    result = await response.Content.ReadAsStringAsync();
                    break;
                }
                catch
                {
                    if (tryAttempt == maxTries) throw;
                }
            }

            return result ?? throw new InvalidOperationException("This should never happen!");
        }
    }
}
