using System.Globalization;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quiz.Application.Exams;
using Quiz.Application.Sessions;
using Quiz.Domain;
using Quiz.Domain.Exams;

namespace Quiz.Application.Maintenance {

    public class ExportAppService : IDataExportAppService {

        protected readonly QuizDBContext _dbContext;
        protected readonly IExamAppService _examAppService;
        protected readonly IQuestionAppService _questionAppService;

        protected readonly IMapper _mapper;
        protected readonly ILogger _logger;
        private readonly CsvConfiguration _csvConfig;

        public ExportAppService(
            ILogger<ExportAppService> logger,
            IExamAppService examAppService,
            IQuestionAppService questionAppService,
            QuizDBContext dbContext,
            IMapper mapper) {
            _examAppService = examAppService;
            _questionAppService = questionAppService;
            _mapper = mapper;
            _dbContext = dbContext;
            _logger = logger;

            _csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture) {
                NewLine = Environment.NewLine,
                BufferSize = 8196,
                Encoding = System.Text.Encoding.UTF8,
                IgnoreBlankLines = true,
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="EntityNotFoundException">In case the entity to update does not exist</exception>
        public async Task ExportToFiles(string folderPath) {
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            using var outStream = new StreamWriter(
                Path.Combine(folderPath, "delme.csv"),
                new FileStreamOptions {
                    Access = FileAccess.Write,
                    BufferSize = 8192,
                    Mode = FileMode.Create,
                    Options = FileOptions.Asynchronous,
                });
            await ExportToCsvAsync(await _dbContext.Exams.ToArrayAsync(), outStream);
        }

        public async Task ExportToFilesDto(string folderPath) {
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            using var outStream = new StreamWriter(
                Path.Combine(folderPath, "delme.csv"),
                new FileStreamOptions {
                    Access = FileAccess.Write,
                    BufferSize = 8192,
                    Mode = FileMode.Create,
                    Options = FileOptions.Asynchronous,
                });

            var q = _dbContext.Exams.ProjectTo<ExamDto>(_mapper.ConfigurationProvider);
            await ExportToCsvAsync(await q.ToArrayAsync(), outStream);
        }

        private async Task ExportToCsvAsync<T>(ICollection<T> items, StreamWriter outStream) {
            using (CsvWriter cw = new CsvWriter(outStream, _csvConfig)) {
                cw.WriteHeader<T>();
                cw.NextRecord();
                await cw.WriteRecordsAsync<T>(items);
            }

            //IEnumerable<string> columnNames = table.Columns.Cast<DataColumn>().
            //                                  Select(column => column.ColumnName);
            //await outStreamAsync.WriteAsync(Encoding.Unicode.GetBytes(string.Join(",", columnNames) + '\n'));

            //foreach (DataRow row in table.Rows) {
            //    IEnumerable<string> fields = row.ItemArray.Select(field =>
            //                                        string.Concat("\"", field.ToString().Replace("\"", "\"\""), "\""));
            //     await outStreamAsync.WriteAsync(Encoding.Unicode.GetBytes(string.Join(",", fields) + '\n'));
            //}
        }
    }
}
