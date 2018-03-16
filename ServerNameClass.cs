using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fougerite;
using System.IO;
using UnityEngine;
using Facepunch.ID;

namespace ServerName
{
    public class ServerNameClass : Fougerite.Module
    {
        public override string Name { get { return "ServerName"; } }
        public override string Author { get { return "Salva/Juli"; } }
        public override string Description { get { return "ServerName"; } }
        public override Version Version { get { return new Version("1.0"); } }

        public string red = "[color #B40404]";
        public string blue = "[color #81F7F3]";
        public string green = "[color #82FA58]";
        public string yellow = "[color #F4FA58]";
        public string orange = "[color #FF8000]";
        public string pink = "[color #FA58F4]";
        public string white = "[color #FFFFFF]";

        public bool RustBuster2016ServerSUPPORT = false;
        public string RBVersion = " ";
        public string SvColor = "[color #FFFFFF]";
        public string SvName = "ServerName";
        public IniParser Settings;
        public override void Initialize()
        {
            Hooks.OnServerLoaded += OnServerLoaded;
            Hooks.OnConsoleReceived += OnConsoleReceived;
            Hooks.OnCommand += OnCommand;
        }
        
        public override void DeInitialize()
        {
            Hooks.OnServerLoaded -= OnServerLoaded;
            Hooks.OnConsoleReceived -= OnConsoleReceived;
            Hooks.OnCommand -= OnCommand;
        }
        public void OnServerLoaded()
        {
            foreach (var x in Fougerite.ModuleManager.Modules)
            {
                if (x.Plugin.Name == "RustBusterServer")
                {
                    RBVersion = "[color yellow]" + "[RB " + x.Plugin.Version + "] ";
                    break;
                }
            }

            ReloadConfig();
            server.hostname = RBVersion + SvColor + SvName;
            Rust.Steam.Server.UpdateServerTitle();
        }
        public void OnConsoleReceived(ref ConsoleSystem.Arg arg, bool external)
        {
            if (arg.Class == "help" && arg.Function == "help" && ((arg.argUser != null && arg.argUser.admin) || arg.argUser == null))
            {
                Logger.LogError("PLUGIN: " + Name + " " + Version);
                Logger.Log("servername.reload - Read the Settings.ini file and apply in a new name");
                Logger.Log("");
            }
            if (arg.Class == "servername" && arg.Function == "reload" && ((arg.argUser != null && arg.argUser.admin) || arg.argUser == null))
            {
                Logger.Log(Name + " Old Server Name:  " + RBVersion + SvColor + SvName);
                ReloadConfig();
                Logger.Log(Name + " New Server Name:  " + RBVersion + SvColor + SvName);
                Logger.Log(Name + " Settings has been Reloaded :)");
                server.hostname = RBVersion + SvColor + SvName;
                Rust.Steam.Server.UpdateServerTitle();
            }
        }
        public void OnCommand(Fougerite.Player player, string cmd, string[] args)
        {
            if (!player.Admin) { return; }
            if (cmd == "servername")
            {
                if (args.Length == 0)
                {
                    player.MessageFrom(Name, "/servername " + blue + " List of commands");
                    player.MessageFrom(Name, "/servername reload " + blue + " Reload and apply the Settings");
                }
                else
                {
                    if (args[0] == "reload")
                    {
                        player.MessageFrom(Name, "Old Server Name:  " + RBVersion + SvColor + SvName);
                        ReloadConfig();
                        player.MessageFrom(Name, "New Server Name:  " + RBVersion + SvColor + SvName);
                        player.MessageFrom(Name, green +"Settings has been Reloaded :)");
                        server.hostname = RBVersion + SvColor + SvName;
                        Rust.Steam.Server.UpdateServerTitle();
                        
                    }
                }
            }
        }
        private void ReloadConfig()
        {
            if (!File.Exists(Path.Combine(ModuleFolder, "Settings.ini")))
            {
                File.Create(Path.Combine(ModuleFolder, "Settings.ini")).Dispose();
                Settings = new IniParser(Path.Combine(ModuleFolder, "Settings.ini"));
                Settings.AddSetting("ServerName", "Color", "[color #FFFFFF]");
                Settings.AddSetting("ServerName", "Name", "Edit this whit the name you want");
                Logger.Log(Name + " Plugin: New Settings File Created!");
                Settings.Save();
                ReloadConfig();
            }
            else
            {
                Settings = new IniParser(Path.Combine(ModuleFolder, "Settings.ini"));
                if (Settings.ContainsSetting("ServerName", "Color") &&
                    Settings.ContainsSetting("ServerName", "Name"))
                {
                    Settings = new IniParser(Path.Combine(ModuleFolder, "Settings.ini"));
                    try
                    {
                        SvColor = Settings.GetSetting("ServerName", "Color");
                        SvName = Settings.GetSetting("ServerName", "Name");
                        Logger.Log(Name + " Plugin: Settings file Loaded!");
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(Name + " Plugin: Detected a problem in the configuration");
                        Logger.Log("ERROR -->" + ex.Message);
                        File.Delete(Path.Combine(ModuleFolder, "Settings.ini"));
                        Logger.LogError(Name + " Plugin: Deleted the old configuration file");
                        ReloadConfig();
                    }

                }
                else
                {
                    Logger.LogError(Name + " Plugin: Detected a problem in the configuration (lost key)");
                    File.Delete(Path.Combine(ModuleFolder, "Settings.ini"));
                    Logger.LogError(Name + " Plugin: Deleted the old configuration file");
                    ReloadConfig();
                }
                return;
            }
        }
    }
}
