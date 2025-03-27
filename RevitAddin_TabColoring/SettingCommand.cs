using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitAddin_TabColoring
{
    [Transaction(TransactionMode.Manual)]
    class SettingCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // open wpf-window
            SettingWindow window = new SettingWindow();
            window.ShowDialog();

            return Result.Succeeded;
        }
    }
}
