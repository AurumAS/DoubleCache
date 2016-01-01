using DoubleCache;
using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DoubleCacheTests
{
    public class PublishingCacheTests
    {
        ICacheAside _decoratedCache;
        ICachePublisher _publisher;
        ICacheAside _publishingCache;

        public PublishingCacheTests()
        {
            _decoratedCache = A.Fake<ICacheAside>();
            _publisher = A.Fake<ICachePublisher>();

            _publishingCache = new PublishingCache(_decoratedCache, _publisher);
        }

        [Fact]
        public void Add_DecoratedCache_Called()
        {
            _publishingCache.Add("a", "b");

            A.CallTo(() => _decoratedCache.Add("a", "b")).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void AddWithTtl_DecoratedCache_Called()
        {
            _publishingCache.Add("a", "b", TimeSpan.FromMinutes(1));

            A.CallTo(() => _decoratedCache.Add("a", "b", TimeSpan.FromMinutes(1))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Add_Publisher_Called()
        {
            _publishingCache.Add("a", "b");

            A.CallTo(() => _publisher.NotifyUpdate("a", typeof(string).FullName));
        }
        
        [Fact]
        public async Task GetAsync_Decorated_Called()
        {
            var fakeAction = A.Fake<Func<Task<object>>>();
            var result = await _publishingCache.GetAsync("a", typeof(string), fakeAction);

            A.CallTo(() => _decoratedCache.GetAsync("a", typeof(string), A<Func<Task<object>>>._)).MustHaveHappened(Repeated.AtLeast.Once);
        }

        [Fact]
        public async Task GetAsync_WithTimeToLive_Decorated_Called()
        {
            var fakeAction = A.Fake<Func<Task<object>>>();
            var result = await _publishingCache.GetAsync("a", typeof(string), fakeAction, TimeSpan.FromSeconds(1));

            A.CallTo(() => _decoratedCache.GetAsync("a", typeof(string), A<Func<Task<object>>>._, TimeSpan.FromSeconds(1)))
                .MustHaveHappened(Repeated.AtLeast.Once);
        }

        [Fact]
        public async Task GetAsync_WrappedAction_CallsMethodAndPublish()
        {
            var fakeAction = A.Fake<Func<Task<object>>>();
            A.CallTo(() => fakeAction.Invoke()).Returns("b");

            Func<Task<object>> method = null;
            A.CallTo(() => _decoratedCache.GetAsync(A<string>._, A<Type>._, A<Func<Task<object>>>._))
                .Invokes(i => method = i.GetArgument<Func<Task<object>>>(2))
                .Returns(Task.FromResult("b"));
                 
            await _publishingCache.GetAsync(
                "a", 
                typeof(string), 
                fakeAction);

            await method.Invoke();

            A.CallTo(() => fakeAction.Invoke()).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _publisher.NotifyUpdate("a", typeof(string).AssemblyQualifiedName)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task GetAsync_WithTimeToLive_WrappedAction_CallsMethodAndPublish()
        {
            var fakeAction = A.Fake<Func<Task<object>>>();
            A.CallTo(() => fakeAction.Invoke()).Returns("b");

            Func<Task<object>> method = null;
            A.CallTo(() => _decoratedCache.GetAsync(A<string>._, A<Type>._, A<Func<Task<object>>>._, TimeSpan.FromSeconds(1)))
                .Invokes(i => method = i.GetArgument<Func<Task<object>>>(2))
                .Returns(Task.FromResult("b"));

            await _publishingCache.GetAsync(
                "a",
                typeof(string),
                fakeAction,
                TimeSpan.FromSeconds(1));

            await method.Invoke();

            A.CallTo(() => fakeAction.Invoke()).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _publisher.NotifyUpdate("a", typeof(string).AssemblyQualifiedName)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task GetAsyncGeneric_Decorated_Called()
        {
            var fakeAction = A.Fake<Func<Task<string>>>();
            var result = await _publishingCache.GetAsync("a", fakeAction);

            A.CallTo(() => _decoratedCache.GetAsync("a", A<Func<Task<string>>>._)).MustHaveHappened(Repeated.AtLeast.Once);
        }

        [Fact]
        public async Task GetAsyncGeneric_WithTimeToLive_Decorated_Called()
        {
            var fakeAction = A.Fake<Func<Task<string>>>();
            var result = await _publishingCache.GetAsync("a", fakeAction, TimeSpan.FromSeconds(1));

            A.CallTo(() => _decoratedCache.GetAsync("a", A<Func<Task<string>>>._, TimeSpan.FromSeconds(1))).MustHaveHappened(Repeated.AtLeast.Once);
        }

        [Fact]
        public async Task GetAsyncGeneric_WrappedAction_CallsMethodAndPublish()
        {
            var fakeAction = A.Fake<Func<Task<string>>>();
            A.CallTo(() => fakeAction.Invoke()).Returns("b");

            Func<Task<string>> method = null;
            A.CallTo(() => _decoratedCache.GetAsync(A<string>._, A<Func<Task<string>>>._))
                .Invokes(i => method = i.GetArgument<Func<Task<string>>>(1))
                .Returns(Task.FromResult("b"));

            await _publishingCache.GetAsync(
                "a",
                fakeAction);

            await method.Invoke();

            A.CallTo(() => fakeAction.Invoke()).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _publisher.NotifyUpdate("a", typeof(string).AssemblyQualifiedName)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task GetAsyncGeneric_WithTimeToLive_WrappedAction_CallsMethodAndPublish()
        {
            var fakeAction = A.Fake<Func<Task<string>>>();
            A.CallTo(() => fakeAction.Invoke()).Returns("b");

            Func<Task<string>> method = null;
            A.CallTo(() => _decoratedCache.GetAsync(A<string>._, A<Func<Task<string>>>._, TimeSpan.FromSeconds(1)))
                .Invokes(i => method = i.GetArgument<Func<Task<string>>>(1))
                .Returns(Task.FromResult("b"));

            await _publishingCache.GetAsync(
                "a",
                fakeAction, TimeSpan.FromSeconds(1));

            await method.Invoke();

            A.CallTo(() => fakeAction.Invoke()).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _publisher.NotifyUpdate("a", typeof(string).AssemblyQualifiedName)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
