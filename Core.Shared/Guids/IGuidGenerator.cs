
namespace PatenteN.Core.Guids {
    public interface IGuidGenerator {
        Guid Create();
        Guid Create(SequentialGuidType guidType);
    }
}