using CcNetCore.Application.Interfaces;
using CcNetCore.Domain.Dtos;
using CcNetCore.Domain.Entities;
using CcNetCore.Utils.Interfaces;

namespace CcNetCore.Application.Services {
    /// <summary>
    /// 菜单服务
    /// </summary>
    public class MenuService : SysService<MenuDto, Menu>, IMenuService, ITransientInstance { }
}