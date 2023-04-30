using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


namespace RevitToRobotInterfacePlugin 
{
    [Transaction(TransactionMode.ReadOnly)]
    internal class CommandDisable : IExternalCommand
    {
        // this method is called when the disable button is pressed from the controls added by App.AddUIControls
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // set the export to robot static flag to false i.e. disabled
            // this is preferable to de-registering the handler
            ThreadWork.IsEnabled = false;
            return Result.Succeeded;
        }    
    }
}
