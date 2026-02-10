using System.Diagnostics;
using Devpull.Controllers;
using Devpull.Course;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.CodeAnalysis.Completion;

namespace Devpull.ClientCode;

[ApiController]
[Route("api/client-code")]
public class ClientCodeController : ControllerBase
{
    private readonly ExecutionService _executionService;
    private readonly ClientCodeService _clientCodeService;
    private readonly ClientCodeCacheService _clientCodeCacheService;

    public ClientCodeController(
        ExecutionService executionService,
        ClientCodeService clientCodeService,
        ClientCodeCacheService clientCodeCacheService
    )
    {
        _executionService = executionService;
        _clientCodeService = clientCodeService;
        _clientCodeCacheService = clientCodeCacheService;
    }

    [HttpPost("execute")]
    public Task<OperationResult<ClientCodeExecResult>> Execute(
        ClientCodeModel model,
        CancellationToken cancellationToken
    )
    {
        return _executionService.TryExecute(
            () => _clientCodeCacheService.Execute(model, HttpContext.RequestAborted)
        );
    }

    [HttpPost("analyze")]
    public Task<OperationResult<DiagnosticMsg[]>> Analyze(
        AnalyzeCodeModel model,
        CancellationToken cancellationToken
    )
    {
        return _executionService.TryExecute(
            () => _clientCodeCacheService.Analyze(model, HttpContext.RequestAborted)
        );
    }

    [HttpPost("get-completion")]
    public Task<OperationResult<CompletionItem[]>> GetCompletion(
        CompletionRequest model,
        CancellationToken cancellationToken
    )
    {
        return _executionService.TryExecute(
            () => _clientCodeService.GetCompletion(model, HttpContext.RequestAborted)
        );
    }
}
