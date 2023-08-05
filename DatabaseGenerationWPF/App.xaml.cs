using DatabaseGenerationWPF.ViewModels;
using DatabaseGenerationWPF.Views;
using Prism.Ioc;
using System.Windows;

namespace DatabaseGenerationWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //containerRegistry.RegisterForNavigation<JsonGenerateDialog, JsonGenerateDialogViewModel>();
        }

    }
}
