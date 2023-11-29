using FluentAssertions;
using LuisLaRotta.CSharp.BPLogixInterviewISBNExercise.Models;
using LuisLaRotta.CSharp.BPLogixInterviewISBNExercise.Services;

namespace LuisLaRotta.CSharp.BPLogixInterviewISBNExerciseTests
{
    public class UnitTest1
    {
        private readonly BookOpsService _bookOpsService;

        public UnitTest1()
        {
            _bookOpsService = new BookOpsService();
        }

        public void Test1()
        {
            [Fact]
            void BookOpsService_GetJsonData_ReturnString()
            {
                //Act
                Book book = _bookOpsService.getBookFromWebservice("0201558025").Result;
                //Assert
                book.Isbn.Should().Be("0201558025");
                //Act
                book = _bookOpsService.getBookFromWebservice("0984782869").Result;
                //Assert
                book.Isbn.Should().Be("0984782869");
            }
        }
    }
}