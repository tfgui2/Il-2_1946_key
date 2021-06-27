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
using System.Windows.Threading;

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

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateDeviceList();
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

            JoyContent.Children.Clear();
            foreach (var cat in DM.Joysticks)
            {
                var temp = MakeJoystickSection(cat);
                JoyContent.Children.Add(temp);
            }
        }

        private List<CheckBox> _allCheckBox = new List<CheckBox>();
        private GroupBox MakeCategoryGroup(DataManagerLibrary.Category category)
        {
            GroupBox gb = new GroupBox();
            gb.Tag = category;

            // group header
            StackPanel headerStack = new StackPanel() { Orientation = Orientation.Horizontal };
            headerStack.Children.Add(new TextBlock() { Text = category.Name });

            Button sortButton = new Button() { Content = "sort", Tag = category };
            sortButton.Click += SortButton_Click;
            headerStack.Children.Add(sortButton);

            gb.Header = headerStack;

            // group content
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

        private GroupBox MakeJoystickSection(DataManagerLibrary.Category category)
        {
            GroupBox gb = new GroupBox();
            // group header
            StackPanel headerStack = new StackPanel() { Orientation = Orientation.Horizontal };
            headerStack.Children.Add(new TextBlock() { Text = category.Name });
            CheckBox check = new CheckBox() { IsChecked = false, Content = "" };
            check.Checked += Check_Checked;
            check.Unchecked += Check_Unchecked;
            headerStack.Children.Add(check);
            gb.Header = headerStack;

            // group content
            StackPanel panel = new StackPanel() { Orientation = Orientation.Vertical, Width = 250 };
            gb.Content = panel;
            check.Tag = panel;

            foreach (var item in category.Entitys)
            {
                if (item.IsJoystick == true)
                {
                    CheckBox temp = new CheckBox();
                    temp.Content = item.Line;
                    temp.Tag = item;
                    panel.Children.Add(temp);
                    _allCheckBox.Add(temp);
                }
            }

            return gb;
        }

        private void Check_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            StackPanel panel = cb.Tag as StackPanel;
            foreach (CheckBox item in panel.Children)
            {
                item.IsChecked = false;
            }
        }

        private void Check_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            StackPanel panel = cb.Tag as StackPanel;
            foreach (CheckBox item in panel.Children)
            {
                item.IsChecked = true;
            }
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
            else if (b.Name == "change")
            {
                ChangeJoystickID();
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

        DeviceIDLibrary.DeviceIDHelper _deviceIDHelper = new DeviceIDLibrary.DeviceIDHelper();
        private void UpdateDeviceList()
        {
            var list = _deviceIDHelper.GetDeviceList();
            if (_deviceIDHelper.IsChanged == true)
                deviceList.ItemsSource = list;
        }

        private void ChangeJoystickID()
        {
            int index = deviceList.SelectedIndex;
            if (index < 0)
                return;

            string dstName = $"JoystickDevice{index}";
            foreach (var c in _allCheckBox)
            {
                if (c.IsChecked == false)
                    continue;

                DataManagerLibrary.Entity e = c.Tag as DataManagerLibrary.Entity;
                string temp = e.Value;
                e.Value = SwapJoyName(temp, dstName);
                c.Content = e.Line;
            }

            UpdateCheckBoxContent();
        }

        private void UpdateCheckBoxContent()
        {
            foreach (var c in _allCheckBox)
            {
                DataManagerLibrary.Entity e = c.Tag as DataManagerLibrary.Entity;
                c.Content = e.Line;
            }
        }

        private string SwapJoyName(string EntityValue, string joystickName)
        {
            var temp = EntityValue.Split(' ');
            temp[1] = joystickName;
            return string.Join(" ", temp);
        }

        private void TabItem_Selected(object sender, RoutedEventArgs e)
        {
            foreach (var item in _allCheckBox)
            {
                item.IsChecked = false;
            }
        }
    }
}
