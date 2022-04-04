
using System;

namespace Quiz.Application.Guids {
    public interface IGuidGenerator {
        Guid Create();
        Guid Create(SequentialGuidType guidType);
    }
}