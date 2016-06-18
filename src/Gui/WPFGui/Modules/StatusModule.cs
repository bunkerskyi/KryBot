using Prism.Modularity;
using Prism.Regions;

namespace WPFGui.Modules
{
	/// <summary>
	///     Модуль меню.
	/// </summary>
	[Module(ModuleName = "SitesModule")]
	public class SitesModule : IModule
	{
		private readonly IRegionManager _regionManager;
		//private readonly MenuService _menuService;

		public SitesModule(IRegionManager regionManager)
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