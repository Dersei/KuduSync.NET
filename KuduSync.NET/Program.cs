using CommandLine;
using System;
using System.Configuration;
using System.Diagnostics;

namespace KuduSync.NET
{
    class Program
    {
        static int Main(string[] args)
        {
            var stopwatch = Stopwatch.StartNew();
            var kuduSyncOptions = new KuduSyncOptions();
            int exitCode = 0;
            Console.WriteLine(ConfigurationManager.AppSettings["KuduSyncDataDirectory"]);
            try
            {
                Parser.Default.ParseArguments<KuduSyncOptions>(args).WithParsed(
                    o =>
                    {
                        using (var logger = GetLogger(o))
                        {
                            new KuduSync(o, logger).Run();
                        }
                    }).WithNotParsed(
                    errors =>
                    {
                        foreach (var error in errors)
                        {
                            Console.Error.WriteLine(error);
                        }
                        exitCode = 1;
                    });
                if (exitCode == 1) return exitCode;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error: " + ex.Message);
                exitCode = 1;
            }

            stopwatch.Stop();

            if (kuduSyncOptions.Perf)
            {
                Console.WriteLine("Time " + stopwatch.ElapsedMilliseconds);
            }

            return exitCode;
        }

        private static Logger GetLogger(KuduSyncOptions kuduSyncOptions)
        {
            int maxLogLines;

            if (kuduSyncOptions.Quiet)
            {
                maxLogLines = -1;
            }
            else if (kuduSyncOptions.Verbose != null)
            {
                maxLogLines = kuduSyncOptions.Verbose.Value;
            }
            else
            {
                maxLogLines = 0;
            }

            return new Logger(maxLogLines);
        }
    }
}
