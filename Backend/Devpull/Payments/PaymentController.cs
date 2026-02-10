using Devpull.Controllers;
using Devpull.Controllers.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Yandex.Checkout.V3;

namespace Devpull.Payments;

[Route("api/payment")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly ExecutionService _executionService;
    private readonly PaymentService _paymentService;

    public PaymentController(ExecutionService executionService, PaymentService paymentService)
    {
        _executionService = executionService;
        _paymentService = paymentService;
    }

    [HttpPost("get-prices")]
    public Task<OperationResult<PricesInfo>> GetPrices()
    {
        return _executionService.TryExecute(() => _paymentService.GetPrices());
    }

    [HttpPost("create-payment")]
    public Task<OperationResult<string>> CreatePayment([FromBody] PaymentModel model)
    {
        return _executionService.TryExecute(
            () => _paymentService.CreatePayment(model, HttpContext.RequestAborted)
        );
    }

    [HttpPost("is-last-payment-success")]
    public Task<OperationResult<bool>> IsLastPaymentSuccess()
    {
        return _executionService.TryExecute(
            () => _paymentService.IsLastPaymentSuccess(HttpContext.RequestAborted)
        );
    }

    [HttpPost("webhook")]
    public async Task<OperationResult<int>> Webhook()
    {
        using var reader = new StreamReader(Request.Body);
        var body = await reader.ReadToEndAsync();
        var notification = Client.ParseMessage(Request.Method, Request.ContentType, body);

        return await _executionService.TryExecute(
            () => _paymentService.ProcessNotification(notification, HttpContext.RequestAborted)
        );
    }
}
