using AutoMapper;
using TZTaskManager.Application.DTOs;
using TZTaskManager.Domain.Entities;
using TaskEntity = TZTaskManager.Domain.Entities.Task;

namespace TZTaskManager.Application.Mappings
{
    /// <summary>
    /// Профиль маппинга AutoMapper
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TaskEntity, TaskDto>()
                .ForMember(dest => dest.TaskTypeName, opt => opt.MapFrom(src => src.TaskType.Name));

            CreateMap<CreateTaskDto, TaskEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.TaskType, opt => opt.Ignore());

            CreateMap<UpdateTaskDto, TaskEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.TaskType, opt => opt.Ignore());
        }
    }
}

