namespace PatenteN.Importer {

    public static class Program {

        const string DBFile = "C:\\Users\\carettig\\Downloads\\Quiz\\Allegato.accdb";
        const string ConnectionString = $@"Driver={{Microsoft Access Driver (*.mdb, *.accdb)}}; Dbq={DBFile}; Uid = Admin; Pwd =; ";

        static void Main(string[] args) {

            var commands = new Commands(ConnectionString);

            Console.WriteLine("Import categories...");
            var count = commands.CopyCategories();
            Console.WriteLine($"Imported {count} categories");
            Console.WriteLine();

            Console.WriteLine("Import Quiz and Answers...");
            commands.CopyQuestionsTxt();
            Console.WriteLine("Done!");
        }
    }

}