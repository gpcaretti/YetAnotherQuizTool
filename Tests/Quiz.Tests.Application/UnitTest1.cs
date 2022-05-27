using Moq;
using Quiz.Application.Exams;

namespace Quiz.Tests.Application {
    public class UnitTest1 {

        public readonly Mock<IExamAppService> _mockExamService = new();

        [Fact]
        public async Task AddProduct_Success() {
            // setup
            IList<ExamDto> output = new List<ExamDto>();

            _mockExamService.Setup(x => x.GetAllRootExams()).ReturnsAsync(output);

            // Act
            var examService = new ExamAppService(_mockExamService);

            var result = _mockExamService.geta;

            // Assert
            Assert.True(result);
            _mockExamService.Verify(x => x.SaveShoppingCartItem(It.IsAny<Product>()), Times.Once);
        }

    }
}