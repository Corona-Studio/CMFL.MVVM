using System.ComponentModel;

namespace CMFL.MVVM
{
    public class PropertyChange : ViewOperationBase, INotifyPropertyChanged
    {
        #region UI更新接口

        /// <summary>
        ///     属性改变
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     属性改变方法
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}