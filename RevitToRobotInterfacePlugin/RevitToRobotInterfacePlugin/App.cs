#region Namespaces
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.REX.Framework;
using REX.DRevit2Robot.REX.Common;
using REX.DRevit2Robot;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Autodesk.Revit.DB.Events;
#endregion


namespace RevitToRobotInterfacePlugin
{
    internal class App : IExternalApplication
    {
        // to access the internal sealed classes, the assembly of one of the public classes (RevitToRobotCmd) is retrieved
        private System.Reflection.Assembly Assem = typeof(RevitToRobotCmd).Assembly;

        private void EvilRevitAppActivate()
        {
            // get instance of internal sealed class RevitUtil via system reflection
            Type typeInfo = Assem.GetType("REX.DRevit2Robot.RevitUtl");

            // get and invoke method RevitAppActivate
            // null x2 as no object (static) and no parameters
            MethodInfo method = Type.GetType(typeInfo.AssemblyQualifiedName).GetMethod("RevitAppActivate");
            method.Invoke(null, null);
        }

        private void EvilPrepareToExecution(ref RevitToRobotCmd revitToRobotCmd)
        {
            // get instance of internal sealed class RevitToRobotCmd via system reflection
            Type typeInfo = Assem.GetType("REX.DRevit2Robot.RevitToRobotCmd");

            // get and invoke method RevitAppActivate
            // null x2 as no object (static) and no parameters
            MethodInfo method = Type.GetType(typeInfo.AssemblyQualifiedName).GetMethod("PrepareToExecution", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(revitToRobotCmd, null);
        }
        private void EvilInitRevitContext()
        {
            // get instance of internal sealed class RevitToRobotCmd via system reflection
            Type typeInfo = Assem.GetType("REX.DRevit2Robot.Revit2Robot");

            // get and invoke method RevitAppActivate
            // null x2 as no object (static) and no parameters
            MethodInfo method = Type.GetType(typeInfo.AssemblyQualifiedName).GetMethod("InitRevitContext");
            method.Invoke(null, null);
        }

        private void EvilTheCommonOptions_FromParams()
        {
            // get instance of internal sealed class RevitToRobotCmd via system reflection
            Type typeInfo = Assem.GetType("REX.DRevit2Robot.Revit2Robot");

            // get the TheCommonOptions parameter of Revit2Robot class
            FieldInfo field = typeInfo.GetField("TheCommonOptions", BindingFlags.Public | BindingFlags.Static);

            // get the instance of the TheCommonOptions in use by Revit2Robot
            clCommonOptions obj = (clCommonOptions)field.GetValue(null);

            // call the FromParams method
            obj.FromParams();
        }

        private void EvilTheSendOptions_FromParams()
        {
            // get instance of internal sealed class RevitToRobotCmd via system reflection
            Type typeInfo = Assem.GetType("REX.DRevit2Robot.Revit2Robot");

            // get the TheSendOptions parameter of Revit2Robot class
            FieldInfo field = typeInfo.GetField("TheSendOptions", BindingFlags.Public | BindingFlags.Static);

            // get the instance of the TheSendOptions in use by Revit2Robot
            clSendOptions obj = (clSendOptions)field.GetValue(null);

            // call the FromParams method
            obj.FromParams();
        }

        private void EvilUpdateOptions_FromParams()
        {
            // get instance of internal sealed class RevitToRobotCmd via system reflection
            Type typeInfo = Assem.GetType("REX.DRevit2Robot.Revit2Robot");

            // get the TheUpdateOptions parameter of Revit2Robot class
            FieldInfo field = typeInfo.GetField("TheUpdateOptions", BindingFlags.Public | BindingFlags.Static);

            // get the instance of the TheUpdateOptions in use by Revit2Robot
            clUpdateOptions obj = (clUpdateOptions)field.GetValue(null);

            // call the FromParams method
            obj.FromParams();
        }

        private void EvilTheRevitJournal_SetUserInteraction(bool status)
        {
            // get instance of internal sealed class RevitToRobotCmd via system reflection
            Type typeInfo = Assem.GetType("REX.DRevit2Robot.Revit2Robot");

            // get the TheRevitJournal parameter of Revit2Robot class
            FieldInfo field = typeInfo.GetField("TheRevitJournal", BindingFlags.Public | BindingFlags.Static);

            clRevitJournal obj = (clRevitJournal)field.GetValue(null);

            field = obj.GetType().GetField("m_userInteraction", BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(obj, status);
        }

        private Result EvilExportToRobot(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Result result;

            // the general structure of operations following here adheres tp the decompile code for
            // the Robot & REX, stripped down top remove logging, etc... for simplicity

            try
            {
                REX.DRevit2Robot.Application AppRef = new REX.DRevit2Robot.Application();
                REXContext rexContext = new REXRevitContext().Get("2022", commandData.Application.Application.VersionName, commandData.Application.Application.Language, commandData, ref message, elements);
                rexContext.Control.Mode = REXMode.Dialog;
                rexContext.Control.ControlMode = REXControlMode.ModalDialog;
                rexContext.Control.Parent = (object)null;
                rexContext.Product.Type = REXInterfaceType.Revit;
                REXEnvironment rexEnvironment = new REXEnvironment("2022");
                if (rexEnvironment.LoadEngine(ref rexContext))
                {
                    if (AppRef.Create(ref rexContext))
                    {
                        string location = Assembly.GetExecutingAssembly().Location;
                        string withoutExtension = Path.GetFileNameWithoutExtension(location);
                        string fullName = Directory.GetParent(location).FullName;
                        if (rexEnvironment != null & Directory.Exists(fullName))
                        {
                            rexEnvironment.RegisterInternalModuleName(withoutExtension);
                            rexEnvironment.RegisterModulePath(withoutExtension, fullName);
                        }

                        // this is evil
                        // this is a violation of the Revit/Robot developer intentions, they are perfectly at liberty to change or remove internal classes between releases 
                        // this is an internal sealed class that is not meant to be publicly available
                        // this was achieved by using a .NET decompiler and System.Reflection
                        Type typeInfo = Assem.GetType("REX.DRevit2Robot.Revit2Robot");

                        REX.DRevit2Robot.REX.Common.Engine.Support.REXAssemblies rexAssem = new REX.DRevit2Robot.REX.Common.Engine.Support.REXAssemblies();
                        rexAssem.Initialize();

                        // get and invoke InitRevitContext
                        // null x2 as no object (static) and no parameters for Revit2Robot
                        EvilInitRevitContext();

                        // set the Revit2Robot class static variable m_resultMessage
                        FieldInfo field = typeInfo.GetField("m_resultMessage", BindingFlags.Public | BindingFlags.Static);
                        field.SetValue(null, "");

                        // set the Revit2Robot class static variable m_result
                        field = typeInfo.GetField("m_result", BindingFlags.Public | BindingFlags.Static);
                        field.SetValue(null, Result.Succeeded);

                        // set the Revit2Robot class static variable TheLogger
                        field = typeInfo.GetField("TheLogger", BindingFlags.Public | BindingFlags.Static);
                        field.SetValue(null, new REXLogger());

                        // set the Revit2Robot class static variable TheCommonOptions
                        field = typeInfo.GetField("TheCommonOptions", BindingFlags.Public | BindingFlags.Static);
                        field.SetValue(null, new clCommonOptions());

                        // set the Revit2Robot class static variable TheSendOptions
                        field = typeInfo.GetField("TheSendOptions", BindingFlags.Public | BindingFlags.Static);
                        field.SetValue(null, new clSendOptions());

                        // set the Revit2Robot class static variable TheUpdateOptions
                        field = typeInfo.GetField("TheUpdateOptions", BindingFlags.Public | BindingFlags.Static);
                        field.SetValue(null, new clUpdateOptions());

                        // get and invoke SetRevitDocument
                        // null as no object (static), passing commandData
                        MethodInfo method = Type.GetType(typeInfo.AssemblyQualifiedName).GetMethod("SetRevitDocument");
                        method.Invoke(null, new[] { commandData });

                        // EvilPrepareToExecution requres an instance to operate on as it is a non static method,
                        // however it does not appear that the instance actually matters from the decompiled code
                        // as only static variables are updated i.e. it could have been static, but happens to not be
                        // This means that an instance must be created, despite being useless and operated on
                        RevitToRobotCmd revitToRobotCmd = new RevitToRobotCmd();
                        EvilPrepareToExecution(ref revitToRobotCmd);

                        // set various static options 
                        EvilTheCommonOptions_FromParams();
                        EvilTheSendOptions_FromParams();
                        EvilUpdateOptions_FromParams();

                        // get the TheCommonOptions parameter of Revit2Robot class
                        field = typeInfo.GetField("TheCommonOptions", BindingFlags.Public | BindingFlags.Static);

                        // get the instance of the TheCommonOptions in use by Revit2Robot
                        clCommonOptions obj = (clCommonOptions)field.GetValue(null);

                        // set the TheFilePath property to the SilentModeFileName i.e. "" 
                        field = typeInfo.GetField("TheFilePath", BindingFlags.Public | BindingFlags.Static);
                        field.SetValue(null, obj.SilentModeFileName);

                        // SendToRobot is the method underneath all others that actually does the sending to robot
                        SendToRobot s2r = new SendToRobot(false);

                        // disable user interaction property of the revit journal, this prevents the popup that
                        // follows the export that asks the user if they want to view the journal entries written
                        EvilTheRevitJournal_SetUserInteraction(false);

                        // send to robot 
                        s2r.Execute(commandData, ref message, elements);

                        // re-enable user interaction property of the revit journal 
                        EvilTheRevitJournal_SetUserInteraction(true);

                        // re-activate the revit window, this prevents any switching to robot i.e. so it remains in the background 
                        EvilRevitAppActivate();

                        // determine successfulness of operation
                        // strictly cancelled result should not be possible given it is 'automated'
                        result = rexContext.Extension.Result != REXResultType.Cancelled ? (rexContext.Extension.Result != REXResultType.Failed ? Result.Succeeded : Result.Failed) : Result.Cancelled;
                    }
                    else
                    {
                        result = Result.Failed;
                    }
                }
                else
                {
                    result = Result.Failed;
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                result = Result.Failed;
            }

            return result;
        }

        private void Handler(object sender, EventArgs args)
        {
            // setup the top level objects in the Revit Platform API are application and document.
            UIApplication uiapp = new UIApplication((Autodesk.Revit.ApplicationServices.Application)sender);
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            Document doc = uidoc.Document;

            try
            {
                // start transaction on active document
                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("Posting command from Handler");

                    // todo needs check that robot is running otherwise export will fail and the user will get a
                    // dialog stating that there is an error and to contact the plugin provider (me!)
                    // ...

                    // create an instance of ExternalCommandData
                    // system reflection is required as the constructor is declared as internal
                    // ExternalCommandData provides the application and view so using system reflection it can be 'faked' from the provided sender arg of the handler
                    ExternalCommandData exCommandData = 
                        (ExternalCommandData)typeof(ExternalCommandData).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null).Invoke(null);

                    // set the application field
                    exCommandData.Application = uiapp;

                    // set the view field
                    if (args.GetType() == typeof(DocumentSavedEventArgs))
                    {
                        exCommandData.View = ((DocumentSavedAsEventArgs)args).Document.ActiveView;
                    }
                    else if (args.GetType() == typeof(DocumentSavedAsEventArgs))
                    {
                        exCommandData.View = ((DocumentSavedAsEventArgs)args).Document.ActiveView;
                    }
                    else
                    {
                        Debug.Print($"error: Handler() Did not expect to be called with EventArgs subclass: {args.GetType()}.");
                        exCommandData.View = null;
                    }

                    // create a message reference to provide (this is only used when there is an error)
                    string message = "";

                    // create an element set (this is only used when there is an error)
                    ElementSet elementSet = new ElementSet();

                    // export to robot
                    Result r = EvilExportToRobot(exCommandData, ref message, elementSet);

                    // print the result to the debug output
                    Debug.Print($"Result of EvilExportToRobot is {r}");

                    // complete the transaction
                    tx.Commit();
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
