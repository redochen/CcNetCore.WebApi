using System;
using AutoMapper;
using CcNetCore.Domain.Dtos;
using CcNetCore.Domain.Entities;
using CcNetCore.Utils;
using CcNetCore.Utils.Extensions;
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
            #region 系统模型
            MapUser ();
            MapMenu ();
            MapRole ();
            MapPermission ();
            MapUserRole ();
            MapRolePermission ();
            MapUserPermission ();
            #endregion
        }

        #region 私有方法
        private string FormatDateTime (DateTime? dt) =>
            dt.GetString (CustomFormat.FORMAT_DATE_TIME);
        private DateTime? ParseDateTime (string str) =>
            str.TryDateTime (CustomFormat.FORMAT_DATE_TIME);

        private string FormatDate (DateTime? dt) =>
            dt.GetString (CustomFormat.FORMAT_DATE);
        private DateTime? ParseDate (string str) =>
            str.TryDateTime (CustomFormat.FORMAT_DATE);

        private string FormatTime (DateTime? dt) =>
            dt.GetString (CustomFormat.FORMAT_TIME);
        private DateTime? ParseTime (string str) =>
            str.TryDateTime (CustomFormat.FORMAT_TIME);
        #endregion

        private void MapUser () {
            CreateMap<UserDto, User> ()
                .ForMember (d => d.Id, opt => opt.MapFrom (s => s.UserID))
                .ForMember (d => d.CreateTime, opt => opt.MapFrom (s => ParseDateTime (s.CreateTime)))
                .ForMember (d => d.UpdateTime, opt => opt.MapFrom (s => ParseDateTime (s.UpdateTime)));

            CreateMap<User, UserDto> ()
                .ForMember (d => d.UserID, opt => opt.MapFrom (s => s.Id))
                .ForMember (d => d.CreateTime, opt => opt.MapFrom (s => FormatDateTime (s.CreateTime)))
                .ForMember (d => d.UpdateTime, opt => opt.MapFrom (s => FormatDateTime (s.UpdateTime)));

            CreateMap<CreateUserDto, User> ();
            CreateMap<UpdateUserDto, User> ();
            CreateMap<VerifyUserDto, User> ();
            CreateMap<ChangePwdDto, User> ();
        }

        private void MapMenu () {
            CreateMap<MenuDto, Menu> ()
                .ForMember (d => d.CreateTime, opt => opt.MapFrom (s => ParseDateTime (s.CreateTime)))
                .ForMember (d => d.UpdateTime, opt => opt.MapFrom (s => ParseDateTime (s.UpdateTime)));

            CreateMap<Menu, MenuDto> ()
                .ForMember (d => d.CreateTime, opt => opt.MapFrom (s => FormatDateTime (s.CreateTime)))
                .ForMember (d => d.UpdateTime, opt => opt.MapFrom (s => FormatDateTime (s.UpdateTime)));

            CreateMap<CreateMenuDto, Menu> ();
            CreateMap<UpdateMenuDto, Menu> ();
        }

        private void MapRole () {
            CreateMap<RoleDto, Role> ()
                .ForMember (d => d.CreateTime, opt => opt.MapFrom (s => ParseDateTime (s.CreateTime)))
                .ForMember (d => d.UpdateTime, opt => opt.MapFrom (s => ParseDateTime (s.UpdateTime)));

            CreateMap<Role, RoleDto> ()
                .ForMember (d => d.CreateTime, opt => opt.MapFrom (s => FormatDateTime (s.CreateTime)))
                .ForMember (d => d.UpdateTime, opt => opt.MapFrom (s => FormatDateTime (s.UpdateTime)));

            CreateMap<CreateRoleDto, Role> ();
            CreateMap<UpdateRoleDto, Role> ();
        }

        private void MapPermission () {
            CreateMap<PermDto, Permission> ()
                .ForMember (d => d.CreateTime, opt => opt.MapFrom (s => ParseDateTime (s.CreateTime)))
                .ForMember (d => d.UpdateTime, opt => opt.MapFrom (s => ParseDateTime (s.UpdateTime)));

            CreateMap<Permission, PermDto> ()
                .ForMember (d => d.CreateTime, opt => opt.MapFrom (s => FormatDateTime (s.CreateTime)))
                .ForMember (d => d.UpdateTime, opt => opt.MapFrom (s => FormatDateTime (s.UpdateTime)));

            CreateMap<CreatePermDto, Permission> ();
            CreateMap<UpdatePermDto, Permission> ();
        }

        private void MapUserRole () {
            CreateMap<UserRoleDto, UserRole> ()
                .ForMember (d => d.CreateTime, opt => opt.MapFrom (s => ParseDateTime (s.CreateTime)))
                .ForMember (d => d.UpdateTime, opt => opt.MapFrom (s => ParseDateTime (s.UpdateTime)));

            CreateMap<UserRole, UserRoleDto> ()
                .ForMember (d => d.CreateTime, opt => opt.MapFrom (s => FormatDateTime (s.CreateTime)))
                .ForMember (d => d.UpdateTime, opt => opt.MapFrom (s => FormatDateTime (s.UpdateTime)));
        }

        private void MapRolePermission () {
            CreateMap<RolePermDto, RolePermission> ()
                .ForMember (d => d.CreateTime, opt => opt.MapFrom (s => ParseDateTime (s.CreateTime)))
                .ForMember (d => d.UpdateTime, opt => opt.MapFrom (s => ParseDateTime (s.UpdateTime)));

            CreateMap<RolePermission, RolePermDto> ()
                .ForMember (d => d.CreateTime, opt => opt.MapFrom (s => FormatDateTime (s.CreateTime)))
                .ForMember (d => d.UpdateTime, opt => opt.MapFrom (s => FormatDateTime (s.UpdateTime)));
        }

        private void MapUserPermission () {
            CreateMap<UserPermDto, UserPermission> ()
                .ForMember (d => d.CreateTime, opt => opt.MapFrom (s => ParseDateTime (s.CreateTime)))
                .ForMember (d => d.UpdateTime, opt => opt.MapFrom (s => ParseDateTime (s.UpdateTime)));

            CreateMap<UserPermission, UserPermDto> ()
                .ForMember (d => d.CreateTime, opt => opt.MapFrom (s => FormatDateTime (s.CreateTime)))
                .ForMember (d => d.UpdateTime, opt => opt.MapFrom (s => FormatDateTime (s.UpdateTime)));
        }
    }
}