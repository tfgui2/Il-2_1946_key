using System;
using System.Collections.Generic;
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

namespace KeyMapUI
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        DataManagerLibrary.DataManager DM { get; set; }

        private void Button_Open(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            var res = dlg.ShowDialog();
            if (res != true)
            {
                return;
            }
            DM = new DataManagerLibrary.DataManager(dlg.FileName);

            UpdateUI();
        }

        private void Button_Save(object sender, RoutedEventArgs e)
        {
            try
            {
                DM.Save();
                MessageBox.Show("Save done");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UpdateUI()
        {
            MainContent.Children.Clear();
            foreach (var cat in DM.Categorys)
            {
                //var temp = MakeCategoryPanel(cat);
                var temp = MakeCategoryGroup(cat);
                MainContent.Children.Add(temp);
            }
        }

        private List<CheckBox> _allCheckBox = new List<CheckBox>();
        private GroupBox MakeCategoryGroup(DataManagerLibrary.Category category)
        {
            GroupBox gb = new GroupBox();
            gb.Tag = category;

            StackPanel headerStack = new StackPanel() { Orientation = Orientation.Horizontal };
            headerStack.Children.Add(new TextBlock() { Text = category.Name });

            Button sortButton = new Button() { Content = "sort", Tag = category };
            sortButton.Click += SortButton_Click;
            headerStack.Children.Add(sortButton);

            gb.Header = headerStack;

            StackPanel panel = new StackPanel() { Orientation = Orientation.Vertical, Width = 250 };
            gb.Content = panel;

            foreach (var item in category.Entitys)
            {
                CheckBox temp = new CheckBox();
                temp.Content = item.Line;
                temp.Tag = item;
                panel.Children.Add(temp);
                _allCheckBox.Add(temp);
            }
            return gb;
        }

        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            Button temp = sender as Button;
            DataManagerLibrary.Category category = temp.Tag as DataManagerLibrary.Category;
            category.Sort();

            UpdateUI();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;

            if (b.Name == "delete")
            {
                List<DataManagerLibrary.Entity> list = FindCheckedEntity();

                foreach (var item in list)
                {
                    DM.Delete(item);
                }

                UpdateUI();
            }
        }

        private List<DataManagerLibrary.Entity> FindCheckedEntity()
        {
            List<DataManagerLibrary.Entity> list = new List<DataManagerLibrary.Entity>();

            foreach (var c in _allCheckBox)
            {
                if (c.IsChecked == true)
                    list.Add(c.Tag as DataManagerLibrary.Entity);
            }

            return list;
        }
    }
}
