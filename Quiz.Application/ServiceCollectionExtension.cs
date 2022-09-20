using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Quiz.Application.Exams;
using Quiz.Application.Guids;
using Quiz.Application.Sessions;
using Quiz.Application.Users;

namespace Quiz.Application {

    public static class ServiceCollectionExtension {

        /// <summary>
        ///     Register my application's services
        /// </summary>
        public static IServiceCollection AddQuizStandardServices(this IServiceCollection services) {
            return services
                .AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies())
                .AddSingleton<IGuidGenerator>(new SequentialGuidGenerator(SequentialGuidType.SequentialAtEnd))

                .AddScoped<ICandidateAppService, NullCandidateAppService>()
                .AddScoped<IExamAppService, ExamAppService>()
                .AddScoped<IQuestionAppService, QuestionAppService>()
                .AddScoped<IExamSessionAppService, ExamSessionAppService>()
            ;
        }

        /// <summary>
        ///     Replace a service with a new one or add it, if not exists
        /// </summary>
        public static IServiceCollection AddOrReplace<TService, TImplementation>(
            this IServiceCollection services,
            ServiceLifetime? lifetime = null)
            where TService : class
            where TImplementation : class, TService {

            if (lifetime == null) lifetime = services.FirstOrDefault(d => d.ServiceType == typeof(TService))?.Lifetime ?? ServiceLifetime.Transient;
            switch (lifetime) {
                case ServiceLifetime.Singleton:
                    return services.Replace(ServiceDescriptor.Singleton<TService, TImplementation>());
                case ServiceLifetime.Scoped:
                    return services.Replace(ServiceDescriptor.Scoped<TService, TImplementation>());
                case ServiceLifetime.Transient:
                default:
                    return services.Replace(ServiceDescriptor.Transient<TService, TImplementation>());
            }
        }

        //public static IServiceCollection Replace<TService, TImplementation>(
        //    this IServiceCollection services,
        //    ServiceLifetime lifetime)
        //    where TService : class
        //    where TImplementation : class, TService {

        //    var descriptorToRemove = services.FirstOrDefault(d => d.ServiceType == typeof(TService));
        //    services.Remove(descriptorToRemove);

        //    var descriptorToAdd = new ServiceDescriptor(typeof(TService), typeof(TImplementation), lifetime);
        //    services.Add(descriptorToAdd);

        //    return services;
        //}
    }
}
