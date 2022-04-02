using System;
using Microsoft.Extensions.DependencyInjection;
using PatenteN.Quiz.Application.Exams;
using PatenteN.Quiz.Application.Guids;
using PatenteN.Quiz.Application.Users;

namespace PatenteN.Quiz {

    public static class ServiceCollectionExtension {

        public static IServiceCollection AddMyServices(this IServiceCollection services) {
            return services
                .AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies())
                .AddSingleton<IGuidGenerator>(new SequentialGuidGenerator(SequentialGuidType.SequentialAtEnd))

                .AddScoped<ICandidateAppService, CandidateAppService>()
                .AddScoped<IExamAppService, ExamAppService>()
                .AddScoped<IQuestionAppService, QuestionAppService>()
                .AddScoped<IResultAppService, ResultAppService>();
        }
    }
}
