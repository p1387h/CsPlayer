using CsPlayer.Regions;
using CsPlayer.SongModule.Views;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsPlayer.SongModule
{
    public class SongModule : IModule
    {
        private IUnityContainer container;
        private IRegionManager regionManager;

        public SongModule(IUnityContainer container, IRegionManager regionManager)
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
            this.container.RegisterInstance<IDialogCoordinator>(DialogCoordinator.Instance);
            this.container.RegisterType<object, SongCollection>(nameof(SongCollection));
        }

        private void InitializeViews()
        {
            this.regionManager.RegisterViewWithRegion(RegionNames.TabRegion, typeof(SongCollection));
        }
    }
}
