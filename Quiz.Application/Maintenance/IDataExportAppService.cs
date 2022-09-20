namespace Quiz.Application.Maintenance {
    public interface IDataExportAppService {
        Task ExportToFiles(string folderPath);
        Task ExportToFilesDto(string folderPath);
    }
}