using System;
using Microsoft.Extensions.DependencyInjection;
using PatenteN.Quiz.Application.Exams;
using PatenteN.Quiz.Application.Users;
using PatenteN.Quiz.Domain.Exams;

namespace PatenteN.Quiz {

    public static class ServiceCollectionExtension {

        public static IServiceCollection AddMyServices(this IServiceCollection services) {
            return services
                .AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies())

                .AddScoped<ICandidateAppService, CandidateAppService>()
                .AddScoped<IExamAppService, ExamAppService>()
                .AddScoped<IQuestionAppService, QuestionAppService>()
                .AddScoped<IResultAppService, ResultAppService>();
        }
    }
}
