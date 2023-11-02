using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace CMFL.MVVM
{
    public class ViewOperationBase
    {
        public Dispatcher CurrentDispatcher { get; } = Dispatcher.CurrentDispatcher;

        public static bool IsInDesignMode => (bool) DesignerProperties.IsInDesignModeProperty
            .GetMetadata(typeof(DependencyObject)).DefaultValue;
    }
}