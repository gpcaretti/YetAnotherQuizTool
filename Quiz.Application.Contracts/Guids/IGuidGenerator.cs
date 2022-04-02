
using System;

namespace PatenteN.Quiz.Application.Guids {
    public interface IGuidGenerator {
        Guid Create();
        Guid Create(SequentialGuidType guidType);
    }
}