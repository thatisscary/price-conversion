namespace purchasedb_deatabasedeply
{
    using DbUp;

    internal class Program
    {
        static void Main(string[] args)
        {
            string? connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__purchasedb");
            if(connectionString == null)
            {
                Console.WriteLine("Connection string not found in environment variables.");
                return;
            }
            EnsureDatabase.For.SqlDatabase(connectionString);
            DeployChanges.To.SqlDatabase(connectionString)
                .WithScriptsFromFileSystem("./scripts")
                .LogToConsole()
                .Build()
                .PerformUpgrade();
         
            
            

        }
    }
}
