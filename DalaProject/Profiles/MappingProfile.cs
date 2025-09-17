using AutoMapper;
using DalaProject.Models;
using DalaProject.DTOs.User;
using DalaProject.DTOs.Company;
using DalaProject.DTOs.Ground;
using DalaProject.DTOs.Season;
using DalaProject.DTOs.Product;
using DalaProject.DTOs.MarketProduct;
using DalaProject.DTOs.Report;
using DalaProject.DTOs.OwnerFermer;

namespace DalaProject.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ================== USER ==================
            CreateMap<User, UserDTO>();
            CreateMap<UserRegisterDTO, User>();

            // ================== COMPANY ==================
            CreateMap<Company, CompanyDTO>();
            CreateMap<CompanyCreateDTO, Company>();
            CreateMap<Company, CompanyDetailsDTO>()
                .ForMember(d => d.OwnerName, opt => opt.MapFrom(s => s.Owner.FullName));

            // ================== GROUND ==================
            CreateMap<Ground, GroundDTO>();
            CreateMap<GroundCreateDTO, Ground>();
            CreateMap<Ground, GroundDetailsDTO>()
                .ForMember(d => d.CompanyName, opt => opt.MapFrom(s => s.Company.Name))
                .ForMember(d => d.FermerName, opt => opt.MapFrom(s => s.Fermer != null ? s.Fermer.FullName : null));

            // ================== SEASON ==================
            CreateMap<Season, SeasonDTO>();
            CreateMap<SeasonCreateDTO, Season>();
            CreateMap<Season, SeasonDetailsDTO>()
                .ForMember(d => d.GroundLocation, opt => opt.MapFrom(s => s.Ground.Location));

            // ================== PRODUCT ==================
            CreateMap<Product, ProductDTO>();
            CreateMap<ProductCreateDTO, Product>();
            CreateMap<Product, ProductDetailsDTO>()
                .ForMember(d => d.SeasonName, opt => opt.MapFrom(s => s.Season.Name));

            // ================== MARKET PRODUCT ==================
            CreateMap<MarketProduct, MarketProductDTO>();
            CreateMap<MarketProductCreateDTO, MarketProduct>();
            CreateMap<MarketProduct, MarketProductDetailsDTO>()
                .ForMember(d => d.FermerName, opt => opt.MapFrom(s => s.Fermer.FullName))
                .ForMember(d => d.FermerPhone, opt => opt.MapFrom(s => s.Fermer.Phone))
                .ForMember(d => d.ProductTitle, opt => opt.MapFrom(s => s.Product.Title))
                .ForMember(d => d.ProductCategory, opt => opt.MapFrom(s => s.Product.Category));

            // ================== REPORT ==================
            CreateMap<Report, ReportDTO>();
            CreateMap<ReportCreateDTO, Report>();
            CreateMap<Report, ReportDetailsDTO>()
                .ForMember(d => d.SeasonName, opt => opt.MapFrom(s => s.Season.Name))
                .ForMember(d => d.FermerName, opt => opt.MapFrom(s => s.Fermer.FullName));

            // ================== OWNER-FERMER ==================
            CreateMap<OwnerFermer, OwnerFermerDTO>();
            CreateMap<OwnerFermerCreateDTO, OwnerFermer>();
            CreateMap<OwnerFermer, OwnerFermerDetailsDTO>()
                .ForMember(d => d.OwnerName, opt => opt.MapFrom(s => s.Owner.FullName))
                .ForMember(d => d.FermerName, opt => opt.MapFrom(s => s.Fermer.FullName));
        }
    }
}