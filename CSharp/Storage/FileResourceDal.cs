using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Company.Solution.Project;
using Company.Solution.Project.Logging;
using NLog;

namespace Company.Solution.Project.Dal
{
    /// <summary>
    /// Implements CRUD operations for a file store.
	/// TObj data must be xml serializable
    /// </summary>
    public class FileResourceDal<TObj>
    {
        private readonly SerializableDictionary<string, TObj> _cachedData = new SerializableDictionary<string, TObj>();
        private readonly string _assemblyPath;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private bool _cacheIsReady = false;
        /// <summary>
        /// This is just the filename.  The program will figure out the path.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// In order to remain flexable, the ctor is not designed to accept a filename and then load it.
        /// This is taking a more flexable approach of allowing the filename to be set later. 
        /// </summary>
        public FileResourceDal()
        {
            LogHelper.Log(LogLevel.Trace, _logger, "FileResourceDal Init");
            // Determine the path to the executing assembly.
            _assemblyPath = AssemblyHelpers.AssemblyDirectory;
        }

        public void CreateNewResource(string name, TObj data)
        {
            ValidateReady();
            if (!_cachedData.ContainsKey(name))
            {
                _cachedData.Add(name, data);
                UpdateDataStore();
            }
        }

        public void DeleteResource(string name)
        {
            ValidateReady();
            if (_cachedData.ContainsKey(name))
            {
                _cachedData.Remove(name);
            }
            UpdateDataStore();
        }

        public TObj GetResource(string name)
        {
            ValidateReady();
            object result = null;
            if (_cachedData.ContainsKey(name))
            {
                result = _cachedData[name];
            }

            return (TObj)result;
        }

        public List<TObj> GetResourceCollection()
        {
            ValidateReady();
            List<TObj> dataList = new List<TObj>();
            foreach (var key in _cachedData.Keys)
            {
                dataList.Add(_cachedData[key]);
            }

            return dataList;
        }

        public void UpdateResource(string name, TObj data)
        {
            ValidateReady();
            if (_cachedData.ContainsKey(name))
            {
                _cachedData[name] = data;
            }
            UpdateDataStore();
        }

        public void CreateOrUpdateResource(string name, TObj data)
        {
            ValidateReady();
            if (_cachedData.ContainsKey(name))
            {
                _cachedData[name] = data; 
            }
            else
            {
                _cachedData.Add(name, data);
            }
            UpdateDataStore();
        }

        /// <summary>
        /// Validate that we have the FileName property set, the file exists, and that data is loaded.
        /// </summary>
        private void ValidateReady()
        {
            // If the filename is not set, blow up big time.
            if (string.IsNullOrEmpty(FileName))
                throw new Exception("Filename not set.");
            // Make sure the file exists
            var fullPath = Path.Combine(_assemblyPath, FileName);
            if (!File.Exists(fullPath))
            {
                // Create the file and then close it.
                using (var stream = new StreamWriter(fullPath, true))
                {
                    // We don't need to do anything here as this just creates the file.
                    _cacheIsReady = true;
                }
            }
            // Make sure we have our existing data loaded and ready.
            if (!_cacheIsReady)
            {
                LoadDataStore();
            }
        }

        /// <summary>
        /// Writes the contents of the cached data to the specified filename
        /// </summary>
        private void UpdateDataStore()
        {
            var serializer = new XmlSerializer(typeof(SerializableDictionary<string, TObj>));

            using (FileStream fs = new FileStream(Path.Combine(_assemblyPath, FileName), FileMode.Create))
            {
                serializer.Serialize(fs, _cachedData);
            }
        }

        private void LoadDataStore()
        {
            var fullPath = Path.Combine(_assemblyPath, FileName);
            SerializableDictionary<string, TObj> results = null;
            if (File.Exists(fullPath))
            {
                try
                {
                    using (TextReader reader = new StreamReader(fullPath))
                    {
                        var serializer = new XmlSerializer(typeof(SerializableDictionary<string, TObj>));
                        _cachedData.Clear();
                        results = (SerializableDictionary<string, TObj>) serializer.Deserialize(reader);
                    }

                    if (results != null)
                    {
                        foreach (string key in results.Keys)
                        {
                            _cachedData.Add(key, results[key]);
                        }
                    }

                }
                catch (Exception e)
                {
                    LogHelper.Log(LogLevel.Trace, _logger, $"Failed to read data store. {e.Message} ", e);
                }
            }
            // Set our flag to true so we can add/remove/change cache data
            _cacheIsReady = true;
        }
    }
}
