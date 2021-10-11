# ConsoleProgressBar
Simple progress bar for C# .NET console applications, based on https://gist.github.com/DanielSWolf/0ab6a96899cc5377bf54

The progress bar operates on a background thread, updating the console 8 times per second regardless of how many times Report() is called. 

Basic usage:
```
static void Main(string[] args)
{
    using (var progressBar = new ConsoleProgressBar())
    {
        Console.Write("Performing task... ");

        for (int i = 0; i < 500; i++)
        {
            progressBar.Report((i + 1) / 500d);
            Thread.Sleep(10);
        }
    }
    Console.WriteLine("");
}
```
