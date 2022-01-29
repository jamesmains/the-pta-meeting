using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using Debug = UnityEngine.Debug;

/// <summary>
/// The Jim Jam Library Core
/// A simple controller for handling files you may want to use across several projects.
/// Say you have a door script that is simple and can be used on multiple projects, but on this other project you
/// fix bugs or add a new feature that you want to use on the first project. So with this library you can add
/// it to the library or update it if it's already in the project.
///
/// Features:
/// |- Manage a single location for reusable scripts
/// |- Get status of each file
/// |- Push updates to the library
/// |- Pull newer versions from the library
/// |- Add new files to library with context menu in project tab
/// 
/// </summary>
#if UNITY_EDITOR
namespace JimJam
{
    public class Resource
    {
        public enum ResourceStates
        {
            Same,
            Newer,
            Older,
            NoLocalCopy
        }

        public ResourceStates state;
        public string filePath;
        public string fileName;
    }
    public class JimJam_Library : EditorWindow
{
    private static string _libraryPath;
    Vector2 _scrollPosition;
    List<Resource> _resources = new List<Resource>();
    private bool _updating;
    private static string _dataPath;
    int _toolbarInt = 0;
    private int _tabIndex = 0;
    readonly string[] _toolbarStrings = {"Scripts", "Unity Packages"};
    private readonly string[] _targetFileTypes = {".cs",".unitypackage"};
    private string _targetType;
    
    [MenuItem("JimJam/Library %g")]
    public static void ShowWindow()
    {
        var window = GetWindow<JimJam_Library>("Jim-Jam Library");
        window.minSize = new Vector2(325, 100);
        _dataPath = Application.dataPath;
        _libraryPath = Environment.GetFolderPath(
            Environment.SpecialFolder.ApplicationData) + @"\JimJam\LibraryPackages";
        if (!Directory.Exists(_libraryPath))
            Directory.CreateDirectory(_libraryPath);
    }

    private void OnGUI()
    {
        // DRAWING A SPRITE TO THE WINDOW
        //var sprite = Resources.Load<Texture>("Icon");
        //if (GUILayout.Button(sprite,new GUIStyle()))
        //    Debug.Log("Transform");
        //GUI.DrawTexture(new Rect(0,0,100,100), sprite);
        //Debug.Log(sprite);
        
        _toolbarInt = GUILayout.Toolbar(_toolbarInt, _toolbarStrings);
        _targetType = _targetFileTypes[_toolbarInt];
        
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Refresh", GUILayout.Height(25),GUILayout.Width(125)))
            CheckForUpdates();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("View in Explorer", GUILayout.Height(25), GUILayout.Width(125)))
            Process.Start(_libraryPath);
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Backup", GUILayout.Height(25), GUILayout.Width(125)))
            SaveToBackup();
        GUILayout.EndHorizontal();
        GUILayout.Space(5);
        if (_tabIndex != _toolbarInt)
        {
            _tabIndex = _toolbarInt;
            CheckForUpdates();
        }
        _scrollPosition = GUILayout.BeginScrollView(
            _scrollPosition, GUILayout.ExpandHeight(true));
        
        // Go through each resource and create a section for it
        if (_resources != null && !_updating && _targetType != null)
            foreach (var file in _resources.ToList())
            {
                GUILayout.BeginHorizontal();
                string displayText = file.fileName;
                GUILayout.Label(displayText,GUILayout.Height(25));
                
                // Condition - The Library version of this resource is the SAME
                if (file.state == Resource.ResourceStates.Same)
                {
                    GUI.contentColor = Color.green;
                    GUILayout.Label("You have most recent file",GUILayout.Height(25), GUILayout.Width(155));
                }
                
                // Condition - The Library version of this resource is NEWER
                else if(file.state == Resource.ResourceStates.Newer)
                {
                    GUI.contentColor = Color.red;
                    GUILayout.Label("Needs to be updated",GUILayout.Height(25), GUILayout.Width(155));
                }
                
                // Condition - The Library version of this resource is OLDER 
                else if(file.state == Resource.ResourceStates.Older)
                {
                    GUI.contentColor = Color.yellow;
                    GUILayout.Label("You have a newer version",GUILayout.Height(25), GUILayout.Width(155));
                }
                
                // Condition - No Local Copy of Library Resource is found
                else if(file.state == Resource.ResourceStates.NoLocalCopy)
                {
                    GUI.contentColor = Color.gray;
                    GUILayout.Label("No local copy",GUILayout.Height(25), GUILayout.Width(155));
                }
                
                // Display interaction buttons for item
                GUI.contentColor = Color.white;
                if (_targetType == _targetFileTypes[0])
                {
                    if (GUILayout.Button("Save", GUILayout.Height(25), GUILayout.Width(45)))
                    {
                        SaveToLibrary(file.filePath,file.fileName);
                        continue;
                    }
                    if (GUILayout.Button("Load", GUILayout.Height(25), GUILayout.Width(45)))
                    {
                        LoadFromLibrary(file.filePath,file.fileName);
                        continue;
                    }
                    if (GUILayout.Button("Open", GUILayout.Height(25), GUILayout.Width(45)))
                    {
                        OpenFileInProject(file.filePath,file.fileName);
                        continue;
                    }
                }
                else if (GUILayout.Button("Load", GUILayout.Height(25), GUILayout.Width(80)))
                {
                    LoadFromLibrary(file.filePath,file.fileName);
                    continue;
                }
                
                GUILayout.EndHorizontal();
            }
        GUILayout.EndScrollView();
    }

    void SaveToBackup()
    {
        Debug.Log("Backing up!");
        string backupPath = _libraryPath + "/Backups";
        CheckForDirectory(backupPath);
        var files = Directory.GetFiles(_libraryPath).ToList();
        foreach (var f in files)
        {
            if (f.Contains(_targetType))
            {
                File.Copy(f,backupPath+"/"+f.Substring(f.LastIndexOf('\\') + 1),true);
            }
        }
    }

    void SaveToLibrary(string fp,string fn)
    {
        var localCopy = Directory.GetFiles(_dataPath, fn, SearchOption.AllDirectories);
        if(localCopy.Length>0)
            File.Copy(localCopy[0],fp,true);
        else Debug.LogWarning("JJL Warning! Attempting to push a non-existent file to the Library!");
        AssetDatabase.Refresh();
        CheckForUpdates();
    }

    void OpenFileInProject(string fp,string fn)
    {
        var localCopy = Directory.GetFiles(_dataPath, fn, SearchOption.AllDirectories);
        if (localCopy.Length > 0)
        {
            int splitIndex = localCopy[0].LastIndexOf("/Assets");
            localCopy[0] = localCopy[0].Substring(splitIndex+1);
            AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<MonoScript>(localCopy[0]));
        }
            
        
        

        //using Process fileopener = new Process();
        //
        //fileopener.StartInfo.FileName = "Explorer.exe";
        //fileopener.StartInfo.Arguments = "\"" + fp + "\"";
        //fileopener.Start();
    }
    
    void LoadFromLibrary(string fp,string fn)
    {
        var localCopy = Directory.GetFiles(_dataPath, fn, SearchOption.AllDirectories);
        if (localCopy.Length > 0)
        {
            File.Copy(fp, localCopy[0],true);
            //File.Replace(fp, s[0],s[0]);    
        }
        else File.Copy(fp, _dataPath+"/"+fn);
        AssetDatabase.Refresh();
        CheckForUpdates();
        
    }

    void CheckForUpdates()
    {
        _updating = true;
        _resources.Clear();
        var files = Directory.GetFiles(_libraryPath).ToList();
        foreach (var f in files)
        {
            if (f.Contains(_targetType))
            {
                var r = new Resource {filePath = f, fileName = f.Substring(f.LastIndexOf('\\') + 1)};
                r.state = CompareVersions(r.filePath, r.fileName);
                _resources.Add(r);
            }
        }
        _updating = false;
    }

    void CheckForDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            Debug.Log("Creating directory at: " + path);
            Directory.CreateDirectory(path);
        }
    }
    
    Resource.ResourceStates CompareVersions(string fp,string fn)
    {
        string dataPath = Application.dataPath;
        var localCopy = Directory.GetFiles(dataPath, fn, SearchOption.AllDirectories);
        if (localCopy.Length > 0)
        {
            DateTime lastModified = File.GetLastWriteTime(fp);
            
            if(lastModified > File.GetLastWriteTime(localCopy[0]))
                return Resource.ResourceStates.Newer;
            if(lastModified == File.GetLastWriteTime(localCopy[0]))
                return Resource.ResourceStates.Same;
            
            return Resource.ResourceStates.Older;    
        }
        return Resource.ResourceStates.NoLocalCopy;
    }
    private void OnValidate()
    {
        _dataPath = Application.dataPath;
        _libraryPath = Environment.GetFolderPath(
            Environment.SpecialFolder.ApplicationData) + @"\JimJam\LibraryPackages";
        if (!Directory.Exists(_libraryPath))
            Directory.CreateDirectory(_libraryPath);
        if(_targetType!=null)
            CheckForUpdates();
    }
    
    [MenuItem("Assets/JJL - Send to Library", false, 0)]
    static void SendFileToLibrary() {
        var obj = Selection.activeObject;
        var path = AssetDatabase.GetAssetPath(obj.GetInstanceID());
        if (path.Length > 0 && (path.Contains(".cs") || path.Contains(".unitypackage")))
        {
            Debug.Log(path);
            string newFileName = path.Substring(path.LastIndexOf('/')+1);
            if (File.Exists(path))
            {
                File.Copy(path,_libraryPath+"/"+newFileName,true);
                AssetDatabase.Refresh();
            }
        }
        else
        {
            Debug.LogWarning("JJL Warning! " + path + " is not a supported file type");
        }
        AssetDatabase.Refresh();
    }
}
}


#endif