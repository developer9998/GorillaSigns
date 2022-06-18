using Zenject;
using ComputerInterface.Interfaces;

namespace GorillaSigns.ComputerInterface
{
	internal class MainInstaller : Installer
	{
		public override void InstallBindings()
		{
			Container.Bind<IComputerModEntry>().To<InterfaceEntry>().AsSingle();
		}
	}
}
