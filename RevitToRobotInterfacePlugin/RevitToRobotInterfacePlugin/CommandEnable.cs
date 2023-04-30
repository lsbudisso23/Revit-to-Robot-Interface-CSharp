using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


namespace RevitToRobotInterfacePlugin
{
    [Transaction(TransactionMode.ReadOnly)]
    internal class CommandEnable : IExternalCommand
    {
        // this method is called when the enable button is pressed from the controls added by App.AddExportToRobotUIControls
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // set the export to robot static flag to false i.e. disabled
            // this is preferable to registering the handler
            App.IsEnabled = true;
            return Result.Succeeded;
        }
    }
}


