﻿using Application.Dtos.Request.Syllabus;
using Application.Dtos.Response.Syllabus;
using Application.Utils;
using Domain.Entities;

namespace Application.Mappers;

public static class SyllabusMapper
{
    public static SyllabusDetailDto SyllabusToSyllabusDetailDto(Syllabus entity)
        => new SyllabusDetailDto()
        {
            Id = entity.Id,
            Name = entity.Name,
            Target = entity.Target,
            TotalSlot = entity.TotalSlot,
            SlotTime = entity.SlotTime,
            TeacherId = entity.Course.ModifiedById,
            MinQuizScoreRatio = entity.PassCondition?.PassRatio,
            Sections = entity.Course.Sections.Select(SectionToSectionDto).ToList()
        };

    public static SectionDto SectionToSectionDto(Section entity)
        => new SectionDto()
        {
            Id = entity.Id,
            Name = entity.Name
        };

    public static FilterSyllabusDto SyllabusToFilterSyllabusDto(Syllabus entity)
        => new FilterSyllabusDto()
        {
            Id = entity.Id,
            Name = entity.Name,
            Status = entity.Status,
            CreatedDate = DateUtils.FormatDateTimeToDateV1(entity.Course.CreatedDate)
        };

    public static Section CreateSectionDtoToSection(CreateSectionDto dto)
        => new Section()
        {
            Name = dto.Name
        };
}