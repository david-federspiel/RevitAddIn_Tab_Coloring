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
    }
}
