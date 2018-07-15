using CsPlayer.PlayerModule.Views;
using CsPlayer.Regions;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsPlayer.PlayerModule
{
    public class PlayerModule : IModule
    {
        private IUnityContainer container;
        private IRegionManager regionManager;

        public PlayerModule(IUnityContainer container, IRegionManager regionManager)
        {
            if (container == null || regionManager == null)
                throw new ArgumentException();

            this.container = container;
            this.regionManager = regionManager;
        }

        public void Initialize()
        {
            this.InitializeServices();
            this.InitializeViews();
        }

        private void InitializeServices()
        {
            this.container.RegisterType<object, Player>(nameof(Player));
        }

        private void InitializeViews()
        {
            this.regionManager.RegisterViewWithRegion(RegionNames.MainRegion, typeof(Player));
        }
    }
}
