using Application.Pipelines;
using Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
namespace UnitTests.Application
{
    public class QueryLogShould
    {
        private class TestRequest : IRequest<Result> { }

        private readonly QueryLoggingBehavior<TestRequest, Result> _behavior;

        public QueryLogShould()
        {
            _behavior = new QueryLoggingBehavior<TestRequest, Result>(NullLogger<QueryLoggingBehavior<TestRequest, Result>>.Instance);
        }

        [Fact]
        public async Task PushSuccessThrough()
        {
            var request = new TestRequest();
            var response = Result.Success();
            var next = new Mock<RequestHandlerDelegate<Result>>();
            next.Setup(n => n()).ReturnsAsync(response);

            var result = await _behavior.Handle(request, next.Object, CancellationToken.None);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task PushFailureThrough()
        {
            var request = new TestRequest();
            var response = Result.Failure(Error.None);

            var next = new Mock<RequestHandlerDelegate<Result>>();
            next.Setup(n => n()).ReturnsAsync(response);

            var result = await _behavior.Handle(request, next.Object, CancellationToken.None);

            Assert.False(result.IsSuccess);

        }
    }
}
