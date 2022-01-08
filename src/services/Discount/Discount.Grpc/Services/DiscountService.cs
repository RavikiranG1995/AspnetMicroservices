using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.Grpc.Services
{
    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly IDiscountRepository _discountRepository;
        private readonly ILogger<DiscountService> _logger;
        private readonly IMapper _mapper;

        public DiscountService(IDiscountRepository discountRepository, ILogger<DiscountService> logger, IMapper mapper)
        {
            this._discountRepository = discountRepository;
            this._logger = logger;
            _mapper = mapper;
        }

        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon = await this._discountRepository.GetDiscount(request.ProductName);
            if (coupon == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount with {request.ProductName} is not found."));
            }
            _logger.LogInformation($"Coupon is retrieved. ProductName : {coupon.ProductName}");
            var couponModel = this._mapper.Map<CouponModel>(coupon);
            return couponModel;
        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var coupon = this._mapper.Map<Coupon>(request.Coupon);
            await this._discountRepository.CreateDiscount(coupon);
            _logger.LogInformation($"Discount creation is success. ProductName : {coupon.ProductName}");
            var couponModel = this._mapper.Map<CouponModel>(coupon);
            return couponModel;
        }

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var coupon = this._mapper.Map<Coupon>(request.Coupon);
            await this._discountRepository.UpdateDiscount(coupon);
            _logger.LogInformation($"Discount updation is success. ProductName : {coupon.ProductName}");
            var couponModel = this._mapper.Map<CouponModel>(coupon);
            return couponModel;
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            var deleted = await this._discountRepository.DeleteDiscount(request.ProductName);
            var response = new DeleteDiscountResponse { Success = deleted };
            return response;
        }
    }
}
