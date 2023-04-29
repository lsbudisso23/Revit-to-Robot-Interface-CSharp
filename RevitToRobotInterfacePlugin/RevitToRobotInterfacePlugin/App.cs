#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Diagnostics;
#endregion

namespace RevitToRobotInterfacePlugin
{
    internal class App : IExternalApplication
    {
        private void Handler(object sender, EventArgs args)
        {
            // setup the top level objects in the Revit Platform API are application and document.
            UIApplication uiapp = new UIApplication((Application)sender);
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            try
            {
                // start transaction on active document
                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("Posting command from Handler");

                    // get revit cmd id for print preview command
                    // print preview us used as PostCommand does not return anything that can be used to determin the 
                    // success of the command, and so an visually obvious command (print preview) is used for testing
                    // as causes a change to the active screen
                    RevitCommandId cmd = RevitCommandId.LookupPostableCommandId(PostableCommand.PrintPreview);

                    // post the print preview command
                    uiapp.PostCommand(cmd);

                    // complete the transaction
                    tx.Commit();
                }
            }
            // handle specific exceptions that can be raised via PostCommand
            // as stated, PostCommand does not return anything useful, but may raise exceptions
            // these exceptions is unhandled are simply ignored, so they are caught and printed
            // to the user for debugging
            catch (Autodesk.Revit.Exceptions.ArgumentNullException ex)
            {
                Debug.Print($"error: Handler() RevitCommandId for PrintPreview could not be determined");
                Debug.Print(ex.Message);
            }
            catch (Autodesk.Revit.Exceptions.ArgumentException ex)
            {
                Debug.Print($"error: Handler() RevitCommandId for PrintPreview could not be posted");
                Debug.Print(ex.Message);
            }
            catch (Autodesk.Revit.Exceptions.InvalidOperationException ex)
            {
                Debug.Print($"error: Handler() RevitCommandId for PrintPreview could not be posted. This is likely as a result of multiple commands being posted (is the same event posting multiple commands?");
                Debug.Print(ex.Message);
            }
            // handle any other exception by printing to the debug console
            catch (Exception ex)
            {
                Debug.Print($"error: Handler() Unhandled/unexpected exception.");
                Debug.Print(ex.Message);
            }
        }

        public Result OnStartup(UIControlledApplication uiControlledApp)
        {
            Result r = Result.Failed;
            try
            {
                // register Handler for the DocumentSaved and DocumentSavedAs events
                uiControlledApp.ControlledApplication.DocumentSaved += Handler;
                uiControlledApp.ControlledApplication.DocumentSavedAs += Handler;
                r = Result.Succeeded;
            }
            // handle any exception by ignoring, but printing to the debug console
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                r = Result.Failed;
            }

            return r;
        }

        public Result OnShutdown(UIControlledApplication uiControlledApp)
        {
            Result r = Result.Failed;

            try
            {
                // de-register Handler for the DocumentSaved and DocumentSavedAs events
                uiControlledApp.ControlledApplication.DocumentSaved -= Handler;
                uiControlledApp.ControlledApplication.DocumentSavedAs -= Handler;
                r = Result.Succeeded;
            }
            // handle any exception by ignoring, but printing to the debug console
            catch (Exception e)
            {
                Debug.Print(e.Message);
                r = Result.Failed;
            }
            return r;
        }
    }
}
