using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;

namespace WPFGui.ViewModels
{
	internal class ViewModelBase : BindableBase
	{
		#region data

		private DelegateCommand _loadedCommand;

		#endregion

		#region properties

		public ICommand LoadedCommand => _loadedCommand ?? (_loadedCommand = new DelegateCommand(OnViewLoaded));

		#endregion

		#region virtual

		protected virtual void OnViewLoaded()
		{
		}

		#endregion
	}
}