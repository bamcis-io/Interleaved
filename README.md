# BAMCIS Interleaved Task Processing

Provides an extension method to handle processing async tasks as they complete. This allows you to handle follow on actions when each task completes without needing to wait for all of them to finish first, or processing them out of order based on completion time.

## Table of Contents
- [Usage](#usage)
- [Revision History](#revision-history)

## Usage

Basic usage examples:

    Task<int>[] Tasks = new[] {
        Task.Delay(3000).ContinueWith(_ => 3),
        Task.Delay(1000).ContinueWith(_ => 1),
        Task.Delay(2000).ContinueWith(_ => 2),
        Task.Delay(5000).ContinueWith(_ => 5),
        Task.Delay(4000).ContinueWith(_ => 4),
    };

    foreach (Task<int> CompletedTask in Tasks.Interleaved())
    {
        int Result = await CompletedTask;
        Console.WriteLine($"{DateTime.Now.ToString()}: {Result}");
    }

If the start time is 4/23/2018 08:00:00 AM, this will print

    4/23/2018 08:00:01 AM: 1
    4/23/2018 08:00:02 AM: 2
    4/23/2018 08:00:03 AM: 3
    4/23/2018 08:00:04 AM: 4
    4/23/2018 08:00:05 AM: 5

Instead of printing:

    4/23/2018 08:00:03 AM: 3
    4/23/2018 08:00:03 AM: 1
    4/23/2018 08:00:03 AM: 2
    4/23/2018 08:00:05 AM: 5
    4/23/2018 08:00:05 AM: 4

This method was adapted from [here](https://blogs.msdn.microsoft.com/pfxteam/2012/08/02/processing-tasks-as-they-complete/ "processing-tasks-as-they-complete").
ctResponse Response = CopyOrMoveObjectMultipartAsync(Req, 16777216, true);

## Revision History

### 1.0.0
Initial release of the application.
