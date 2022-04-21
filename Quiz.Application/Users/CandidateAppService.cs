﻿using AutoMapper;
using Microsoft.Extensions.Logging;
using Quiz.Application.Guids;
using Quiz.Domain;
using Quiz.Domain.Users;

namespace Quiz.Application.Users {

    public class CandidateAppService
        : QuizApplicationService<Candidate, CandidateDto, Guid>, ICandidateAppService {

        public CandidateAppService(
            ILogger<CandidateAppService> logger,
            IGuidGenerator guidGenerator,
            QuizDBContext dbContext,
            IMapper mapper)
            : base( logger, guidGenerator, dbContext, mapper) {
        }

        public async Task<int> UpdateCandidate(CandidateDto dto) {
            Candidate entity = await _dbSet.FindAsync(dto.Id);
            if (entity == null) throw new Exception($"Entity not found ${typeof(Candidate)}");
            _mapper.Map<CandidateDto, Candidate>(dto, entity);
            entity.ModifiedOn = DateTimeOffset.Now;
            _dbSet.Update(entity);
            return await _dbContext.SaveChangesAsync();
        }

    }
}
