using Devpull;
using Devpull.ClientCode;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.ClientCodeService;

[TestClass]
public class ClientCodeCacheServiceTests
{
    [TestMethod]
    public async Task analyze_disabled_cache()
    {
        var analyzeCodeModel = new AnalyzeCodeModel
        {
            Code = "Console.WriteLine(\"Hello, World!\");"
        };
        var loggerMock = new Mock<ILogger<ClientCodeCacheService>>();
        var mockClientCodeService = new Mock<IClientCodeService>();
        mockClientCodeService
            .Setup(m => m.Analyze(analyzeCodeModel, CancellationToken.None))
            .ReturnsAsync([]);

        var sut = new ClientCodeCacheService(new FakeCacheService(), mockClientCodeService.Object, loggerMock.Object);

        var result = await sut.Analyze(analyzeCodeModel, CancellationToken.None);
        Assert.AreEqual(0, result.Length);
        mockClientCodeService.Verify(m => m.Analyze(analyzeCodeModel, CancellationToken.None), Times.Once);

        result = await sut.Analyze(analyzeCodeModel, CancellationToken.None);
        Assert.AreEqual(0, result.Length);
        mockClientCodeService.Verify(m => m.Analyze(analyzeCodeModel, CancellationToken.None), Times.Exactly(2));
    }

    [TestMethod]
    public async Task analyze_should_not_cache()
    {
        var analyzeCodeModel = new AnalyzeCodeModel
        {
            Code = "Custom code"
        };
        var loggerMock = new Mock<ILogger<ClientCodeCacheService>>();
        var mockClientCodeService = new Mock<IClientCodeService>();
        mockClientCodeService
            .Setup(m => m.Analyze(analyzeCodeModel, CancellationToken.None))
            .ReturnsAsync([]);
        var mockCacheService = new Mock<ICacheService>();

        var sut = new ClientCodeCacheService(mockCacheService.Object, mockClientCodeService.Object, loggerMock.Object);

        var result = await sut.Analyze(analyzeCodeModel, CancellationToken.None);
        Assert.AreEqual(0, result.Length);
        mockClientCodeService.Verify(m => m.Analyze(analyzeCodeModel, CancellationToken.None), Times.Once);
        mockCacheService.Verify(m => m.GetItem<DiagnosticMsg[]>(It.IsAny<string>()), Times.Never);
        mockCacheService.Verify(m => m.SetItem(It.IsAny<string>(), Array.Empty<DiagnosticMsg>()), Times.Never);

        result = await sut.Analyze(analyzeCodeModel, CancellationToken.None);
        Assert.AreEqual(0, result.Length);
        mockClientCodeService.Verify(m => m.Analyze(analyzeCodeModel, CancellationToken.None), Times.Exactly(2));
        mockCacheService.Verify(m => m.GetItem<DiagnosticMsg[]>(It.IsAny<string>()), Times.Never);
        mockCacheService.Verify(m => m.SetItem(It.IsAny<string>(), Array.Empty<DiagnosticMsg>()), Times.Never);
    }

    [TestMethod]
    public async Task analyze_use_cache()
    {
        var analyzeCodeModel = new AnalyzeCodeModel
        {
            Code = "Console.WriteLine(\"Hello, World!\");"
        };
        var loggerMock = new Mock<ILogger<ClientCodeCacheService>>();
        var mockClientCodeService = new Mock<IClientCodeService>();
        mockClientCodeService
            .Setup(m => m.Analyze(analyzeCodeModel, CancellationToken.None))
            .ReturnsAsync([]);
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var memoryCache = new MemoryCacheService(cache);

        var sut = new ClientCodeCacheService(memoryCache, mockClientCodeService.Object, loggerMock.Object);

        var result = await sut.Analyze(analyzeCodeModel, CancellationToken.None);
        Assert.AreEqual(0, result.Length);
        mockClientCodeService.Verify(m => m.Analyze(analyzeCodeModel, CancellationToken.None), Times.Once);

        result = await sut.Analyze(analyzeCodeModel, CancellationToken.None);
        Assert.AreEqual(0, result.Length);
        mockClientCodeService.Verify(m => m.Analyze(analyzeCodeModel, CancellationToken.None), Times.Once);
    }

    // use_cache
    [TestMethod]
    public async Task execute_use_cache()
    {
        var clientCodeModel = new ClientCodeModel
        {
            Code = "Console.WriteLine(\"Hello, World!\");",
            Inputs = []
        };
        var loggerMock = new Mock<ILogger<ClientCodeCacheService>>();
        loggerMock.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

        var expectedResult = ClientCodeExecResult.Create(1, string.Empty, string.Empty, 256);
        var mockClientCodeService = new Mock<IClientCodeService>();
        mockClientCodeService
            .Setup(m => m.Execute(clientCodeModel, CancellationToken.None, null))
            .ReturnsAsync(expectedResult);
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var memoryCache = new MemoryCacheService(cache);

        var sut = new ClientCodeCacheService(memoryCache, mockClientCodeService.Object, loggerMock.Object);

        var result = await sut.Execute(clientCodeModel, CancellationToken.None);
        Assert.AreEqual(expectedResult, result);
        mockClientCodeService.Verify(m => m.Execute(clientCodeModel, CancellationToken.None, null), Times.Once);
        loggerMock.Verify(m => m.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => !string.IsNullOrWhiteSpace(v.ToString())),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);

        result = await sut.Execute(clientCodeModel, CancellationToken.None);
        Assert.AreEqual(expectedResult, result);
        mockClientCodeService.Verify(m => m.Execute(clientCodeModel, CancellationToken.None, null), Times.Once);
        loggerMock.Verify(m => m.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => !string.IsNullOrWhiteSpace(v.ToString())),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Exactly(2));
    }

    [TestMethod]
    public async Task execute_disabled_cache()
    {
        var clientCodeModel = new ClientCodeModel
        {
            Code = "Console.WriteLine(\"Hello, World!\");",
            Inputs = []
        };
        var loggerMock = new Mock<ILogger<ClientCodeCacheService>>();
        var expectedResult = ClientCodeExecResult.Create(1, string.Empty, string.Empty, 256);
        var mockClientCodeService = new Mock<IClientCodeService>();
        mockClientCodeService
            .Setup(m => m.Execute(clientCodeModel, CancellationToken.None, null))
            .ReturnsAsync(expectedResult);

        var sut = new ClientCodeCacheService(new FakeCacheService(), mockClientCodeService.Object, loggerMock.Object);

        var result = await sut.Execute(clientCodeModel, CancellationToken.None);
        Assert.AreEqual(expectedResult, result);
        mockClientCodeService.Verify(m => m.Execute(clientCodeModel, CancellationToken.None, null), Times.Once);

        result = await sut.Execute(clientCodeModel, CancellationToken.None);
        Assert.AreEqual(expectedResult, result);
        mockClientCodeService.Verify(m => m.Execute(clientCodeModel, CancellationToken.None, null), Times.Exactly(2));
    }

    [TestMethod]
    public async Task execute_should_not_cache()
    {
        var clientCodeModel = new ClientCodeModel
        {
            Code = "Custom code",
            Inputs = []
        };
        var expectedResult = ClientCodeExecResult.Create(1, string.Empty, string.Empty, 256);

        var loggerMock = new Mock<ILogger<ClientCodeCacheService>>();
        var mockClientCodeService = new Mock<IClientCodeService>();
        mockClientCodeService
            .Setup(m => m.Execute(clientCodeModel, CancellationToken.None, null))
            .ReturnsAsync(expectedResult);
        var mockCacheService = new Mock<ICacheService>();

        var sut = new ClientCodeCacheService(mockCacheService.Object, mockClientCodeService.Object, loggerMock.Object);

        var result = await sut.Execute(clientCodeModel, CancellationToken.None);
        Assert.AreEqual(expectedResult, result);
        mockClientCodeService.Verify(m => m.Execute(clientCodeModel, CancellationToken.None, null), Times.Once);
        mockCacheService.Verify(m => m.GetItem<ClientCodeExecResult>(It.IsAny<string>()), Times.Never);
        mockCacheService.Verify(m => m.SetItem(It.IsAny<string>(), expectedResult), Times.Never);

        result = await sut.Execute(clientCodeModel, CancellationToken.None);
        Assert.AreEqual(expectedResult, result);
        mockClientCodeService.Verify(m => m.Execute(clientCodeModel, CancellationToken.None, null), Times.Exactly(2));
        mockCacheService.Verify(m => m.GetItem<ClientCodeExecResult>(It.IsAny<string>()), Times.Never);
        mockCacheService.Verify(m => m.SetItem(It.IsAny<string>(), expectedResult), Times.Never);
    }

    [TestMethod]
    public async Task execute_code_with_input_no_cache()
    {
        var clientCodeModel = new ClientCodeModel
        {
            Code = "Console.WriteLine(\"Hello, World!\");",
            Inputs = ["42"]
        };
        var expectedResult = ClientCodeExecResult.Create(1, string.Empty, string.Empty, 256);
        var mockClientCodeService = new Mock<IClientCodeService>();
        mockClientCodeService
            .Setup(m => m.Execute(clientCodeModel, CancellationToken.None, null))
            .ReturnsAsync(expectedResult);
        var mockCacheService = new Mock<ICacheService>();
        var loggerMock = new Mock<ILogger<ClientCodeCacheService>>();

        var sut = new ClientCodeCacheService(mockCacheService.Object, mockClientCodeService.Object, loggerMock.Object);

        var result = await sut.Execute(clientCodeModel, CancellationToken.None);
        Assert.AreEqual(expectedResult, result);
        mockClientCodeService.Verify(m => m.Execute(clientCodeModel, CancellationToken.None, null), Times.Once);
        mockCacheService.Verify(m => m.GetItem<ClientCodeExecResult>(It.IsAny<string>()), Times.Never);
        mockCacheService.Verify(m => m.SetItem(It.IsAny<string>(), expectedResult), Times.Never);

        result = await sut.Execute(clientCodeModel, CancellationToken.None);
        Assert.AreEqual(expectedResult, result);
        mockClientCodeService.Verify(m => m.Execute(clientCodeModel, CancellationToken.None, null), Times.Exactly(2));
        mockCacheService.Verify(m => m.GetItem<ClientCodeExecResult>(It.IsAny<string>()), Times.Never);
        mockCacheService.Verify(m => m.SetItem(It.IsAny<string>(), expectedResult), Times.Never);
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(100)]
    [DataRow(101)]
    public async Task execute_bad_exit_code_no_cache(int badExitCode)
    {
        var clientCodeModel = new ClientCodeModel
        {
            Code = "Console.WriteLine(\"Hello, World!\");"
        };
        var expectedResult = ClientCodeExecResult.Create(badExitCode, string.Empty, string.Empty, 256);

        var loggerMock = new Mock<ILogger<ClientCodeCacheService>>();
        loggerMock.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

        var mockClientCodeService = new Mock<IClientCodeService>();
        mockClientCodeService
            .Setup(m => m.Execute(clientCodeModel, CancellationToken.None, null))
            .ReturnsAsync(expectedResult);
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var memoryCache = new MemoryCacheService(cache);

        var sut = new ClientCodeCacheService(memoryCache, mockClientCodeService.Object, loggerMock.Object);

        var result = await sut.Execute(clientCodeModel, CancellationToken.None);
        Assert.AreEqual(expectedResult, result);
        mockClientCodeService.Verify(m => m.Execute(clientCodeModel, CancellationToken.None, null), Times.Once);
        loggerMock.Verify(m => m.Log(
            expectedResult.IsExpectedError ? LogLevel.Warning : LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => !string.IsNullOrWhiteSpace(v.ToString())),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);

        result = await sut.Execute(clientCodeModel, CancellationToken.None);
        Assert.AreEqual(expectedResult, result);
        mockClientCodeService.Verify(m => m.Execute(clientCodeModel, CancellationToken.None, null), Times.Exactly(2));
        loggerMock.Verify(m => m.Log(
            expectedResult.IsExpectedError ? LogLevel.Warning : LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => !string.IsNullOrWhiteSpace(v.ToString())),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Exactly(2));
    }
}
