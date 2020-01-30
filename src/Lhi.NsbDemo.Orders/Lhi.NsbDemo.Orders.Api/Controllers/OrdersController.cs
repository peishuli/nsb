using System;
using Lhi.NsbDemo.Orders.Api.Models;
using Lhi.NsbDemo.Orders.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Prometheus;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lhi.NsbDemo.Orders.Api.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IEndpointInstance _endpoint;
        private readonly ILogger<OrdersController> _logger;
        private static Counter _promCounter; 

        public OrdersController(IEndpointInstance endpoint, ILogger<OrdersController> logger)
        {
            _endpoint = endpoint;
            _logger = logger;

            _promCounter = Metrics.CreateCounter("nsbdemo_api_request_counter", "Order API Request Counter", labelNames: new[] { "host", "status" });
        }

        [HttpPost]
        [Produces(typeof(CreateOrderResponseDto))]
        [SwaggerResponse(StatusCodes.Status201Created)]

        [Route("api/v1/orders/")]
        public LhiActionResult<CreateOrderResponseDto> CreateOrder([FromBody] CreateOrderRequestDto orderRequest)
        {
            var transactionId = Guid.NewGuid().ToString();
            var contractId = Guid.NewGuid().ToString();
            var createOrderCommand = new CreateOrderCommand
            {
                TransactionId = transactionId,
                ContractId = contractId
            };

            try
            {
                _endpoint.Send(createOrderCommand);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not send CreateOrderCommand to NServiceBus");
            }


            // log info
            var format = "{Application}.{Service}.{Operation}-{TransactionId}: {LogMessage}";
            var message = $"Step 0: CreateOrderCommand was sent with TransactionId {transactionId}";
            //_logger.LogInformation(message, transactionId);
            _logger.LogInformation(format, "Lhi.NsbDemo.Orders", "Api", "OrderController-CreateOrder",transactionId, message);

            // instrumentation
            _promCounter.Labels(Environment.MachineName, "received").Inc();

            LhiActionResult<CreateOrderResponseDto> responseMessage = new LhiActionResult<CreateOrderResponseDto>(
                new CreateOrderResponseDto
                {
                    //TrackingId = createOrderCommand.TransactionId
                });
            responseMessage.CustomResponseHeaders.Add("TrackingId", createOrderCommand.TransactionId);
            
            return responseMessage;
        }
    }
}