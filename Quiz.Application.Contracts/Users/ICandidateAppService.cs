using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PatenteN.Quiz.Domain.Users;

namespace PatenteN.Quiz.Application.Users {

    public interface ICandidateAppService : IQuizApplicationService<Candidate, CandidateDto> {
        Task<int> UpdateCandidate(CandidateDto entity);
    }
}
