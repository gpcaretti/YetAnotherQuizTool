using System.Linq.Dynamic.Core;
using AutoMapper;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quiz.Application.Dtos;
using Quiz.Application.Guids;
using Quiz.Application.Users;
using Quiz.Domain.Identity;

namespace Quiz.Blazor.ServerApp.Services {

    public class IdentityCandidateAppService : /*QuizApplicationService<ApplicationUser, CandidateDto, Guid>, */ ICandidateAppService {

        private readonly ILogger<IdentityCandidateAppService> _logger;
        private readonly IGuidGenerator _guidGenerator;

        private readonly QuizIdentityDBContext _dbContext;
        private readonly IMapper _mapper;

        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityCandidateAppService(
            ILogger<IdentityCandidateAppService> logger,
            IGuidGenerator guidGenerator,
            QuizIdentityDBContext dbContext,
            IMapper mapper,
            AuthenticationStateProvider authenticationStateProvider,
            UserManager<ApplicationUser> userManager)
            /*: base(logger, guidGenerator, dbContext, mapper)*/ {
            _logger = logger;
            _guidGenerator = guidGenerator;
            _dbContext = dbContext;
            _mapper = mapper;
            _authenticationStateProvider = authenticationStateProvider;
            _userManager = userManager;
        }

        public Task<bool> Any(Guid id) {
            return _dbContext.Users.AnyAsync(u => u.Id == id.ToString());
        }

        // TODO
        public Task<int> Create(CandidateDto dto) {
            throw new NotImplementedException();
        }

        // TODO
        public Task<int> Delete(Guid id) {
            throw new NotImplementedException();
        }

        public async Task<CandidateDto?> FindById(Guid id) {
            var entity = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id.ToString());
            return (entity != null) ? _mapper.Map<CandidateDto>(entity!) : default(CandidateDto);
        }

        public async Task<ICollection<CandidateDto>> GetAll(PagedAndSortedResultRequestDto input) {
            IQueryable<ApplicationUser> query = string.IsNullOrEmpty(input.Sorting)
                ? _dbContext.Users.OrderBy(e => e.Id)
                : _dbContext.Users.OrderBy(input.Sorting);
            query = query.Skip(input.SkipCount).Take(input.MaxResultCount);

            var entities = await query.ToListAsync();
            return _mapper.Map<CandidateDto[]>(entities);
        }

        public Task<string?> GetCandidateName(Guid? candidateId) {
            return candidateId.HasValue
                ? _dbContext.Users.Where(u => u.Id == candidateId.ToString()).Select(u => u.UserName).FirstOrDefaultAsync()
                : Task.FromResult(default(String));
        }

        public async Task<CandidateDto?> GetCurrentUser() {
            AuthenticationState authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var appUser = await _userManager.GetUserAsync(authState.User);
            return (appUser != null) ? _mapper.Map<CandidateDto>(appUser) : default(CandidateDto);
        }

        public Task<int> UpdateCandidate(CandidateDto dto) {
            throw new NotImplementedException();
        }
    }
}
