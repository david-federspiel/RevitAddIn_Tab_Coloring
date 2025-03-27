using System.Windows;
using System.Windows.Controls;

namespace RevitAddin_TabColoring
{
    public partial class SettingWindow : Window
    {
        // variabls to store type (ausgefüllt, umrandet, strich)
        public static string projectSelection = "Ausgefüllt";
        public static string familySelection = "Umrandet - mittel";
        public SettingWindow()
        {
            InitializeComponent();
        }

        // event handler > combox  loaded
        private void ProjectComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            // always display current selection
            ProjectComboBox.SelectedItem = FindComboBoxItem(ProjectComboBox, projectSelection);
        }

        // event handler > combobox loaded
        private void FamilyComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            // always display current selection
            FamilyComboBox.SelectedItem = FindComboBoxItem(FamilyComboBox, familySelection);
        }
        // Event-handler, if user changes Project-tab settings
        private void ProjectComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProjectComboBox.SelectedItem is System.Windows.Controls.ComboBoxItem selectedItem)
            {
                projectSelection = selectedItem.Content.ToString();
                App.TabUnColoring();
                App.TabColoring(App.documentDictionary);
            }
        }

        // Event-handler, if user changes family-tab settings
        private void FamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FamilyComboBox.SelectedItem is System.Windows.Controls.ComboBoxItem selectedItem)
            {
                familySelection = selectedItem.Content.ToString();
                App.TabUnColoring();
                App.TabColoring(App.documentDictionary);
            }
        }

        private ComboBoxItem FindComboBoxItem(ComboBox comboBox, string value)
        {
            foreach (var item in comboBox.Items)
            {
                if (item is ComboBoxItem comboBoxItem && comboBoxItem.Content.ToString() == value)
                {
                    return comboBoxItem;
                }
            }
            return null;
        }
    }
}
