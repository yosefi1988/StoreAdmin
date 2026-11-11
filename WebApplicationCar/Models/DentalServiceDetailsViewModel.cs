using System;
using System.Collections.Generic;

namespace WebApplicationCar.Models
{
    public class DentalServiceDetailsViewModel
    {
        public int ServiceId { get; set; }
        public string ServiceCode { get; set; }
        public int ResourceId { get; set; }
        public string NameFa { get; set; }
        public string NameEn { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public DateTime? CreatedAt { get; set; }

        public decimal? Price { get; set; }
        public string PriceType { get; set; }
        public string UnitName { get; set; }

        public List<ImageDto> Images { get; set; } = new List<ImageDto>();
        public List<AttributeDto> Attributes { get; set; } = new List<AttributeDto>();
        public List<ProviderGroupDto> ProviderGroups { get; set; } = new List<ProviderGroupDto>();
    }

    public class ServiceScheduleDto
    {
        public int ScheduleId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int Capacity { get; set; }
        public int ReservedCount { get; set; }
        public int RemainingCapacity { get; set; }
        public int ProviderId { get; set; }
        public string ProviderName { get; set; }
    }

    public class ProviderGroupDto
    {
        public int ProviderId { get; set; }
        public string ProviderName { get; set; }
        public List<ServiceScheduleDto> Schedules { get; set; } = new List<ServiceScheduleDto>();
    }
}