/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
*/
using Prism.Modularity;
using Prism.Regions;

namespace WPFGui.Modules
{
    /// <summary>
    ///     Модуль меню.
    /// </summary>
    [Module(ModuleName = "MenuModule")]
    public class MenuModule : IModule
    {
        private readonly IRegionManager _regionManager;
        //private readonly MenuService _menuService;

        public MenuModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            //_menuService = new MenuService();
        }

        public void Initialize()
        {
            _regionManager.Regions[MainRegionNames.MenuRegion].Add(new object());
        }
    }
}