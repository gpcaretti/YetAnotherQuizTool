using AutoMapper;
using Microsoft.Extensions.Logging;
using Quiz.Application.Guids;
using Quiz.Domain;
using Quiz.Domain.Identity;

namespace Quiz.Application.Users {

    public class NullCandidateAppService : QuizApplicationService<Candidate, CandidateDto, Guid>, ICandidateAppService {

        private readonly string NULL_NAME = "NO NAME";

        public NullCandidateAppService(
            ILogger<NullCandidateAppService> logger,
            IGuidGenerator guidGenerator,
            QuizDBContext dbContext,
            IMapper mapper)
            : base(logger, guidGenerator, dbContext, mapper) {
        }

        public override Task<bool> Any(Guid candidateId) {
            return Task.FromResult(candidateId == Guid.Empty);
        }

        public override Task<int> Create(CandidateDto dto) {
            throw new NotSupportedException();
        }

        public override Task<int> Delete(Guid id) {
            throw new NotSupportedException();
        }

        public override Task<CandidateDto> FindById(Guid id) {
            // TOD0
            return base.FindById(id);
        }

        public Task<string> GetCandidateName(Guid? candidateId) {
            return Task.FromResult(NULL_NAME);
        }

        public Task<CandidateDto> GetCurrentUser() {
            // TODO
            return Task.FromResult(
                new CandidateDto {
                    Id = Guid.Empty,
                    Name = NULL_NAME
                });
        }

        public Task<int> UpdateCandidate(CandidateDto dto) {
            throw new NotSupportedException();
        }

    }
}
    