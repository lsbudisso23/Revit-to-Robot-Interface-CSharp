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

                    // get revit cmd id for robot structural analysis 
                    // this command string is obtained upon issuing the command manually (via the gui) in revit
                    // and observing the external command printed in the journal file for the active session
                    // (C:\Users\<username>\AppData\Local\Autodesk\Revit\Autodesk Revit 2022\Journals) as stated in
                    // https://help.autodesk.com/view/RVT/2022/ENU/?guid=Revit_API_Revit_API_Developers_Guide_Advanced_Topics_Commands_html
                    // The journal entry is as follows:
                    //   ' [Jrn.Tooltip] Rvt.Attr.Tooltip.CommandID: CustomCtrl_%CustomCtrl_%CustomCtrl_%Analyze%Structural Analysis%AnalysisAndCodeCheck%RobotStructuralAnalysisLink Rvt.Attr.Tooltip.ElapsedTime: 0.1351142 Rvt.Attr.Tooltip.Enabled: True Rvt.Attr.Tooltip.IsFirst: True Rvt.Attr.Tooltip.Key: CustomCtrl_%CustomCtrl_%CustomCtrl_%Analyze%Structural Analysis%AnalysisAndCodeCheck%RobotStructuralAnalysisLink Rvt.Attr.Tooltip.Progressive: True Rvt.Attr.Tooltip.Progressive.Delay: 2 
                    //   'E 28-Feb-2023 20:46:47.151;   0:< 
                    //   Jrn.RibbonEvent "Execute external command:CustomCtrl_%CustomCtrl_%CustomCtrl_%Analyze%Structural Analysis%AnalysisAndCodeCheck%RobotStructuralAnalysisLink:REX.DRevit2Robot.DirectRevitAccess" 
                    string s = "CustomCtrl_%CustomCtrl_%CustomCtrl_%Analyze%Structural Analysis%AnalysisAndCodeCheck%RobotStructuralAnalysisLink";
                    RevitCommandId cmd = RevitCommandId.LookupCommandId(s);

                    // fixme
                    // unfortunately this does not work as intended
                    // no exceptions are caused, and the cmd id for this string is successfully resolved (not null),
                    // however upon investigation, revit does not appear to understand this command as the following 
                    // entry is written to the current journal:
                    //   'C 01-Mar-2023 15:25:19.693;  DBG_INFO: CustomCtrl_%CustomCtrl_%CustomCtrl_%Analyze%Structural Analysis%AnalysisAndCodeCheck%RobotStructuralAnalysisLinkdoes not exist.: line 894 of E:\Ship\2022_px64\Source\API\RevitAPIUI\Objects\APIUIApplicationHandwritten.cpp. 
                    // likewise with the python version, using PostCommand appears to be a dead end...


                    // post the robot structural analysis command
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
                Debug.Print($"error: Handler() RevitCommandId for Robot Structural Analysis could not be determined");
                Debug.Print(ex.Message);
            }
            catch (Autodesk.Revit.Exceptions.ArgumentException ex)
            {
                Debug.Print($"error: Handler() RevitCommandId for Robot Structural Analysis could not be posted");
                Debug.Print(ex.Message);
            }
            catch (Autodesk.Revit.Exceptions.InvalidOperationException ex)
            {
                Debug.Print($"error: Handler() RevitCommandId for Robot Structural Analysis could not be posted. This is likely as a result of multiple commands being posted (is the same event posting multiple commands?");
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
