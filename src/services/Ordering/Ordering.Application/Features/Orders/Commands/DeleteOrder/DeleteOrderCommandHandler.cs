using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Exceptions;
using Ordering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Orders.Commands.DeleteOrder
{
    class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand>
    {
        private readonly IOrderRepostory _orderRepostory;
        private readonly IMapper _mapper;
        private readonly ILogger<DeleteOrderCommandHandler> _logger;

        public DeleteOrderCommandHandler(IOrderRepostory orderRepostory, IMapper mapper, ILogger<DeleteOrderCommandHandler> logger)
        {
            _orderRepostory = orderRepostory;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var orderToDelete = await _orderRepostory.GetByIdAsync(request.Id);
            if (orderToDelete == null)
            {
                _logger.LogError("Order does not exist on DB.");
                throw new NotFoundException(nameof(Order), request.Id);
            }
            await _orderRepostory.DeleteAsync(orderToDelete);
            _logger.LogInformation($"Order {orderToDelete.Id} is successfully updated.");
            return Unit.Value;
        }
    }
}
