#region Namespaces
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
#endregion

namespace RevitToRobotInterfacePlugin
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {

        // declare private static to store parameters passed to Execute method of this class
        private static ExternalCommandData _commandData = null;
        private static string _message = null;
        private static ElementSet _elements = null;
        // declare flag to protect use of statics
        private static bool _enabled = false;

        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            // this is invoked manually in revit via 'Add-ins -> External Tools ->
            // Command RevitToRobotInterfacePlugin'
            // this is done as using the Execute method of the DirectRevitAccess requires
            // these, and they cannot be created by the user, as it is not meant to be 
            // used in this way
            // this is a bit of an imperfect workaround, but is viable for the time being

            // store the commandData, message, and elements in class static variables
            _commandData = commandData;
            _message = message;
            _elements = elements;

            // set enabled flag - i.e. safe to use these static variables now
            _enabled = true;

            return Result.Succeeded;
        }

        // getter methods for static variables
        public static ExternalCommandData CommandData { get { return _commandData; } }
        public static ref string Message { get { return ref _message; } }
        public static ElementSet Elements { get { return _elements; } }
        public static bool Enabled { get { return _enabled; } }

    }
}
