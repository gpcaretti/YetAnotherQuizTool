using System.Data.Odbc;
using PatenteN.Core.Guids;
using PatenteN.Importer.Helpers;
using PatenteN.Importer.Models;

namespace PatenteN.Importer {

    internal class Commands {

        private readonly string _connectionString;
        private readonly IGuidGenerator _guidGenerator = new SequentialGuidGenerator();

        public Commands(string connectionString) {
            _connectionString = connectionString;

        }

        public int CopyCategories() {
            var query =
                @"SELECT Categories.[Id], Categories.[Code], Categories.[Name], Categories.[AncestorId] " +
                @"FROM Categories " +
                @"ORDER BY Categories.[Code];";
            OdbcCommand command = new OdbcCommand(query);

            // read categories from jet
            var jetCats = new List<JetCategory>();
            using (OdbcConnection connection = new OdbcConnection(_connectionString)) {
                command.Connection = connection;
                connection.Open();
                using (var reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        var category = new JetCategory();
                        category.Id = reader.GetInt32(0);
                        category.Code = reader.SafeGetString(1);
                        category.Name = reader.SafeGetString(2);
                        category.AncestorId = reader.SafeGetInt32(3);
                        jetCats.Add(category);
                    }
                }
            }

            // copy on SQL express
            using (var context = new PatenteNContext()) {
                // get all categories from sql express
                // copy/update all jet cats
                int n = 0;
                foreach (var jetCat in jetCats) {
                    // get the relative cat or create a new one
                    var cat = context.Categories.FirstOrDefault(c => c.Code.Equals(jetCat.Code)) ?? new Category { Id = Guid.Empty };
                    cat.Name = jetCat.Name.Trim();
                    cat.Code = jetCat.Code.Trim();
                    // if jetCat has an ancestor, get the same ancestor from cats and associate ot
                    if (jetCat.AncestorId != null) {
                        var jetAncestor = jetCats.FirstOrDefault(c => c.Id == jetCat.AncestorId);
                        if (jetAncestor != null) cat.AncestorId = context.Categories.FirstOrDefault(c => c.Code.Equals(jetAncestor.Code))?.Id ?? null;
                    }

                    // save the cat
                    if (cat.Id == Guid.Empty) {
                        cat.Id = _guidGenerator.Create();
                        context.Add(cat);
                    } else {
                        context.Update(cat);
                    }
                    context.SaveChanges();
                    n++;
                }

                // save all
                context.SaveChanges();

                return (n + 1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int CopyQuestionsTxt() {
            string fileName = @"C:\Users\carettig\Downloads\Quiz\Sorgente.txt";
            using (var context = new PatenteNContext()) {
                var cats = context.Categories.ToList();

                int nRiga = 0;
                using (StreamReader reader = new StreamReader(fileName)) {
                    string line;
                    while ((line = reader.ReadLine()) != null) {
                        nRiga++;

                        // se la riga è vuota, saltala
                        if (string.IsNullOrEmpty(line)) continue;

                        // recupera la domanda
                        var source = new Sorgente(line.Split(new char[] { '\t', }));
                        if (string.IsNullOrWhiteSpace(source.Code) || source.Code.Length < 7) continue;
                        if (string.IsNullOrWhiteSpace(source.Statement)) throw new Exception($"Testo domanda errata alla riga {nRiga}: {source.Statement}");

                        var domanda = new Question() {
                            Id = _guidGenerator.Create(),
                            Code = source.Code.Trim(),
                            Statement = source.Statement?.Trim() ?? "",
                        };

                        var cat = (Category?)null;
                        var guessedCatCode = source.Code;
                        for (int i = 0; i < 3; i++) {
                            guessedCatCode = guessedCatCode.SubstringLastIndexOf('.');
                            cat = cats.FirstOrDefault(c => c.Code.Equals(guessedCatCode));
                            if (cat != null) break;
                        }
                        if (cat == null) throw new Exception($"Non riesco a trovare una categoria associata al numero alla riga {nRiga}");
                        domanda.CategoryId = cat.Id;

                        context.Questions.Add(domanda);

                        // recupera le 3 possibili risposte
                        bool esisteRispostaGiusta = false;
                        for (int i = 0; i < 3; i++) {
                            line = reader.ReadLine();
                            if (line == null) throw new Exception($"Mi aspettavo una risposta alla riga {nRiga} ma ho beccato 'null'");
                            nRiga++;
                            source = new Sorgente(line.Split(new char[] { '\t', }));
                            if (!string.IsNullOrWhiteSpace(source.Code)) throw new Exception($"Numero non previsto alla riga {nRiga}: {source.Code}");
                            //if (source.Posizione == null) throw new Exception($"Manca la posizione della riposta alla riga {nRiga}");
                            var risposta = new Answer() {
                                Id = _guidGenerator.Create(),
                                QuestionId = domanda.Id,
                                Statement = source.Statement?.Trim() ?? "",
                                Position = (i + 1),
                                IsCorrect = source.IsCorrect == "X",
                            };
                            if (risposta.IsCorrect && esisteRispostaGiusta) throw new Exception($"Troppe risposte corrette alla riga {nRiga} per la domanda {domanda.Code}");
                            if (risposta.IsCorrect && !esisteRispostaGiusta) esisteRispostaGiusta = true;
                            context.Answers.Add(risposta);
                        }
                        if (!esisteRispostaGiusta) throw new Exception($"Non trovo la risposta corretta alla riga {nRiga} per la domanda {domanda.Code}");
                        context.SaveChanges();
                        System.Console.WriteLine($"Saved question {domanda.Code}");
                    }
                }

                // save all
                context.SaveChanges();

                return nRiga;
            }
        }

        public int CopyQuestions() {
            using (var context = new PatenteNContext()) {
                var cats = context.Categories.ToList();

                int nRiga = 0;
                while (nRiga < context.Sorgentes.Count()) {
                    var source = context.Sorgentes.Skip(nRiga).First();
                    // se la riga è vuota, saltala
                    //if (string.IsNullOrEmpty(source.Code) && string.IsNullOrEmpty(source.Position) && string.IsNullOrEmpty(source.IsCorrect) &&
                    //    (string.IsNullOrEmpty(source.Statement) || source.Statement.StartsWith("Fine ", StringComparison.InvariantCultureIgnoreCase))) {
                    if (string.IsNullOrEmpty(source.Code) || source.Code.Length < 7) {
                        nRiga++;
                        continue;
                    }
                    // recupera la domanda
                    if (string.IsNullOrWhiteSpace(source.Code)) throw new Exception($"Numero errato alla riga {nRiga}: {source.Code}");
                    if (string.IsNullOrWhiteSpace(source.Statement)) throw new Exception($"Testo domanda errata alla riga {nRiga}: {source.Statement}");
                    var domanda = new Question() {
                        Id = _guidGenerator.Create(),
                        Code = source.Code.Trim(),
                        Statement = source.Statement?.Trim() ?? "",
                    };

                    var cat = (Category?)null;
                    var guessedCatCode = source.Code;
                    for (int i = 0; i < 2; i++) {
                        guessedCatCode = source.Code.SubstringLastIndexOf('.');
                        cat = cats.FirstOrDefault(c => c.Code.Equals(guessedCatCode));
                        if (cat != null) break;
                    }
                    if (cat == null) throw new Exception($"Non riesco a trovare una categoria associata al numero alla riga {nRiga}");
                    domanda.CategoryId = cat.Id;

                    // recupera le 3 possibili risposte
                    bool esisteRispostaGiusta = false;
                    for (int i = 0; i < 3; i++) {
                        nRiga++;
                        source = context.Sorgentes.Skip(nRiga).First();
                        if (!string.IsNullOrWhiteSpace(source.Code)) throw new Exception($"Numero non previsto alla alla riga {nRiga}: {source.Code}");
                        //if (source.Posizione == null) throw new Exception($"Manca la posizione della riposta alla riga {nRiga}");
                        var risposta = new Answer() {
                            Id = _guidGenerator.Create(),
                            QuestionId = domanda.Id,
                            Statement = source.Statement?.Trim() ?? "",
                            Position = (i + 1),
                            IsCorrect = source.IsCorrect?.Equals("X", StringComparison.InvariantCultureIgnoreCase) ?? false,
                        };
                        if (risposta.IsCorrect && esisteRispostaGiusta) throw new Exception($"Troppe risposte corrette alla riga {nRiga} per la domanda {domanda.Code}");
                        if (risposta.IsCorrect && !esisteRispostaGiusta) esisteRispostaGiusta = true;
                    }
                    if (!esisteRispostaGiusta) throw new Exception($"Non trovo la risposta corretta alla riga {nRiga} per la domanda {domanda.Code}");
                    nRiga++;
                }

                //// save all
                //context.SaveChanges();

                return nRiga;
            }
        }

    }
}

