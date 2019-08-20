using AutoMapper;
using CcNetCore.Domain.Dtos;
using CcNetCore.Domain.Entities;
using CcNetCore.Utils.Interfaces;

namespace CcNetCore.Domain {
    /// <summary>
    /// 映射配置
    /// </summary>
    public class MappingProfile : Profile, IAutoMapperProfile {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MappingProfile () {
            CreateMap<UserDto, User> ();
            CreateMap<User, UserDto> ();

            CreateMap<RoleDto, Role> ();
            CreateMap<Role, RoleDto> ();

            CreateMap<MenuDto, Menu> ();
            CreateMap<Menu, MenuDto> ();

            CreateMap<PermDto, Permission> ();
            CreateMap<Permission, PermDto> ();

            CreateMap<UserRoleDto, UserRole> ();
            CreateMap<UserRole, UserRoleDto> ();

            CreateMap<RolePermDto, RolePermission> ();
            CreateMap<RolePermission, RolePermDto> ();
        }
    }
}