using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitAddin_TabColoring
{
    [Transaction(TransactionMode.Manual)]
    public class OnOffCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //change status
            App.isActive = !App.isActive;

            // color/uncolor tabs according to status
            if (App.isActive == true)
            {
                App.TabColoring(App.documentDictionary);
            }
            else if (App.isActive == false)
            {
                App.TabUnColoring();
            }

            return Result.Succeeded;
        }
    }
}