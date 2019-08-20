using AutoMapper;
using CcNetCore.Application.Models;
using CcNetCore.Domain.Dtos;
using CcNetCore.Utils.Interfaces;

namespace CcNetCore.Application {
    /// <summary>
    /// 映射配置
    /// </summary>
    public class MappingProfile : Profile, IAutoMapperProfile {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MappingProfile () {
            #region User
            CreateMap<UserModel, UserDto> ();
            CreateMap<UserDto, UserModel> ();

            CreateMap<CreateUserModel, UserDto> ();
            CreateMap<UpdateUserModel, UserDto> ();
            CreateMap<VerifyUserModel, UserDto> ();
            CreateMap<ChangePwdModel, UserDto> ()
                .ForMember (d => d.PasswordHash, opt => opt
                    .MapFrom (s => s.OldPasswordHash));
            #endregion

            #region Menu
            CreateMap<MenuModel, MenuDto> ();
            CreateMap<MenuDto, MenuModel> ();

            CreateMap<CreateMenuModel, MenuDto> ();
            CreateMap<UpdateMenuModel, MenuDto> ();
            #endregion

            #region Role
            CreateMap<RoleModel, RoleDto> ();
            CreateMap<RoleDto, RoleModel> ();

            CreateMap<CreateRoleModel, RoleDto> ();
            CreateMap<UpdateRoleModel, RoleDto> ();
            #endregion

            #region Permission
            CreateMap<PermModel, PermDto> ();
            CreateMap<PermDto, PermModel> ();

            CreateMap<CreatePermModel, PermDto> ();
            CreateMap<UpdatePermModel, PermDto> ();
            #endregion

            #region UserRole
            CreateMap<UserRoleModel, UserRoleDto> ();
            CreateMap<UserRoleDto, UserRoleModel> ();

            CreateMap<CreateUserRoleModel, UserRoleDto> ();
            #endregion

            #region RolePermission
            CreateMap<RolePermModel, RolePermDto> ();
            CreateMap<RolePermDto, RolePermModel> ();

            CreateMap<CreateRolePermModel, RolePermDto> ();
            #endregion
        }
    }
}