using System;
using System.Threading.Tasks;

namespace dune_library.Utils
{
    internal class Wait_Until_Something
    {
        public static async Task<string> AwaitInput(int time, I_Input_Provider Input_Provider)
        {
            var taskCompletionSource = new TaskCompletionSource<string>();

            // Task to read input
            Console.WriteLine("Waiting for input...");
            _ = Task.Run(() =>
            {
                string input = Input_Provider.GetInputAsync().Result;
                taskCompletionSource.TrySetResult(input);
            });

            // Task to delay for the specified time
            var delayTask = Task.Delay(time);

            // Await either input or timeout
            var completedTask = await Task.WhenAny(taskCompletionSource.Task, delayTask);

            if (completedTask == taskCompletionSource.Task)
            {
                string input = await taskCompletionSource.Task;
                return input;
            }
            else
            {
                return "Failure";
            }
        }
    }
}
