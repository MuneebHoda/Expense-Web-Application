using Serilog;
using Serilog.Sinks.MSSqlServer;
using System;
using System.Configuration;

public class LogService
{
    public static void ConfigureLogging()
    {
        try
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ExpenseAppDbContext"].ConnectionString;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.MSSqlServer(
                    connectionString: connectionString,
                    sinkOptions: new MSSqlServerSinkOptions
                    {
                        TableName = "Logs",
                        AutoCreateSqlTable = true,
                    }
                )
                .CreateLogger();

            Log.Information("Logger - Started Successfully");
        }
        catch (Exception ex) 
        {
            Console.WriteLine("Logger - Setup Failed: " + ex.Message);
        }
    }

    public static void ShutdownLogger()
    {
        try
        {
            Log.Information("Logger - Shutting Down");
            Log.CloseAndFlush();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Logger - Error during logger shutdown : " + ex.Message);
        }
    }
}