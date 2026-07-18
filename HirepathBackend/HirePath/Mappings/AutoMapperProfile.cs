using AutoMapper;
using HirePathAI.API.DTOs.Candidate;
using HirePathAI.API.Models.Entities;

namespace HirePath.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // ============ CANDIDATE MAPPINGS ============

            // Create DTO to Entity
            CreateMap<CreateCandidateProfileDto, CandidateProfile>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Skills, opt => opt.Ignore())
                .ForMember(dest => dest.Educations, opt => opt.Ignore())
                .ForMember(dest => dest.Experiences, opt => opt.Ignore())
                .ForMember(dest => dest.Resumes, opt => opt.Ignore())
                .ForMember(dest => dest.Applications, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.IsProfileComplete, opt => opt.Ignore())
                .ForMember(dest => dest.ProfileUpdatedAt, opt => opt.Ignore());

            // Update DTO to Entity
            CreateMap<UpdateCandidateProfileDto, CandidateProfile>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Skills, opt => opt.Ignore())
                .ForMember(dest => dest.Educations, opt => opt.Ignore())
                .ForMember(dest => dest.Experiences, opt => opt.Ignore())
                .ForMember(dest => dest.Resumes, opt => opt.Ignore())
                .ForMember(dest => dest.Applications, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.IsProfileComplete, opt => opt.Ignore())
                .ForMember(dest => dest.ProfileUpdatedAt, opt => opt.Ignore());

            // Entity to DTO
            CreateMap<CandidateProfile, CandidateProfileDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName))
                .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.Skills))
                .ForMember(dest => dest.Educations, opt => opt.MapFrom(src => src.Educations))
                .ForMember(dest => dest.Experiences, opt => opt.MapFrom(src => src.Experiences))
                .ForMember(dest => dest.Resumes, opt => opt.MapFrom(src => src.Resumes));

            // ============ SKILL MAPPINGS ============
            CreateMap<CreateSkillDto, CandidateSkill>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CandidateProfile, opt => opt.Ignore())
                .ForMember(dest => dest.CandidateProfileId, opt => opt.Ignore());

            CreateMap<UpdateSkillDto, CandidateSkill>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CandidateProfile, opt => opt.Ignore())
                .ForMember(dest => dest.CandidateProfileId, opt => opt.Ignore());

            CreateMap<CandidateSkill, CandidateSkillDto>();

            // ============ EDUCATION MAPPINGS ============
            CreateMap<CreateEducationDto, CandidateEducation>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CandidateProfile, opt => opt.Ignore())
                .ForMember(dest => dest.CandidateProfileId, opt => opt.Ignore());

            CreateMap<UpdateEducationDto, CandidateEducation>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CandidateProfile, opt => opt.Ignore())
                .ForMember(dest => dest.CandidateProfileId, opt => opt.Ignore());

            CreateMap<CandidateEducation, CandidateEducationDto>();

            // ============ EXPERIENCE MAPPINGS ============
            CreateMap<CreateExperienceDto, CandidateExperience>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CandidateProfile, opt => opt.Ignore())
                .ForMember(dest => dest.CandidateProfileId, opt => opt.Ignore());

            CreateMap<UpdateExperienceDto, CandidateExperience>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CandidateProfile, opt => opt.Ignore())
                .ForMember(dest => dest.CandidateProfileId, opt => opt.Ignore());

            CreateMap<CandidateExperience, CandidateExperienceDto>();

            // ============ PROFILE PICTURE MAPPINGS ============
            CreateMap<ProfilePicture, ProfilePictureDto>();

            // ============ RESUME MAPPINGS ============
            CreateMap<Resume, ResumeDto>();
        }
    }
}