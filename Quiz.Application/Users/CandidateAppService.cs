using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PatenteN.Quiz.Domain;
using PatenteN.Quiz.Domain.Users;

namespace PatenteN.Quiz.Application.Users {

    public class CandidateAppService : QuizApplicationService<Candidate, CandidateDto>, ICandidateAppService {

        public CandidateAppService(QuizDBContext dbContext, IMapper mapper) : base(dbContext, mapper) {
        }

        public async Task<int> UpdateCandidate(CandidateDto dto) {
            Candidate entity = await _dbSet.FindAsync(dto.Sl_No);
            if (entity == null) throw new Exception($"Entity not found ${typeof(Candidate)}");
            _mapper.Map<CandidateDto, Candidate>(dto, entity);
            entity.ModifiedOn = DateTime.Now;
            _dbSet.Update(entity);
            return await _dbContext.SaveChangesAsync();
        }

    }
}
