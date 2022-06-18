using System;
using ComputerInterface.Interfaces;

namespace GorillaSigns.ComputerInterface
{
	public class InterfaceEntry : IComputerModEntry
	{
		public string EntryName => "Gorilla Signs";
		public Type EntryViewType => typeof(InterfaceView);
	}
}
