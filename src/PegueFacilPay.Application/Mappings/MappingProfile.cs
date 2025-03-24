using AutoMapper;
using PegueFacilPay.Domain.Entities;
using PegueFacilPay.Application.DTOs;

namespace PegueFacilPay.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Transaction, TransactionResponseDto>();
            CreateMap<CreateTransactionDto, Transaction>();
        }
    }
} 