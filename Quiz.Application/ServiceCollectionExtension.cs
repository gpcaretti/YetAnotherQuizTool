using System.Data;
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
        public static IServiceCollection AddQuizStandardServices(
            this IServiceCollection services,
            ServiceLifetime? lifetime = null) {
            return services
                .AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies())
                .AddSingleton<IGuidGenerator>(new SequentialGuidGenerator(SequentialGuidType.SequentialAtEnd))

                .AddOrReplace <ICandidateAppService, NullCandidateAppService>(lifetime)
                .AddOrReplace<IExamAppService, ExamAppService>(lifetime)
                .AddOrReplace < IQuestionAppService, QuestionAppService>(lifetime)
                .AddOrReplace<IExamSessionAppService, ExamSessionAppService>(lifetime)
            ;
        }

        /// <summary>
        ///     Add a service if not exist, else repleace it
        /// </summary>
        /// <param name="lifetime">If null, is set with the same of the replaced service, else <see cref="ServiceLifetime.Transient"/></param>
        public static IServiceCollection AddOrReplace<TService, TImplementation>(
            this IServiceCollection services,
            ServiceLifetime? lifetime = null)
            where TService : class
            where TImplementation : class, TService {

            // get service desc., if the service already exists
            var serviceDescriptor = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(TService));
            if (serviceDescriptor != null) services.Remove<TService>(serviceDescriptor);

            lifetime ??= serviceDescriptor?.Lifetime ?? ServiceLifetime.Transient;

            switch (lifetime) {
                case ServiceLifetime.Singleton:
                    return services.AddSingleton<TService, TImplementation>();
                case ServiceLifetime.Scoped:
                    return services.AddScoped<TService, TImplementation>();
                case ServiceLifetime.Transient:
                    return services.AddTransient<TService, TImplementation>();
                default:
                    throw new ArgumentException($"'{nameof(lifetime)}' not implemented");
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

        /// <summary>
        ///     Remove the passed service, if exists.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="serviceDescriptor"></param>
        /// <returns></returns>
        /// <exception cref="ReadOnlyException"></exception>
        public static IServiceCollection Remove<T>(this IServiceCollection services, ServiceDescriptor serviceDescriptor = null) {
            if (services.IsReadOnly) throw new ReadOnlyException($"{nameof(services)} is read only");

            serviceDescriptor ??= services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(T));
            if (serviceDescriptor != null) {
                if (serviceDescriptor.ServiceType != typeof(T))
                    throw new ArgumentException($"'{nameof(serviceDescriptor)}' doesn't describe the same type of '{nameof(T)}'");
                services.Remove(serviceDescriptor);
            }

            return services;
        }
    }
}
