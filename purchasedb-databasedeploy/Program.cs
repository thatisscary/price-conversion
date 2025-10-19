﻿namespace purchasedb_deatabasedeply
{
    using DbUp;

    internal class Program
    {
        static void Main(string[] args)
        {
            string connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__purchasedb");
            EnsureDatabase.For.SqlDatabase(connectionString);
            DeployChanges.To.SqlDatabase(connectionString)
                .WithScriptsFromFileSystem("./scripts")
                .LogToConsole()
                .Build()
                .PerformUpgrade();
         
            
            

        }
    }
}
