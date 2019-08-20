using CcNetCore.Application.Interfaces;
using CcNetCore.Application.Models;
using CcNetCore.Domain.Dtos;
using CcNetCore.Utils.Interfaces;

namespace CcNetCore.Application.Services {
    /// <summary>
    /// 菜单服务
    /// </summary>
    public class MenuService : BaseService<MenuModel, MenuDto>, IMenuService, ITransientInstance { }
}