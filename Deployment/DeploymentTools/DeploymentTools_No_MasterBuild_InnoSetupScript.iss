[Setup]
Compression=lzma
AppName=Deployment Tools
AppVerName=Deployment Tools 2.1.2
AppPublisher=SurePayroll, Inc.
AppPublisherURL=http://www.surepayroll.com/
AppSupportURL=http://www.surepayroll.com/
AppCopyright=SurePayroll
DefaultDirName={pf}\SurePayroll\DeploymentTools
DefaultGroupName=SurePayroll\Deployment Tools
OutputBaseFilename=DeploymentToolsNoMasterBuildSetup
OutputDir=InnoSetupOutput
SetupIconFile=C:\Source\SurePayroll\Dev\SurePayroll10\Deployment\DeploymentTools\DeploymentTools\Icon1.ico
SolidCompression=true
AppendDefaultDirName=false

[Files]
Source: DeploymentTools\bin\Debug\config.cfg; DestDir: {app}; Flags: onlyifdoesntexist uninsneveruninstall uninsremovereadonly
Source: DeploymentTools\bin\Debug\DeploymentTools.exe; DestDir: {app}; Flags: promptifolder uninsremovereadonly replacesameversion
Source: DeploymentTools\bin\Debug\DeploymentTools.pdb; DestDir: {app}; Flags: replacesameversion
Source: DeploymentTools\bin\Debug\DeploymentTools.exe.config; DestDir: {app}; Flags: uninsremovereadonly replacesameversion
Source: DeploymentTools\bin\Debug\EDB.dll; DestDir: {app}; Flags: replacesameversion
Source: DeploymentTools\bin\Debug\EDB.pdb; DestDir: {app}; Flags: replacesameversion
Source: DeploymentTools\bin\Debug\EMInterfaces.dll; DestDir: {app}; Flags: replacesameversion
Source: DeploymentTools\bin\Debug\EMInterfaces.pdb; DestDir: {app}; Flags: replacesameversion
Source: DeploymentTools\bin\Debug\EUtil.dll; DestDir: {app}; Flags: replacesameversion
Source: DeploymentTools\bin\Debug\EUtil.pdb; DestDir: {app}; Flags: replacesameversion
Source: DeploymentTools\bin\Debug\TreeSync.dll; DestDir: {app}; Flags: replacesameversion
Source: DeploymentTools\bin\Debug\TreeSync.pdb; DestDir: {app}; Flags: replacesameversion
Source: DeploymentTools\bin\Debug\DTPluginBase.dll; DestDir: {app}; Flags: replacesameversion
Source: DeploymentTools\bin\Debug\DTPluginBase.pdb; DestDir: {app}; Flags: replacesameversion
Source: COMRegistrationPlugin\bin\Debug\COMRegistrationPlugin.dll; DestDir: {app}; Flags: replacesameversion
Source: COMRegistrationPlugin\bin\Debug\COMRegistrationPlugin.pdb; DestDir: {app}; Flags: replacesameversion
Source: COMRegistrationPlugin\bin\Debug\Interop.MTSAdmin.dll; DestDir: {app}
Source: ProjectDeployPackage\bin\Debug\ProjectDeployPackagePlugin.dll; DestDir: {app}; Flags: replacesameversion
Source: ProjectDeployPackage\bin\Debug\ProjectDeployPackagePlugin.pdb; DestDir: {app}; Flags: replacesameversion
Source: ServiceDeployPlugin\bin\Debug\ServiceDeployPlugin.dll; DestDir: {app}; Flags: replacesameversion
Source: ServiceDeployPlugin\bin\Debug\ServiceDeployPlugin.pdb; DestDir: {app}; Flags: replacesameversion
Source: DeploymentTools\bin\Debug\ParallelTasks.dll; DestDir: {app}; Flags: replacesameversion
Source: DeploymentTools\bin\Debug\ParallelTasks.pdb; DestDir: {app}; Flags: replacesameversion
Source: DeploymentTools\bin\Debug\TreeSync.dll; DestDir: {app}; Flags: replacesameversion
Source: DeploymentTools\bin\Debug\TreeSync.pdb; DestDir: {app}; Flags: replacesameversion
Source: depedencies_backup\SQLServer2008\Microsoft.SqlServer.ConnectionInfo.dll; DestDir: {app}
Source: depedencies_backup\SQLServer2008\Microsoft.SqlServer.ConnectionInfoExtended.dll; DestDir: {app}
Source: depedencies_backup\SQLServer2008\Microsoft.SqlServer.Management.Sdk.Sfc.dll; DestDir: {app}
Source: depedencies_backup\SQLServer2008\Microsoft.SqlServer.RegSvrEnum.dll; DestDir: {app}
Source: depedencies_backup\SQLServer2008\Microsoft.SqlServer.Replication.dll; DestDir: {app}
Source: depedencies_backup\SQLServer2008\Microsoft.SqlServer.Rmo.dll; DestDir: {app}
Source: depedencies_backup\SQLServer2008\Microsoft.SqlServer.ServiceBrokerEnum.dll; DestDir: {app}
Source: depedencies_backup\SQLServer2008\Microsoft.SqlServer.Smo.dll; DestDir: {app}
Source: depedencies_backup\SQLServer2008\Microsoft.SqlServer.SmoExtended.dll; DestDir: {app}
Source: depedencies_backup\SQLServer2008\Microsoft.SqlServer.SqlEnum.dll; DestDir: {app}
Source: depedencies_backup\SQLServer2008\Microsoft.SqlServer.SqlWmiManagement.dll; DestDir: {app}
Source: depedencies_backup\SQLServer2008\Microsoft.SqlServer.WmiEnum.dll; DestDir: {app}


[Languages]
Name: english; MessagesFile: compiler:Default.isl

[Tasks]
Name: desktopicon; Description: {cm:CreateDesktopIcon}; GroupDescription: {cm:AdditionalIcons}
Name: quicklaunchicon; Description: {cm:CreateQuickLaunchIcon}; GroupDescription: {cm:AdditionalIcons}

[Icons]
Name: {group}\Deployment Tools; Filename: {app}\DeploymentTools.exe
Name: {group}\{cm:UninstallProgram,Deployment Tools}; Filename: {uninstallexe}
Name: {commondesktop}\Deployment Tools; Filename: {app}\DeploymentTools.exe; Tasks: desktopicon
Name: {userappdata}\Microsoft\Internet Explorer\Quick Launch\Deployment Tools; Filename: {app}\DeploymentTools.exe; Tasks: quicklaunchicon

[Run]
Filename: {app}\DeploymentTools.exe; Description: {cm:LaunchProgram,Deployment Tools}; Flags: nowait postinstall skipifsilent
