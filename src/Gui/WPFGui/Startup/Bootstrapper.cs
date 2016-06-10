using System;
using System.Windows;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;
using WPFGui.Modules;

namespace WPFGui.Startup
{
	public class Bootstrapper : UnityBootstrapper, IDisposable
	{
		#region data

		private IEventAggregator _eventAggregator;

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion

		#region Private

		private void InitializeLaunch()
		{
			// TODO Обработка "необработанных" исключений.
		}

		#endregion

		#region overrides

		protected override DependencyObject CreateShell()
		{
			return Container.Resolve<MainWindow>();
		}

		protected override void InitializeShell()
		{
			// register view
			var regionManager = Container.Resolve<IRegionManager>();
			if (regionManager == null) throw new ArgumentNullException(nameof(regionManager));

			Application.Current.MainWindow.Show();
		}

		/// <summary>
		///     Configures the <see cref="T:Microsoft.Practices.Unity.IUnityContainer" />.
		///     May be overwritten in a derived class to add specific type mappings required by the application.
		/// </summary>
		protected override void ConfigureContainer()
		{
			base.ConfigureContainer();
			Container.RegisterType(
				typeof(IServiceLocator),
				typeof(UnityServiceLocatorAdapter),
				new ContainerControlledLifetimeManager());
			ServiceLocator.SetLocatorProvider(() => Container.Resolve<IServiceLocator>());
		}

		/// <summary>
		///     Configures the <see cref="T:Prism.Modularity.IModuleCatalog" /> used by Prism.
		/// </summary>
		protected override void ConfigureModuleCatalog()
		{
			base.ConfigureModuleCatalog();

			var moduleCatalog = ModuleCatalog as ModuleCatalog;
			if (moduleCatalog == null) throw new ArgumentNullException(nameof(moduleCatalog));

			moduleCatalog.AddModule(typeof(StatusModule), InitializationMode.WhenAvailable);
			moduleCatalog.AddModule(typeof(MenuModule), InitializationMode.WhenAvailable);
			moduleCatalog.AddModule(typeof(MainModule), InitializationMode.WhenAvailable);
			moduleCatalog.AddModule(typeof(SitesModule), InitializationMode.WhenAvailable);
		}

		/// <summary>
		///     Run the bootstrapper process.
		/// </summary>
		/// <param name="runWithDefaultConfiguration">
		///     If <see langword="true" />,
		///     registers default Prism Library services in the container.
		///     This is the default behavior.
		/// </param>
		public override void Run(bool runWithDefaultConfiguration)
		{
			InitializeLaunch();

			base.Run(runWithDefaultConfiguration); // override собственную последовательность
		}

		#endregion
	}
}