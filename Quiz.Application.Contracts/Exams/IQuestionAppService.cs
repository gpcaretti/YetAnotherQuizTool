using System.Threading.Tasks;
using PatenteN.Quiz.Domain.Exams;

namespace PatenteN.Quiz.Application.Exams {
    public interface IQuestionAppService : IQuizApplicationService<Question, QuestionDto> {
        Task<QnADto> GetQuestionList(int ExamID);
        Task<int> UpdateQuestion(Question entity);
    }
}
