using System;
using Microsoft.Extensions.DependencyInjection;
using Quiz.Application.Exams;
using Quiz.Application.Guids;
using Quiz.Application.Users;

namespace Quiz.Application {

    public static class ServiceCollectionExtension {

        /// <summary>
        ///     Register my application's services
        /// </summary>
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
