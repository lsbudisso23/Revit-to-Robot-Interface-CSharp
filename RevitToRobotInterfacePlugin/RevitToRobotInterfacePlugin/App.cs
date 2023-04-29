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
                // if the addin has been enabled (Add-ins -> External Tools -> Command RevitToRobotInterfacePlugin button pressed
                // by the user in active revit session/document)
                if (Command.Enabled)
                {
                    // start transaction on active document
                    using (Transaction tx = new Transaction(doc))
                    {
                        tx.Start("Posting command from Handler");

                        // create instance of DirectRevitAccess
                        // this part of the API is undocument but was found by analysing the journal
                        // entries upon manual invocation as well as through the helpful blog post:
                        // https://forums.autodesk.com/t5/revit-api-forum/calling-robot-structural-analysis-using-revit-api/m-p/5973473#M13549
                        REX.DRevit2Robot.DirectRevitAccess dra = new REX.DRevit2Robot.DirectRevitAccess();

                        // invoke the execute method of this command, providing the 'saved' command data,
                        // message, and elements
                        Result r = dra.Execute(Command.CommandData, ref Command.Message, Command.Elements);

                        // print the result to the debug output
                        Debug.Print($"Result of REX.DRevit2Robot.DirectRevitAccess.Execute() is {r}");

                        // complete the transaction
                        tx.Commit();
                    }
                }
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
