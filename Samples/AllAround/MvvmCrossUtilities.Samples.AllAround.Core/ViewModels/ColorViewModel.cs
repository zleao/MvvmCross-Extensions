using System.Collections.ObjectModel;
using Cirrious.CrossCore.UI;
using MvvmCrossUtilities.Libraries.Portable.Utilities;
using MvvmCrossUtilities.Samples.AllAround.Core.ViewModels.Base;

namespace MvvmCrossUtilities.Samples.AllAround.Core.ViewModels
{
    public class ColorViewModel : AllAroundViewModel
    {
        #region Properties

        public override string PageTitle
        {
            get { return "Contrast converter"; }
        }

        public ObservableCollection<MyColor> ColorList
        {
            get { return _colorList; }
        }
        private readonly ObservableCollection<MyColor> _colorList = new ObservableCollection<MyColor>();

        #endregion

        #region Constructor

        public ColorViewModel()
        {
            ColorList.Add(new MyColor() { Value = Color.Black, Name = "Black" });
            ColorList.Add(new MyColor() { Value = Color.White, Name = "White" });
            ColorList.Add(new MyColor() { Value = Color.Red, Name = "Red" });
            ColorList.Add(new MyColor() { Value = Color.Lime, Name = "Lime" });
            ColorList.Add(new MyColor() { Value = Color.Blue, Name = "Blue" });
            ColorList.Add(new MyColor() { Value = Color.Yellow, Name = "Yellow" });
            ColorList.Add(new MyColor() { Value = Color.Cyan, Name = "Cyan" });
            ColorList.Add(new MyColor() { Value = Color.Magenta, Name = "Magenta" });
            ColorList.Add(new MyColor() { Value = Color.Silver, Name = "Silver" });
            ColorList.Add(new MyColor() { Value = Color.Gray, Name = "Gray" });
            ColorList.Add(new MyColor() { Value = Color.Maroon, Name = "Maroon" });
            ColorList.Add(new MyColor() { Value = Color.Olive, Name = "Olive" });
            ColorList.Add(new MyColor() { Value = Color.Green, Name = "Green" });
            ColorList.Add(new MyColor() { Value = Color.Purple, Name = "Purple" });
            ColorList.Add(new MyColor() { Value = Color.Teal, Name = "Teal" });
            ColorList.Add(new MyColor() { Value = Color.Navy, Name = "Navy" });
        }

        #endregion

        #region Nested classes

        public sealed class MyColor
        {
            public MvxColor Value { get; set; }
            public string Name { get; set; }
        }

        #endregion
    }
}
