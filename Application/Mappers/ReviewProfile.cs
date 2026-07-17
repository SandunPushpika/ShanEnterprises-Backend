using AutoMapper;
using Core.DTOs.Request.Review;
using Core.Entities;

namespace Application.Mappers;

public class ReviewProfile: Profile
{
    public ReviewProfile()
    {
        CreateMap<AddReviewRequest, Review>()
            .ForMember(
                dest => dest.BookingId,
                opt => opt.Ignore()
            )
            .ForMember(
                dest => dest.CustomerId,
                opt => opt.Ignore()
            )
            .ForMember(
                dest => dest.VehicleId,
                opt => opt.Ignore()
            )
            .ForMember(
                dest => dest.CreatedAt,
                opt => opt.Ignore()
            );
    }
    
}