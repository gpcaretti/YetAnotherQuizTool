using System.Runtime.Serialization;

namespace Quiz.Application {

    [Serializable]
    public class EntityNotFoundException : Exception {
        public Type Type { get; }
        public Guid EntityId { get; }

        public EntityNotFoundException() {
        }

        public EntityNotFoundException(string message) : base(message) {
        }

        public EntityNotFoundException(Type type, Guid id, string message = null)
            : base(message) {
            Type = type;
            EntityId = id;
        }

        public EntityNotFoundException(string message, Exception innerException) : base(message, innerException) {
        }

        protected EntityNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) {
        }
    }
}