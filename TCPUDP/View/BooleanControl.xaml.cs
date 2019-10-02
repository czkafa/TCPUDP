using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TCPUDP.View
{
    /// <summary>
    /// BooleanControl.xaml の相互作用ロジック
    /// </summary>
    public partial class BooleanControl : UserControl
    {
        public BooleanControl()
        {
            InitializeComponent();
        }

        [BindableAttribute(true)]
        public bool IsToggled
        {
            get { return (bool)GetValue(IsToggledProperty); }
            set { SetValue(IsToggledProperty, value); }
        }
        public static readonly DependencyProperty IsToggledProperty =
        DependencyProperty.Register("IsToggled", typeof(bool), typeof(BooleanControl), new UIPropertyMetadata(false));
        //[BindableAttribute(true)]
        //public string Text
        //{
        //    get { return (string)GetValue(TextProperty); }
        //    set { SetValue(TextProperty, value); }
        //}
        //public static readonly DependencyProperty TextProperty =
        //DependencyProperty.Register("Text", typeof(string), typeof(BooleanControl), new UIPropertyMetadata(false));


    }

}
